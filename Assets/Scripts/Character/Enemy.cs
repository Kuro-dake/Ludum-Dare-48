using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    [SerializeField]
    protected FloatRange pursuit_delay;

    [SerializeField]
    float attacks_per_second = 1f, attack_range = 1f;

    float next_attack = 0f;

    protected Character target => GM.player;

    protected abstract IEnumerator Pursue();

    public override void Initialize()
    {
        base.Initialize();
        StartPursuit();
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
        if (Vector2.Distance(target.transform.position, transform.position) < attack_range)
        {
            if (next_attack < 0f)
            {
                next_attack = attacks_per_second;
                target.ReceiveAttack(AttackData.Create(attack, "hit", this));
            }
            
        }
        if (!stunned)
        {
            next_attack -= Time.deltaTime;
        }
        
    }

    protected override void Die()
    {
        base.Die();
        StopPursuit();
    }
}
