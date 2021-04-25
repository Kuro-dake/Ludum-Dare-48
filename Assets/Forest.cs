using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
public class Forest : MonoBehaviour
{
    [SerializeField]
    EnvTree tree_prefab;

    float last_x_generated;
    float cam_x => Camera.main.transform.position.x;
    private void Start()
    {
        last_x_generated = cam_x;
    }
    //[SerializeField]
    float density => /*(1000f - cam_x) / 100f*/6f;
    [SerializeField]
    Transform forest_screen;
    [SerializeField]
    float size_divider = 50f;
    [SerializeField]
    Light2D global_light;
    [SerializeField]
    float[] light_intensity;
    [SerializeField]
    Color[] light_color, sky_color;

    [SerializeField]
    FloatRange[] aim_intensity_ranges;
    
    public void Initialize()
    {
        EnvManager.EnvPreset ep = SC.env.current_level_env_preset;

        light_intensity = ep.light_intensity;
        light_color = ep.light;
        sky_color = ep.sky;
        
    }

    void CreateTree()
    {
        EnvTree et = Instantiate(tree_prefab);
        EnvManager.EnvPreset ep = SC.env.current_level_env_preset;
        et.GetComponent<SpriteRenderer>().color = Color.Lerp(ep.tree_bark[0], ep.tree_bark[1], Random.Range(0f, 1f));
        ParticleSystem.MainModule mm = et.GetComponentInChildren<ParticleSystem>().main;
        mm.startColor = new ParticleSystem.MinMaxGradient(ep.tree_crown[0], ep.tree_crown[1]);
        Vector3 pos = Vector3.zero;

        pos.x = cam_x + (EnvTree.destroy_distance - 10f) * (cam_x > last_x_generated ? 1 : -1);

        et.transform.position = pos;

        et.transform.SetParent(transform);

        pos = et.transform.localPosition;
        pos.y = Random.Range(-1f, 1f);

        et.transform.localPosition = pos;

        et.transform.localScale = Vector3.one * Random.Range(.6f, .8f) * cam_multiplier;
        et.transform.localRotation = Quaternion.Euler(Vector3.back * Random.Range(-15f, 15f));
        last_x_generated = cam_x;

        et.Initialize();
    }

    private void Update()
    {
        if(Mathf.Abs(last_x_generated - cam_x) > density)
        {

            CreateTree();
            
        }
        forest_screen.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.clear, SC.env.current_level_env_preset.fscreen_color, progress_t);
        global_light.intensity = Mathf.Lerp(light_intensity[0], light_intensity[1], progress_t);
        global_light.color = Color.Lerp(light_color[0], light_color[1], progress_t);
        GM.player.aim_transform.GetComponent<LightGlow>().target_intensity_range 
            = new FloatRange(Mathf.Lerp(aim_intensity_ranges[0].min, aim_intensity_ranges[1].min, progress_t), Mathf.Lerp(aim_intensity_ranges[0].max, aim_intensity_ranges[1].max, progress_t));
        Camera.main.backgroundColor = Color.Lerp(sky_color[0], sky_color[1], progress_t);
        prog_t_disp = progress_t;
    }
    [SerializeField]
    float prog_t_disp;

    
    float cm_min = .3f;
    float cm_max = 3f;
    float cm_diff => cm_max - cm_min;

    float cam_multiplier => Mathf.Clamp(cam_x / size_divider, cm_min, cm_max);
    float cam_multiplier_minus_dist => Mathf.Clamp((cam_x - (EnvTree.destroy_distance - 10f)) / size_divider, cm_min, cm_max);
    float progress_t => SC.env.manual_env_manipulation ? prog_t_disp : (cam_multiplier_minus_dist - cm_min) / cm_diff;


}
