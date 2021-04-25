using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Ability : MonoBehaviour
{
    [SerializeField]
    string _name = "";
    public string ability_name => _name != "" ? _name : GetType().ToString().Replace("Ability", "");

    [TextArea(10,10)]
    public string description;
    [SerializeField]
    public float cooldown = 5f;
    public float current_cooldown { get; protected set; }
    public bool active = false;
    public bool ready { get => current_cooldown <= 0f && active; }

    public void Cast()
    {
        if (ready)
        {
            current_cooldown = cooldown;
            SC.routines.StartCoroutine(CastStep());
        }
        
    }
    protected abstract IEnumerator CastStep();
    
    private void Update()
    {
        current_cooldown -= Time.deltaTime;
    }

    public string type => GetType().ToString();
}
