using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayerStatsManager", menuName = "Service/PlayerStatsManager", order = 0)]

public class PlayerStatsManager : Service
{
    [SerializeField]
    float _attack_delay;
    public float attack_delay => _attack_delay;

    [SerializeField]
    List<Ability> abilities;

    public Dictionary<string, bool> owned_abilities = new Dictionary<string, bool>();

    public override void GameStartInitialize()
    {
        base.GameStartInitialize();
        owned_abilities.Add(typeof(FlashAbility).ToString(), true);
        owned_abilities.Add(typeof(FireballAbility).ToString(), true);
        owned_abilities.Add(typeof(StunAbility).ToString(), true);

        //owned_abilities.Clear();
    }
}
