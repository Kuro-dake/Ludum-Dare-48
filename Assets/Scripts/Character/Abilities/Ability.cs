using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Ability : MonoBehaviour
{

    float cooldown;
    float current_cooldown;
    
    public bool ready { get => current_cooldown <= 0f; }

    public void Cast()
    {
        SC.routines.StartCoroutine(CastStep());
    }
    protected abstract IEnumerator CastStep();

    private void Update()
    {
        current_cooldown -= Time.deltaTime;
    }


}
