using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{

    static GM inst;
    protected Player _player;
    public static Player player => inst._player;

    protected virtual void Start()
    {
        if(FindObjectsOfType<GM>().Length != 1)
        {
            throw new System.Exception("There has to be exactly one object of GM type");
        }
        inst = this;
        SC.Initialize();
    }

    
}
