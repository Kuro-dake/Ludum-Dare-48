using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityInventory : MonoBehaviour
{

    public void ResetAbilities()
    {
        abilities.ForEach(a => a.active = false);
        foreach(KeyValuePair<string, bool> kv in owned_abilities)
        {
            abilities.Find(a=>a.type == kv.Key).active = kv.Value;
        }
    }

    List<Ability> abilities => new List<Ability>(GetComponentsInChildren<Ability>());
    Dictionary<string, bool> owned_abilities => SC.player_stats.owned_abilities;
    public void NoCooldownsAllUnlock()
    {
        abilities.ForEach(a => a.active = true);
        abilities.ForEach(a => a.cooldown = 0f);
    }
    public Ability Activate(string ability)
    {
        System.Type t = System.Type.GetType(ability.UCFirst() + "Ability");
        

        Ability to_activate = abilities.Find(a => a.GetType() == t);
        Debug.Log(ability.UCFirst());
        Debug.Log(t);

        to_activate.active = true;
        
        if (!owned_abilities.ContainsKey(to_activate.type))
        {
            owned_abilities.Add(to_activate.type, true);
        }

        return to_activate;

    }
}
