using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleSystemEffect : Effect
{
    public float speed
    {
        set
        {
            ParticleSystem.MainModule mm = ps.main;

            mm.simulationSpeed = value;
        }
    }
    public float lifetime
    {
        set
        {
            StartCoroutine(Expire(value));
        }
    }
    IEnumerator Expire(float time)
    {
        yield return new WaitForSeconds(time);
        Stop();
    }

    public override bool is_playing
    {
        get
        {
            return ps.isPlaying;
        }
    }
    public ParticleSystem ps { get { return GetComponent<ParticleSystem>(); } }
    public float original_scale = 1f;
    public override void Play(Vector2 position)
    {
        base.Play(position);
        ps.Play();
        if (destroy_on_not_playing)
        {
            StartCoroutine(WaitToStop());
        }
        speed = 1f;
        transform.localScale = Vector3.one * original_scale;
    }

    public override void Stop()
    {
        base.Stop();
        ps.Stop();
    }

    [SerializeField]
    bool destroy_on_not_playing = false;

    IEnumerator WaitToStop()
    {
        while (is_playing)
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}
