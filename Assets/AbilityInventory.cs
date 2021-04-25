using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityInventory : MonoBehaviour
{
   public Ability Activate(string ability)
    {
        System.Type t = System.Type.GetType(ability.UCFirst() + "Ability");
        List<Ability> abilities = new List<Ability>(GetComponentsInChildren<Ability>());

        Ability to_activate = abilities.Find(a => a.GetType() == t);
        Debug.Log(ability.UCFirst());
        Debug.Log(t);

        to_activate.active = true;

        return to_activate;
    }
}
