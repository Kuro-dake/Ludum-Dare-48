using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : PlayParticleSystemEffect
{

    public Color color_1, color_2;
    
    public override void Play(Vector2 position)
    {
        ParticleSystem.MainModule mm = ps.main;
        mm.startColor = new ParticleSystem.MinMaxGradient(color_1, color_2);
        base.Play(position);
    }
}
