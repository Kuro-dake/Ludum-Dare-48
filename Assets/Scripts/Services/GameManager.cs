using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

[CreateAssetMenu(fileName = "GameManager", menuName = "Service/GameManager", order = 0)]

public class GameManager : Service
{
 
    public CinemachineVirtualCamera cine_cam => FindObjectOfType<CinemachineVirtualCamera>();

    public void Test()
    {
        Debug.Log("working");
        
    }

    public void LoadScene(string scene, System.Action callback = null)
    {
        SC.routines.StartCoroutine(LoadSceneStep(scene, callback));
    }

    IEnumerator LoadSceneStep(string scene, System.Action callback = null)
    {
        SC.ui.curtain = true;
        while (!SC.ui.curtain_visible)
        {
            yield return null;
        }
        callback?.Invoke();

        SceneManager.LoadScene(scene);

        SC.ui.curtain = false;
    }
}
