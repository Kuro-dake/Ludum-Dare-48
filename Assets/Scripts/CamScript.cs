using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, .1f, SC.env.env_layer_mask);
        foreach (Collider2D c in cols)
        {
            EnvBlock eb = c.GetComponent<EnvBlock>();
            if (eb != null)
            {
                eb.TriggerCollider(c);
                return;
            }
        }
    }
}
