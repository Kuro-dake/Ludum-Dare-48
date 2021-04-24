using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.RepresentationModel;
[CreateAssetMenu(fileName = "EnemyManager", menuName = "Service/EnemyManager", order = 0)]

public class EnemyManager : Service
{

    EntityList<BlockPreset> block_presets;

    public void Generate(string name, Vector2 position)
    {
        Enemy n_enemy = Instantiate(Resources.Load<Enemy>("Enemies/" + name.UCFirst()), service_transform);

        n_enemy.transform.position = position;

        n_enemy.Initialize();

    }

    public override void GameStartInitialize()
    {
        base.GameStartInitialize();
        block_presets = new EntityList<BlockPreset>(Setup.GetSetup("level_1").GetChainedNode<YamlSequenceNode>("level:blocks"));

        //block_presets.ForEach(b => b.enemies.ForEach(e => Debug.Log(e.type)));

    }
    public BlockPreset GetBlockPresetByNumber(int number) => block_presets[number];
    public bool SpawnBlockEnemies(int block_number)
    {
        if (block_presets.Count <= block_number || block_number < 0)
        {
            return false;
        }
        Debug.Log("spawned bn " + block_number);
        BlockPreset bp = GetBlockPresetByNumber(block_number);

        foreach(EnemyPreset ep in bp.enemies)
        {
            Generate(ep.type, GM.player.transform.position + ep.position.Vector3());
        }

        return true;

    }
}
