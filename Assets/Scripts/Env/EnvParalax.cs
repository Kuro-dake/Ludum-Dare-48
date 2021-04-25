using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvParalax : MonoBehaviour
{

    float cam_x => Camera.main.transform.position.x;

    // Update is called once per frame
    [SerializeField]
    float x_multiplier = 1f;
    [SerializeField]
    bool motion_only = false;
    void LateUpdate()
    {
        if (!motion_only)
        {

            transforms.ForEach(delegate (Transform t)
            {
                if (cam_x - t.position.x < -adapt_to_width * .5f)
                {
                    t.position -= Vector3.right * modded_width * transforms.Count;
                }
                if (cam_x - t.position.x > adapt_to_width * .5f)
                {
                    t.position += Vector3.right * modded_width * transforms.Count;
                }
            });

        }
        transform.x(cam_x * (1f - x_multiplier));
    }

    List<Transform> transforms;

    private void Start()
    {
        Initialize();
    }
    float width = 0f;
    [SerializeField]
    float width_mod;
    float modded_width => width + width_mod;
    const float adapt_to_width = 60f;

    void Initialize()
    {
        transforms = new List<Transform>();
        if (motion_only)
        {
            return;
        }
        if(transform.childCount != 1)
        {
            throw new System.Exception("Initializing paralax with wrong number of children");
        }

        SpriteRenderer sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Bounds b = new Bounds();
        if (sr != null)
        {
            b.Encapsulate(sr.bounds);
        }
        else
        {
            for (int i = 0; i < transform.GetChild(0).transform.childCount; i++)
            {
                SpriteRenderer csr = transform.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>();
                if(csr == null)
                {
                    throw new System.Exception("Not sure what to do with this obj. " + csr.name);
                }
                b.Encapsulate(csr.bounds);
            }
        }

        width = b.size.x;

        transforms.Add(transform.GetChild(0).transform);


        int num_objects = Mathf.CeilToInt(adapt_to_width / modded_width);
        for(int i = 1; i< num_objects; i++)
        {
            transforms.Add(Instantiate(transform.GetChild(0).gameObject, transform).transform);
        }

        PlaceTransforms();


        /*for(int i = 0; i < transform.childCount; i++)
        {
            objects.Add(transform.GetChild(i).gameObject);
        }*/
    }
    void PlaceTransforms()
    {
        float start_x = modded_width * (transforms.Count - 1) * -.5f;
        for(int i = 0; i<transforms.Count; i++) {
            Vector3 locpos = transforms[i].position;
            locpos.x = start_x + i * width;
            locpos.y = 0f;
            transforms[i].localPosition = locpos;
        }
    }
}
