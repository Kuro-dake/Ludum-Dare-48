using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : Enemy
{
    [SerializeField]
    float attacks_per_second = 1f, attack_range = 1f;

    float next_attack = 0f;

    Character target => GM.player;

    private void Update()
    {
        if(Vector2.Distance(target.transform.position, transform.position) < attack_range)
        {
            if(next_attack < 0f) {
                next_attack = attacks_per_second;
                target.ReceiveAttack(AttackData.Create(attack, "hit"));
            }
        }
    }



}
