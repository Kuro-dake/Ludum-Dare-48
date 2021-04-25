using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
public class Carrier : MonoBehaviour, PoolableInterface
{
    Vector2 target, orig_pos;
    [SerializeField]
    float speed = 20f, affect_every_seconds = .3f, impact_lifetime = 1f;
    float lifetime;
    public bool is_active
    {
        get => gameObject.activeSelf;
        protected set
        {
            gameObject.SetActive(value);
            if (!value)
            {
                DetachVisuals();
            }
        }
    }
    bool arrived = false;
    // Update is called once per frame
    [SerializeField]
    impact_type impact = impact_type.on_hit;
    public bool piercing = false;
    TrailRenderer tr => GetComponent<TrailRenderer>();
    IEnumerator DelayedTRDeactivation()
    {
        if(tr != null)
        {
            yield return new WaitForSeconds(tr.time);
            yield return null;
        }
        
        is_active = false;
    }
    [SerializeField]
    float noise;
    void Update()
    {
        if(lifetime <= 0f)
        {
            return;
        }
        switch (impact)
        {
            case impact_type.explode:
                if (arrived)
                {
                    break;
                }
                transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * speed) + (Random.insideUnitCircle * noise);
                if (Vector2.Distance(transform.position, target) < .1f && !arrived)
                {
                    Arrived();
                    arrived = true;
                }
                break;
            case impact_type.on_hit:

                Vector2 direction = (target - orig_pos).normalized * Time.deltaTime * speed + (Random.insideUnitCircle * noise);
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, Time.deltaTime * speed);
                foreach(RaycastHit2D hit in hits)
                {
                    Character c = hit.collider.GetComponent<Character>();
                    if (c != null && c.is_alive && (attack_data.origin is Player && c != attack_data.origin || c is Player && attack_data.origin != GM.player) )
                    {
                        attack_data.impact_point = hit.point;
                        Affect(c);
                        if (!piercing) {

                            is_active = false;
                            return;
                        }
                        
                    }
                }
                transform.position += (direction).Vector3();
                lifetime -= Time.deltaTime;
                if (lifetime <= 0f)
                {
                    StartCoroutine(DelayedTRDeactivation());
                    
                }
                break;
        }
        
    }
    void DetachVisuals()
    {
        if(visuals == null)
        {
            return;
        }
        ParticleSystem ps = visuals.GetComponent<ParticleSystem>();
        visuals.SetParent(null);

        if(ps != null)
        {
            ps.Stop();
        }
    }
    [SerializeField]
    bool affect_once = true;
    List<Character> affected = new List<Character>();
    Dictionary<Character, float> affected_at = new Dictionary<Character, float>();
    [SerializeField]
    bool stun = false;
    Transform visuals => transform.Find("Visual");
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
        if (stun)
        {
            c.Stun(1f);
        }
    }

    [SerializeField]
    float explosion_scale = 3f, explosion_speed = 1f;
    protected void Arrived()
    {
        DetachVisuals();
        StartCoroutine(ExplodeStep());
    }

    [SerializeField]
    Color explosion_color, explosion_color_2;

    IEnumerator ExplodeStep()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color mid_color = Color.Lerp(explosion_color, explosion_color_2, .5f);
        if (sr != null)
        {
            sr.sprite = SC.game["ring"];
            sr.color = mid_color * 1.4f - Color.black * .2f;
            sr.enabled = true;
        }
        Effect e = SC.effects["explosion"];
        (e as Explosion).color_1 = explosion_color;
        (e as Explosion).color_2 = explosion_color_2;
        e.Play(transform.position);
        float sr_color_speed = 1f * (explosion_speed / explosion_scale) * 1.2f;

        Light2D l2d = GetComponent<Light2D>();
        if(l2d != null)
        {
            l2d.color = mid_color;
        }
        LightGlow lg = GetComponent<LightGlow>();

        while (transform.localScale.x < explosion_scale)
        {
            transform.localScale = Vector3.one * Mathf.MoveTowards(transform.localScale.x, explosion_scale, Time.deltaTime * explosion_speed);
            /*Debug.Log(Character.all_characters.Count + " chars");
            Debug.Log(Character.all_characters.FindAll(c => Vector2.Distance(transform.position, c.transform.position) < explosion_scale).Count + " in range");*/

            List<Collider2D> c2ds = new List<Collider2D>(Physics2D.OverlapCircleAll(transform.position, transform.localScale.x * .5f));

            List<Character> cs = c2ds.ConvertAll(c2d => c2d.GetComponent<Character>());
            cs.RemoveAll(c => c == null);
            cs.ForEach(c => Affect(c));

            /*Character.all_characters.FindAll(c => Vector2.Distance(transform.position, c.transform.position) < transform.localScale.x * .5f)
                .ForEach(c => Affect(c));*/
            Debug.DrawRay(transform.position, Vector2.right * transform.localScale.x * .5f, Color.red);

            e.transform.localScale = transform.localScale * .3f;

            sr.color -= Color.black * Time.deltaTime * sr_color_speed;
            
            if(lg != null)
            {
                lg.target_radius_range = new FloatRange(transform.localScale.x * 2f, transform.localScale.x * 2.4f);
            }

            yield return null;
        }
        e.Stop();
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

        Carrier carrier = SC.pool.carriers.GetPooledObjectFromPrefab(type, true);
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

        //sr.sortingLayerName = "AboveCharacters";
        /// endthispart

        return carrier;
    }

    public enum impact_type
    {
        on_hit,
        explode
    }

}


