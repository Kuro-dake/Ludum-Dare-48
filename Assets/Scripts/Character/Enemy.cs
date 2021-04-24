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
        StartCoroutine(Pursue());
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
        next_attack -= Time.deltaTime;
    }
}
