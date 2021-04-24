using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleGM : GM
{
    [SerializeField]
    List<ListGO> title_text_prefabs;
    List<GameObject> title_texts = new List<GameObject>();


    Queue<ListGO> title_text_prefabs_queue;

    protected override void Start()
    {
        base.Start();

        title_text_prefabs_queue = new Queue<ListGO>(title_text_prefabs);

        StartCoroutine(IntroProgression());
    }

    void ProgressIntro()
    {
        title_texts.ForEach(tt => Destroy(tt));
        title_texts.Clear();
        if (title_text_prefabs_queue.Count > 0) {
            title_text_prefabs_queue.Dequeue().list.ForEach(ttp => title_texts.Add(Instantiate(ttp, SC.ui.service_transform)));
        }
        UISortingOrder.Sort();
    }

    IEnumerator IntroProgression()
    {
        while(title_text_prefabs_queue.Count > 0)
        {
            SC.ui.FadeInOutCallback(ProgressIntro);
            yield return SC.controls.WaitForSpacebar();
            while (SC.routines.IsRunning(UIManager.ui_fioc_routine_name))
            {
                yield return null;
            }
            yield return null;
        }
        
        SC.game.LoadScene("Level", ProgressIntro);

    }

    private void Update()
    {
        
    }
    [System.Serializable]
    public class ListGO
    {
        public List<GameObject> list;
    }
}
