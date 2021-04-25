using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

[CreateAssetMenu(fileName = "GameManager", menuName = "Service/GameManager", order = 0)]

public class GameManager : Service
{
    [SerializeField]
    List<NamedSprite> named_sprites;
    public Sprite this[string s]
    {
        get
        {
            return named_sprites.Find(ns => ns.first == s)?.second;
        }
    }
    public CinemachineVirtualCamera cine_cam => FindObjectOfType<CinemachineVirtualCamera>();

    public void Test()
    {
        Debug.Log("working");
        
    }
    [System.NonSerialized]
    string load_scene_name;
    System.Action load_scene_callback;
    void LoadSceneCallback()
    {
        load_scene_callback?.Invoke();
        SceneManager.LoadScene(load_scene_name);
    }
    public void LoadScene(string scene, System.Action callback = null)
    {
        load_scene_callback = callback;
        load_scene_name = scene;

        SC.ui.FadeInOutCallback(LoadSceneCallback);
    }

}
