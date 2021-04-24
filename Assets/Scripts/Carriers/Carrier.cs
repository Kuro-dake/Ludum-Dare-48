using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrier : MonoBehaviour, PoolableInterface
{
    Vector2 target, orig_pos;
    [SerializeField]
    float speed = 20f, affect_every_seconds = .3f, impact_lifetime = 1f;
    float lifetime;
    public bool is_active { get => gameObject.activeSelf; protected set => gameObject.SetActive(value); }
    bool arrived = false;
    // Update is called once per frame
    [SerializeField]
    impact_type impact = impact_type.on_hit;

    void Update()
    {
        switch (impact)
        {
            case impact_type.explode:
                transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * speed);
                if (Vector2.Distance(transform.position, target) < .1f && !arrived)
                {
                    Arrived();
                    arrived = true;
                }
                break;
            case impact_type.on_hit:
                
                Vector2 direction = (target - orig_pos).normalized * Time.deltaTime * speed;
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, Time.deltaTime * speed);
                foreach(RaycastHit2D hit in hits)
                {
                    Character c = hit.collider.GetComponent<Character>();
                    if (c != null && (attack_data.origin is Player && c != attack_data.origin || c is Player && attack_data.origin != GM.player) )
                    {
                        Affect(c);
                        is_active = false;
                        return;
                    }
                }
                transform.position += (direction).Vector3();
                lifetime -= Time.deltaTime;
                if(lifetime <= 0f)
                {
                    is_active = false;
                }
                break;
    }
        
    }
    [SerializeField]
    bool affect_once = true;
    List<Character> affected = new List<Character>();
    Dictionary<Character, float> affected_at = new Dictionary<Character, float>();
    protected virtual void Affect(Character c)
    {
        Debug.Log("affecting" + c);
        if(affected.Contains(c) && affect_once)
        {
            return;
        }
        if (!affected.Contains(c))
        {
            affected.Add(c);
        }
        if (!affect_once) {

            if (!affected_at.ContainsKey(c))
            {
                affected_at.Add(c, 0f);
            }
            
            if(Time.time - affected_at[c] > affect_every_seconds)
            {

                ApplyEffect(c);
                affected_at[c] = Time.time;
            }

        }
        else
        {
            ApplyEffect(c);
        }

    }
    AttackData attack_data;
    protected virtual void ApplyEffect(Character c)
    {
        Debug.Log("affecting");
        c.ReceiveAttack(attack_data);
    }

    [SerializeField]
    float explosion_scale = 3f, explosion_speed = 1f;
    protected void Arrived()
    {
        StartCoroutine(ExplodeStep());
    }



    IEnumerator ExplodeStep()
    {
        while (transform.localScale.x < explosion_scale)
        {
            transform.localScale = Vector3.one * Mathf.MoveTowards(transform.localScale.x, explosion_scale, Time.deltaTime * explosion_speed);
            /*Debug.Log(Character.all_characters.Count + " chars");
            Debug.Log(Character.all_characters.FindAll(c => Vector2.Distance(transform.position, c.transform.position) < explosion_scale).Count + " in range");*/
            Character.all_characters.FindAll(c => Vector2.Distance(transform.position, c.transform.position) < explosion_scale)
                .ForEach(c => Affect(c));
            yield return null;
        }
        gameObject.SetActive(false);
    }


    static int unid = 1;
    int id;
    public Carrier()
    {
        id = unid++;
    }
    Vector3 orig_scale;
    void Awake()
    {
        orig_scale = transform.localScale;
    }    
    public static Carrier Create(Vector3 origin, AttackData attack_data, Vector2 target, string type = "bullet")
    {
        List<Carrier> ret = new List<Carrier>();

        Carrier carrier = SC.pool.carriers.GetPooledObjectFromPrefab(type);
        carrier.orig_pos = origin;
        carrier.lifetime = carrier.impact_lifetime;
        carrier.affected.Clear();
        carrier.transform.position = origin;
        carrier.attack_data = attack_data;
        carrier.transform.localScale = carrier.orig_scale;
        carrier.is_active = true;
        carrier.target = target;
        carrier.arrived = false;
        /// this part is dev until I create pooled prefabs
        SpriteRenderer sr = carrier.GetComponent<SpriteRenderer>() != null ? carrier.GetComponent<SpriteRenderer>() : carrier.gameObject.AddComponent<SpriteRenderer>();

        sr.color = Color.white;

        sr.sortingLayerName = "AboveCharacters";
        /// endthispart

        return carrier;
    }

    public enum impact_type
    {
        on_hit,
        explode
    }

}


