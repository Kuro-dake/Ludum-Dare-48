using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.RepresentationModel;
using System.Linq;
[CreateAssetMenu(fileName = "EnemyManager", menuName = "Service/EnemyManager", order = 0)]

public class EnemyManager : Service
{

    EntityList<BlockPreset> block_presets;
    EntityList<EnemyPreset> enemy_presets;
    public void Generate(EnemyPositionPreset epp, Vector2 position)
    {
        
        Enemy n_enemy = Instantiate(Resources.Load<Enemy>("Enemies/" + epp.enemy_preset.type.UCFirst()), service_transform);

        n_enemy.transform.position = position;

        n_enemy.Initialize(epp.enemy_preset);

    }
    public EnemyPreset GetEnemyPresetById(string id) => enemy_presets.Find(ep => ep.id == id);
    public override void GameStartInitialize()
    {
        base.GameStartInitialize();
        block_presets = new EntityList<BlockPreset>(Setup.GetSetup("level_1").GetChainedNode<YamlSequenceNode>("level:blocks"));
        enemy_presets = new EntityList<EnemyPreset>(Setup.GetSetup("enemy_presets").GetChainedNode<YamlSequenceNode>("enemy_presets"));

        //block_presets.ForEach(b => b.enemies.ForEach(e => Debug.Log(e.type)));

    }
    public BlockPreset GetBlockPresetByNumber(int number) => number < block_presets.Count ? block_presets[number] : null;
    const string spawnblock_routine = "sbes_routine";
    IEnumerator SpawnBlockEnemiesStep(int block_number)
    {
        if (block_presets.Count <= block_number || block_number < 0)
        {
            yield break;
        }
        in_combat = true;
        Debug.Log("spawned bn " + block_number);
        BlockPreset bp = GetBlockPresetByNumber(block_number);

        Dictionary<int, List<EnemyPositionPreset>> ens_by_wave = new Dictionary<int, List<EnemyPositionPreset>>();

        foreach (EnemyPositionPreset ep in bp.enemies)
        {
            if (!ens_by_wave.ContainsKey(ep.wave))
            {
                ens_by_wave.Add(ep.wave, new List<EnemyPositionPreset>());
            }
            ens_by_wave[ep.wave].Add(ep);
            
        }
        
        List<KeyValuePair<int, List<EnemyPositionPreset>>> ens_list = ens_by_wave.ToList();
        ens_list.Sort((a, b) => a.Key.CompareTo(b.Key));

        foreach(List<EnemyPositionPreset> ep_list in ens_list.ConvertAll(kv => kv.Value))
        {
            foreach(EnemyPositionPreset ep in ep_list)
            {
                Generate(ep, GM.player.transform.position + ep.position.Vector3());
                yield return new WaitForSeconds(.3f);
            }
            while(Enemy.all_enemies.Count > 0)
            {
                yield return null;
            }
            yield return new WaitForSeconds(2f);
        }

        in_combat = false;

    }
    public void StopSpawnblock()
    {
        StopCoroutine(spawnblock_routine);
        in_combat = false;
    }
    public void SpawnBlockEnemies(int block_number)
    {
        StartCoroutine(SpawnBlockEnemiesStep(block_number), spawnblock_routine);

    }

    public bool in_combat { get; protected set; } = false;
}
