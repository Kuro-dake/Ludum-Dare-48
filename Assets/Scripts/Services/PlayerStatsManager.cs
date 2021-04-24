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

}
