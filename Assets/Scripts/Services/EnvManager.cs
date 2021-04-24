using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvManager", menuName = "Service/EnvManager", order = 0)]

public class EnvManager : Service
{
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

    public EnvBlock CreateAdjecentEnvBlock(EnvBlock e, bool left)
    {
        EnvBlock neb = Instantiate(env_block_prefab, e.transform.position + env_block_width * (left ? -1 : 1) * Vector3.right, Quaternion.identity);
        neb.Initialize(e);

        SC.enemies.SpawnBlockEnemies(neb.block_number);

        return neb;
    }

}
