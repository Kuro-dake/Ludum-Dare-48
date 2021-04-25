using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    [SerializeField]
    protected FloatRange pursuit_delay;

    [SerializeField]
    float attack_delay = 1f, attack_range = 1f;
    [SerializeField]
    float next_attack = 0f;

    protected Character target => GM.player;

    protected abstract IEnumerator Pursue();

    static List<Enemy> _all_enemies = new List<Enemy>();
    public static List<Enemy> all_enemies
    {
        get
        {
            _all_enemies.RemoveAll(e => e == null || !e.is_alive);
            return new List<Enemy>(_all_enemies);
        }
    }
    public virtual void Initialize(EnemyPreset ep)
    {
        hp_max = ep.hp;
        attack = ep.attack;
        attack_delay = ep.attack_delay;
        attack_range = ep.attack_range;

        pursuit_delay = ep.pursuit_delay;

        transform.localScale = Vector3.one * ep.scale_range;

        speed = ep.move_speed;

        Initialize();
    }
    public override void Initialize()
    {
        base.Initialize();
        StartPursuit();
        _all_enemies.Add(this);
    }
    public string pursuit_routine => "pursuit_" + GetHashCode();
    void StartPursuit()
    {
        SC.routines.StartCoroutine(Pursue(), pursuit_routine);
    }

    public override void Stun(float duration)
    {
        base.Stun(duration);
        StartCoroutine(RestartPursuitStun());
    }
    void StopPursuit()
    {
        SC.routines.StopCoroutine(pursuit_routine);
    }
    IEnumerator RestartPursuitStun()
    {
        StopPursuit();
        while(stunned)
        {
            yield return null;
        }
        StartPursuit();
    }

    protected override void Update()
    {
        base.Update();
        List<Character> chars = new List<Collider2D>(Physics2D.OverlapCircleAll(transform.position, attack_range)).ConvertAll(c2d=>c2d.GetComponent<Character>());

        if (chars.Contains(target))
        {
            if (next_attack < 0f)
            {
                next_attack = attack_delay;
                target.ReceiveAttack(AttackData.Create(attack, "hit", this));
                anim.SetTrigger("attack");
            }
            
        }
        if (!stunned)
        {
            next_attack -= Time.deltaTime;
        }
        
    }
    protected override void Clear()
    {
        base.Clear();
        StopPursuit();
    }
    protected override void Die()
    {
        base.Die();
        StopPursuit();

    }
}
