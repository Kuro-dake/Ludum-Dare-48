using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItem : MonoBehaviour
{


    List<string> dialogue;
    string effect;
    public void Initialize(ItemPreset ip)
    {
        dialogue = ip.dialogue_lines;
        effect = ip.effect;
        Vector3 pos = GM.player.transform.position;
        pos.z = 0f;
        pos.x += 16f;
        pos.y = SC.env.ground_y;
        transform.position = pos;
    }
    bool interacted = false;
    public void Interact()
    {
        if (interacted)
        {
            return;
        }
        interacted = true;
        StartCoroutine(InteractStep());
    }

    IEnumerator InteractStep()
    {
        SC.controls.active = false;
        SC.ui.RunDialogue(dialogue);
        while (SC.ui.running_dialogue)
        {
            yield return null;
        }
        TriggerEffect();
        yield return Fade(1f, 0f);
        
        if(result_dialogue.Count > 0)
        {
            SC.ui.RunDialogue(result_dialogue);
            while (SC.ui.running_dialogue)
            {
                Debug.Log("still running");
                yield return null;
            }
        }
        SC.controls.active = true;
        Destroy(gameObject);

    }
    List<string> result_dialogue = new List<string>();
    void TriggerEffect()
    {
        
        string ability_prefix = "ability_";
        if (effect.Substring(0, Mathf.Clamp(ability_prefix.Length, 0, effect.Length)) == ability_prefix)
        {
            string aname = effect.Replace(ability_prefix, "");
            Ability a = FindObjectOfType<AbilityInventory>().Activate(aname);
            AbilitiesBlock ab = FindObjectOfType<AbilitiesBlock>();
            
            result_dialogue.Add("You have gained the " + a.ability_name + " ability.");
            if(a.description.Length > 0)
            {
                result_dialogue.Add(a.description);
            }
            
            result_dialogue.Add("Press " + ab.GetUIButtonAction(a).shortcut.KeyShortcut() + " to use.");

        }

        switch (effect)
        {
            case "no_cooldowns":
                FindObjectOfType<AbilityInventory>().NoCooldownsAllUnlock();
                break;
        }
    }

    private void Update()
    {
        if(Mathf.Abs(GM.player.transform.position.x - transform.position.x) < 1f)
        {
            Interact();
        }
    }

    IEnumerator Fade(float current, float to, float speed = 1f)
    {
        
        List<SpriteRenderer> srs = new List<SpriteRenderer>() { GetComponentInChildren<SpriteRenderer>() };

        while (current != to)
        {
            current = Mathf.MoveTowards(current, to, Time.deltaTime * speed);
            srs.ForEach(delegate (SpriteRenderer sr)
            {
                Color c = sr.color;
                c.a = current;
                sr.color = c;
            });
            yield return null;
        }
    }
}
