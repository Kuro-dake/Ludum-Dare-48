using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIManager", menuName = "Service/UIManager", order = 0)]

public class UIManager : Service
{
    [SerializeField]
    List<GameCursor> cursor_prefabs;
    [System.NonSerialized]
    List<GameCursor> cursors = new List<GameCursor>();

    public Canvas canvas => service_transform.GetComponent<Canvas>();

    public override void GameStartInitialize()
    {
        base.GameStartInitialize();
        DontDestroyOnLoad(service_transform);
        cursor_prefabs.ForEach(cp => cursors.Add(Instantiate(cp, service_transform)));
    }

    public void SwitchCursor(cursor_type type)
    {
        cursors.ForEach(c => c.gameObject.SetActive(false));
        GameCursor cursor = null;
        switch (type)
        {
            
            case cursor_type.ground:
                cursor = cursors.Find(c => c is GroundLevelCursor);
                break;
            case cursor_type.star:
                cursor = cursors.Find(c => c is StarLevelCursor);
                break;
        }
        cursor?.gameObject.SetActive(true);
    }

    public enum cursor_type
    {
        ground,
        star
    }
}

