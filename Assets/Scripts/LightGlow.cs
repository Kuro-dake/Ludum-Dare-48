using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightGlow : MonoBehaviour
{
    void Start()
    {

        source.color = GetRandomColorFromParentParticleSystem();
    }
    [SerializeField]
    public FloatRange target_intensity_range = null, target_radius_range = null, duration_range = null;
    Light2D source { get { return GetComponent<Light2D>(); } }
    Color GetRandomColorFromParentParticleSystem()
    {
        if (GetComponentInParent<ParticleSystem>() == null)
        {
            return source.color;
        }
        Color ret = Color.Lerp(
                GetComponentInParent<ParticleSystem>().main.startColor.colorMin,
                GetComponentInParent<ParticleSystem>().main.startColor.colorMax,
                Random.Range(0f, 1f)
                );
        ret.a = 1f;
        return ret;
    }
    public void OnEnable()
    {
        StartCoroutine(Glow());
    }
    public float radius_multiplier = 1f;
    IEnumerator Glow()
    {
        while (true)
        {
            float duration_inverse = 1f / duration_range;
            float current = 0f;

            float orig_intensity = source.intensity;
            float orig_radius = source.pointLightOuterRadius;

            float target_intensity = target_intensity_range;
            float target_radius = target_radius_range;

            /*Color orig_color = source.color;
            Color target_color = GetRandomColorFromParentParticleSystem();*/

            while (current < 1f)
            {
                current += Time.deltaTime * duration_inverse;

                source.intensity = Mathf.Lerp(orig_intensity, target_intensity, current);
                source.pointLightOuterRadius = Mathf.Lerp(orig_radius, target_radius, current) * radius_multiplier;

                //source.color = Color.Lerp(orig_color, target_color, current);

                yield return null;
            }
        }
    }

}
