using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemGenFreeze : MonoBehaviour
{
    ParticleSystem ps => GetComponent<ParticleSystem>();

    private void Start()
    {
        StartCoroutine(Apply());
    }

    IEnumerator Apply()
    {
        ParticleSystem.MainModule mm = ps.main;
        ParticleSystem.EmissionModule em = ps.emission;
        em.rate = mm.maxParticles * 100;
        
        while(ps.particleCount < mm.maxParticles)
        {
            yield return null;
        }
        mm.simulationSpeed = 0f;
    }
}
