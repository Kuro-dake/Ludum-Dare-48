using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyGenerator", menuName = "Service/EnemyGenerator", order = 0)]

public class EnemyGenerator : Service
{
    [SerializeField]
    List<Pair<string, Enemy>> enemy_prefabs;
}
