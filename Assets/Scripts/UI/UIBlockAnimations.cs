using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIBlockAnimations : MonoBehaviour
{
    [SerializeField]
    Image[] frames, backgrounds;


    [SerializeField]
    public Color[] frames_colors, backgrounds_colors;
    [SerializeField]
    float _t = 1f;
    public float t
    {
        get
        {
            return _t;
        }
        set
        {
            _t = value;
            if (name == "StarmapInfoBlock")
                Debug.Log("set " + name + " t");
        }
    }
    float prev_t;
    [SerializeField]
    float away_scale = 0f;
    public void Update()
    {
        if (prev_t == t)
        {
            //return;
        }

        foreach (Image i in frames)
        {
            i.color = Color.Lerp(frames_colors[0], frames_colors[1], t);
        }
        foreach (Image i in backgrounds)
        {
            i.color = Color.Lerp(backgrounds_colors[0], backgrounds_colors[1], t);
        }
        transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vector3.zero, away_rotation, t));
        transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * away_scale, t);
        GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(screen_positions[0], screen_positions[1], t);
        prev_t = t;
    }

    [SerializeField]
    Vector3 away_rotation;
    [SerializeField]
    public Vector2[] screen_positions;

}
