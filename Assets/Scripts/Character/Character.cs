using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    protected float speed = 5f;
    
    [SerializeField]
    protected int hp_max, hp, attack = 1;

    protected int shield = 0;

    public void GainShield(int value)
    {
        shield = shield > value ? shield : value;
    }

    float stun_duration = 0f;

    public virtual void Stun(float duration)
    {
        stun_duration = stun_duration > duration ? stun_duration : duration;
    }

    static List<Character> _all_characters = new List<Character>();
    string shake_routine => "shake_" + GetHashCode();
    public void Shake()
    {
        SC.routines.StartCoroutine(ShakeRoutine(.5f), shake_routine);
    }
    IEnumerator ShakeRoutine(float duration)
    {
        while(duration > 0f)
        {
            shake_parent.transform.localPosition = shake_parent_orig_pos + Random.insideUnitCircle * duration * 1f; 
            duration -= Time.deltaTime;
            yield return null;
        }
    }
    public static List<Character> all_characters
    {
        get
        {
            _all_characters.RemoveAll(c => c == null || !c.is_alive);
            return new List<Character>(_all_characters);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    Vector2 shake_parent_orig_pos;
    public virtual void Initialize()
    {
        hp = hp_max;
        _all_characters.Add(this);
        anim.SetBool("floater", move_type == movement_type.floating);
        shake_parent_orig_pos = shake_parent.localPosition;

        StartCoroutine(Fade(0f, 1f, 4f));
    }
    public string char_movement_routine_name => "char_movement_" + GetHashCode();
    public void MoveTo(Vector2 to)
    {
        SC.routines.StartCoroutine(MoveToStep(to), char_movement_routine_name);
    }

    IEnumerator MoveToStep(Vector2 to)
    {
        while(this != null && (!airborne || move_type == movement_type.floating) && !stunned && Vector2.Distance(transform.position, to) > 0f)
        {
            orientation_left = transform.position.x > to.x;
            transform.position = Vector3.MoveTowards(transform.position, to, Time.deltaTime * speed);
            walking = true;
            
            yield return null;
        }
    }

    public void ReceiveAttack(AttackData ad)
    {

        int remnant = ad.damage;
        if(shield > 0)
        {
            shield -= remnant;
            if(shield < 0)
            {
                remnant = Mathf.Abs(shield);
            }
            else
            {
                remnant = 0;
            }
            
        }
        Explosion e = SC.effects["hit"] as Explosion;
        
        
        if (remnant > 0)
        {
            Shake();
        }
        else
        {
            e.color_1 = e.color_2 = Color.blue;
        }

        e.Play(ad.impact_point == default ? transform.position + Vector3.up * 1f : ad.impact_point);

        hp -= remnant;
        if (hp <= 0)
        {
            Die();
        }
    }
    public bool stunned => stun_duration > 0f;
    public bool airborne => transform.position.y > SC.env.ground_y;
    public Animator anim => GetComponent<Animator>();
    protected bool walking = false;
    protected Transform shake_parent => transform.Find("shake_parent");
    protected bool orientation_left
    {
        set
        {
            if(shake_parent == null)
            {
                return;
            }
            shake_parent.localScale = new Vector3(value ? -1f : 1f, 1f, 1f);
            return;
            new List<SpriteRenderer>(shake_parent?.GetComponentInChildren<StaticPSDAnimation>().GetComponentsInChildren<SpriteRenderer>(true)).
                ForEach(sr => sr.flipX = value);
        }
    }
    protected virtual void Update()
    {
        if (!is_alive)
        {
            return;
        }
        if(move_type == movement_type.ground)
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.MoveTowards(transform.position.y, SC.env.ground_y, Time.deltaTime * SC.env.fall_speed);
            transform.position = pos;
        }
        stun_duration -= Time.deltaTime;
        
    }

    protected void LateUpdate()
    {

        if (anim != null)
        {
            anim.SetBool("walking", walking);
        }
        walking = false;
    }
    protected void OnDestroy()
    {
        Clear();
    }
    protected virtual void Die()
    {
        Clear();
        StartCoroutine(DieStep());
    }
    protected IEnumerator DieStep()
    {

        Shake();

        yield return Fade(1f, 0f, 10f);

        Destroy(gameObject);
    }

    IEnumerator Fade(float current, float to, float speed = 1f)
    {
        StaticPSDAnimation spsda = shake_parent.Find("anim_parent").Find("player").GetComponentInChildren<StaticPSDAnimation>();
        List<SpriteRenderer> srs = new List<SpriteRenderer>(spsda.GetComponentsInChildren<SpriteRenderer>(true));

        while (current != to)
        {
            current = Mathf.MoveTowards(current, to, Time.deltaTime * speed);
            srs.ForEach(delegate (SpriteRenderer sr)
            {
                Color c = sr.color;
                c.a = current;
                sr.color = c;
            });
            yield return null;
        }
    }

    protected virtual void Clear()
    {
        SC.routines.StopCoroutine(shake_routine);
        SC.routines.StopCoroutine(char_movement_routine_name);
    }
    public bool is_alive => hp > 0;
    
    protected virtual movement_type move_type => movement_type.ground;

}

public enum movement_type
{
    ground,
    floating
}