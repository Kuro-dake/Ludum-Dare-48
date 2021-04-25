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

    public virtual void Initialize()
    {
        hp = hp_max;
        _all_characters.Add(this);
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
        
        hp -= remnant;
        if (hp <= 0)
        {
            Die();
        }
    }
    public bool stunned => stun_duration > 0f;
    public bool airborne => transform.position.y > SC.env.ground_y;
    Animator anim => GetComponent<Animator>();
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
        Debug.Log(anim);
        if (anim != null)
        {
            anim.SetBool("walking", walking);
        }
        walking = false;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
    public bool is_alive => hp > 0;
    [SerializeField]
    protected movement_type move_type = movement_type.ground;

}

public enum movement_type
{
    ground,
    floating
}