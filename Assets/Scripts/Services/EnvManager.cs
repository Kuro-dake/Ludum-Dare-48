using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YamlDotNet.RepresentationModel;

[CreateAssetMenu(fileName = "EnvManager", menuName = "Service/EnvManager", order = 0)]

public class EnvManager : Service
{
    public bool manual_env_manipulation = false;
    [System.Serializable]
    public class EnvPreset
    {
        public string desc;
        public Color[] clouds;
        public Color[] grass = new Color[2];
        public Color[] tree_bark;
        public Color ground;
        public Color[] mountain;
        public Color[] tree_crown = new Color[2];
        public Color[] bush = new Color[2];
        public Color[] sky = new Color[2];
        public Color[] light = new Color[2];
        public float[] light_intensity = new float[2];
        public Color fscreen_color;
        public int level;
    }
    public List<EnvPreset> env_presets;
    [SerializeField]
    EnvBlock env_block_prefab;

    [SerializeField]
    float _destroy_block_distance, env_block_width;

    [SerializeField]
    float _ground_y, _fall_speed;
    public float ground_y => _ground_y;
    public float fall_speed => _fall_speed;


    public float destroy_block_distance => _destroy_block_distance;

    public int env_layer_mask { get; protected set; }
    public override void GameStartInitialize()
    {
        base.GameStartInitialize();
        env_layer_mask = LayerMask.GetMask(new string[] { "Environment" });

    }
    [System.NonSerialized]
    int level = 4;
    public string level_file => "level_" + level.ToString();
    public void TriggerStartBlock()
    {
        BlockPreset bp = Entity.Create<BlockPreset>(Setup.GetSetup(level_file).GetChainedNode<YamlMappingNode>("level:start"));
        ProgressEnvironment(bp);
    }

    [System.NonSerialized]
    int _block_number = 0;
    int block_number
    {
        get => _block_number;
        set
        {
            _block_number = value;
            Debug.Log("set bn to " + value);
        }
    }
    [System.NonSerialized]
    bool triggered = false;
    [SerializeField]
    float camera_center_plus_block_trigger = 20f;
    public override void Update()
    {
        base.Update();
        if(LevelGM.player.transform.position.x > LevelGM.cam_bounds.center.x + camera_center_plus_block_trigger && !triggered)
        {
            triggered = true;
            ProgressEnvironment();
        }
    }
    [SerializeField]
    int dev_start_block_number = 0;
    public void RestartLevel()
    {
        block_number = dev_start_block_number;
        triggered = false;
        Debug.Log("restarted level");

        Forest f = FindObjectOfType<Forest>();
        EnvSetter es = FindObjectOfType<EnvSetter>();

        f.Initialize();
        es.InitEnv();

    }
    public EnvPreset current_level_env_preset => env_presets.Find(ep=>ep.level == level);
    public void ProgressEnvironment(BlockPreset bp = null)
    {
        StartCoroutine(ProgressEnvironmentStep(bp), progenv_routine);
    }
    [SerializeField]
    GroundItem ground_item_prefab;
    IEnumerator ProgressEnvironmentStep(BlockPreset set_bp = null)
    {
        BlockPreset bp = set_bp == null ? SC.enemies.GetBlockPresetByNumber(block_number) : set_bp;

        SC.ui.RunDialogue(bp.dialogue_lines);
        SC.controls.active = false;
        while (SC.ui.running_dialogue)
        {
            yield return null;
        }
        SC.controls.active = true;
        yield return new WaitForSeconds(.5f);

        if (bp.item != null)
        {
            GroundItem gi = Instantiate(ground_item_prefab);
            gi.Initialize(bp.item);
            while(FindObjectOfType<GroundItem>() != null)
            {
                yield return null;
            }
        }

        SC.enemies.SpawnBlockEnemies(bp, GM.player.transform.position);
        

        LevelGM.UpdateCameraConfines(true);

        yield return WaitForBlockFinish(set_bp == null); 
    }
    public void StopProgenv()
    {
        StopCoroutine(progenv_routine);
        SC.enemies.StopSpawnblock();
    }
    const string progenv_routine = "wfb_envman";
    IEnumerator WaitForBlockFinish(bool advance = true)
    {
        while (SC.enemies.in_combat)
        {
            yield return null;
        }
        
        LevelGM.UpdateCameraConfines();
        if (advance)
        {
            block_number++;
            triggered = false;
        }

        if (SC.enemies.GetBlockPresetByNumber(block_number) == null)
        {
            level++;
            SC.game.LoadScene("Level");
         
        }

    }


    

}
