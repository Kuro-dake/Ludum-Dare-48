﻿using System.Collections;
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

    public void RestartLevel()
    {
        block_number = 0;
        triggered = false;
        Debug.Log("restarted level");
    }

    public void ProgressEnvironment()
    {
        StartCoroutine(ProgressEnvironmentStep(), progenv_routine);
    }

    IEnumerator ProgressEnvironmentStep()
    {
        if (SC.enemies.GetBlockPresetByNumber(block_number) == null)
        {
            Debug.LogError("finish level");
            yield break;
        }
        
        SC.ui.RunDialogue(SC.enemies.GetBlockPresetByNumber(block_number).dialogue_lines);
        SC.controls.active = false;
        while (SC.ui.running_dialogue)
        {
            yield return null;
        }
        SC.controls.active = true;
        yield return new WaitForSeconds(.5f);
        SC.enemies.SpawnBlockEnemies(block_number);
        

        LevelGM.UpdateCameraConfines(true);

        yield return WaitForBlockFinish(); 
    }
    public void StopProgenv()
    {
        StopCoroutine(progenv_routine);
        SC.enemies.StopSpawnblock();
    }
    const string progenv_routine = "wfb_envman";
    IEnumerator WaitForBlockFinish()
    {
        while (SC.enemies.in_combat)
        {
            yield return null;
        }
        
        LevelGM.UpdateCameraConfines();

        block_number++;
        triggered = false;
    }


    

}
