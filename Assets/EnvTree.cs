using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvTree : MonoBehaviour
{

    float cam_x => Camera.main.transform.position.x;

    public const float destroy_distance = 40f;
    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(transform.position.x - cam_x) > destroy_distance)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = Mathf.RoundToInt(transform.position.x);

        sr.flipX = Random.Range(0, 2) == 1;

        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        ps.GetComponent<ParticleSystemRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.x) + 1;
    }
}
