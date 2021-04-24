using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleGM : GM
{
    [SerializeField]
    List<GameObject> title_text_prefabs;
    List<GameObject> title_texts = new List<GameObject>();
    protected override void Start()
    {
        base.Start();
        title_text_prefabs.ForEach(tp => title_texts.Add(Instantiate(tp, SC.ui.service_transform)));
    }

    void ClearTitle()
    {
        title_texts.ForEach(tp => Destroy(tp));
    }

    IEnumerator IntroProgression()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SC.game.LoadScene("Level", ClearTitle);
        }
    }

}
