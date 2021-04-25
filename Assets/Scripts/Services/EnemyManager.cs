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
        
        enemy_presets = new EntityList<EnemyPreset>(Setup.GetSetup("enemy_presets").GetChainedNode<YamlSequenceNode>("enemy_presets"));

        //block_presets.ForEach(b => b.enemies.ForEach(e => Debug.Log(e.type)));
    }

    public void InitLevel()
    {
        block_presets = new EntityList<BlockPreset>(Setup.GetSetup(SC.env.level_file).GetChainedNode<YamlSequenceNode>("level:blocks"));
    }

    public BlockPreset GetBlockPresetByNumber(int number) => number < block_presets.Count ? block_presets[number] : null;
    const string spawnblock_routine = "sbes_routine";
    IEnumerator SpawnBlockEnemiesStep(BlockPreset bp, Vector3 center)
    {
        /*if (bp.enemies == null)
        {
            yield break;
        }*/
        in_combat = true;
        
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
        center.y = SC.env.ground_y;
        foreach(List<EnemyPositionPreset> ep_list in ens_list.ConvertAll(kv => kv.Value))
        {
            foreach(EnemyPositionPreset ep in ep_list)
            {
                Explosion e = SC.effects["explosion"] as Explosion;

                e.speed = 2f;

                e.color_1 = e.color_2 = Color.black;
                e.lifetime = .5f;
                e.Play(center + ep.position.Vector3() + Vector3.up * .7f);
                yield return new WaitForSeconds(.5f);

                Generate(ep, center + ep.position.Vector3());
                //yield return new WaitForSeconds(.3f);
            }
            while(Enemy.all_enemies.Count > 0)
            {
                yield return null;
            }
            
            yield return new WaitForSeconds(2f);
            center = GM.player.transform.position;
            center.y = SC.env.ground_y;
        }

        in_combat = false;

    }
    public void StopSpawnblock()
    {
        StopCoroutine(spawnblock_routine);
        in_combat = false;
    }
    public void SpawnBlockEnemies(BlockPreset bp, Vector3 center)
    {
        StartCoroutine(SpawnBlockEnemiesStep(bp, center), spawnblock_routine);

    }

    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Enemy.all_enemies.ForEach(e => e.ReceiveAttack(AttackData.Create(100, "hit", GM.player)));
        }
    }
    public bool in_combat { get; protected set; } = false;
}
