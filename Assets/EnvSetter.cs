using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvSetter : MonoBehaviour
{
    public void InitEnv()
    {
        List<EnvParalax> pars = new List<EnvParalax>(GetComponentsInChildren<EnvParalax>());

        EnvManager.EnvPreset ep = SC.env.current_level_env_preset;

        foreach (EnvParalax p in pars)
        {
            if (p.name.Contains("grass"))
            {
                SetPSColors(p, ep.grass);
            }
            if (p.name.Contains("clouds"))
            {
                SetPSColors(p, ep.clouds);
            }
            if (p.name.Contains("bushes"))
            {
                SetPSColors(p, ep.bush);
            }
            if (p.name.Contains("ground"))
            {
                SetSRColor(p, ep.ground);
            }
            if (p.name.Contains("mountains"))
            {
                SetSRColorMulti(p, ep.mountain);
            }
        }
        pars.ForEach(p => p.Initialize());
    }

    public static void SetPSColors(EnvParalax p, Color[] colors)
    {
        ParticleSystem ps = p.transform.GetChild(0).GetComponentInChildren<ParticleSystem>();
        ParticleSystem.MainModule mm = ps.main;
        mm.startColor = new ParticleSystem.MinMaxGradient(colors[0], colors[1]);
    }
    public static void SetSRColor(EnvParalax p, Color color)
    {
        SpriteRenderer sr = p.transform.GetChild(0).GetComponent<SpriteRenderer>();
        sr.color = color;
    }
    static void SetSRColorMulti(EnvParalax p, Color[] c)
    {
        new List<SpriteRenderer>(p.transform.GetChild(0).GetComponentsInChildren<SpriteRenderer>()).ForEach(sr => sr.color = Color.Lerp(c[0],c[1], Random.Range(0f,1f)));
    }
}
