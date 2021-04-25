﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : MonoBehaviour
{
    [SerializeField]
    EnvTree tree_prefab;

    float last_x_generated;
    float cam_x => Camera.main.transform.position.x;
    private void Start()
    {
        last_x_generated = cam_x;
    }
    //[SerializeField]
    float density => /*(1000f - cam_x) / 100f*/6f;
    [SerializeField]
    Transform forest_screen;
    [SerializeField]
    float size_divider = 50f;
    private void Update()
    {
        if(Mathf.Abs(last_x_generated - cam_x) > density)
        {


            EnvTree et = Instantiate(tree_prefab);

            Vector3 pos = Vector3.zero;


            pos.x = cam_x + (EnvTree.destroy_distance - 10f) * (cam_x > last_x_generated ? 1 : -1);

            et.transform.position = pos;

            et.transform.SetParent(transform);

            pos = et.transform.localPosition;
            pos.y = Random.Range(-1f,1f);

            et.transform.localPosition = pos;

            et.transform.localScale = Vector3.one * Random.Range(.6f, .8f) * cam_multiplier;
            et.transform.localRotation = Quaternion.Euler(Vector3.back * Random.Range(-15f, 15f));
            last_x_generated = cam_x;

            et.Initialize();
        }
        forest_screen.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.clear, Color.black, (cam_multiplier - cm_min) / cm_diff);
    }

    float cm_min = .3f;
    float cm_max = 3f;
    float cm_diff => cm_max - cm_min;

    float cam_multiplier => Mathf.Clamp(cam_x / size_divider, cm_min, cm_max);

}
