using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticPSDAnimation : MonoBehaviour
{

    private void Start()
    {
        Initialize();
    }

    public bool dis = false;
    public float change_in = .1f;

    public List<FrameList> frames = new List<FrameList>();

    public bool manual = false;

    static List<StaticPSDAnimation> all_animations = new List<StaticPSDAnimation>();

    private void Awake()
    {
        all_animations.Add(this);

    }
    public static void InitializeAnimation()
    {
        all_animations.ForEach(delegate (StaticPSDAnimation spa)
        {
            spa.Initialize();
        });
    }
    static int current_anim_index = 0;
    public int anim_index { get; protected set; }
    bool initialized = false;
    public void Initialize()
    {
        frames.ForEach(delegate (FrameList fl)
        {
            fl.Initialize();
        });
        if (manual)
        {
            return;
            /*frames.RemoveAll(delegate (FrameList fl)
            {
                return !fl.any_active;
            });*/
        }
        StartCoroutine(ChangeFrameStep());
        if (!initialized)
        {
            anim_index = current_anim_index++;
        }
        initialized = true;

    }

    public void AddFrame(string name, GameObject frame)
    {
        if (!ContainsKey(name))
        {
            frames.Add(new FrameList(name));
        }

        this[name].frames.Add(frame);
    }

    public bool ContainsKey(string key)
    {
        return this[key] != null;
    }

    public FrameList this[string name]
    {
        get
        {
            return frames.Find(delegate (FrameList f)
            {
                return f.name == name;
            });
        }
    }

    public void AdvanceFrame()
    {
        foreach (FrameList fl in frames)
        {
            fl.AdvanceFrame();
        }
    }

    public void RefreshFrame(Group group = null)
    {
        foreach (FrameList fl in frames)
        {
            fl.RefreshFrame(group);
        }
    }

    IEnumerator ChangeFrameStep()
    {
        while (true)
        {
            //Debug.Log("running");
            yield return new WaitForSecondsRealtime(change_in);

            if (dis)
            {
                foreach (FrameList fl in frames)
                {
                    foreach (GameObject go in fl.frames)
                    {
                        go.GetComponent<SpriteRenderer>().color = Color.white;
                        go.SetActive(false);
                    }
                }
                while (dis)
                {
                    yield return null;
                }
            }

            AdvanceFrame();
        }
    }
    private void OnDestroy()
    {
        all_animations.Remove(this);
    }
    void OnDisable()
    {

    }
    public void DisableFrameList(string frame_list)
    {
        if (this[frame_list] != null)
        {

            this[frame_list].enabled = false;
            this[frame_list].frames.ForEach(delegate (GameObject go)
            {
                go.SetActive(false);
            });
        }
        Transform color = transform.Find(frame_list + " color");
        if (color != null)
        {
            color.gameObject.SetActive(false);
        }

    }


    public void EnableFrameList(string frame_list)
    {
        if (this[frame_list] != null)
        {

            this[frame_list].enabled = true;
            this[frame_list].frames[0].SetActive(true);
        }
        Transform color = transform.Find(frame_list + " color");
        if (color != null)
        {
            color.gameObject.SetActive(true);
        }

    }


}

[System.Serializable]
public class FrameList
{
    public string name;
    public List<GameObject> frames;

    public int current_frame = 0;

    public bool enabled
    {
        get
        {
            return _enabled;
        }
        set
        {
            _enabled = value;
            if (enabled)
            {
                RefreshFrame();
            }
            else
            {
                frames.ForEach(delegate (GameObject go) { go.SetActive(false); });
            }
        }
    }
    public bool _enabled = true;


    public FrameList(string nname)
    {
        name = nname;
        frames = new List<GameObject>();
    }

    public bool any_active
    {
        get
        {
            return frames.Find(delegate (GameObject go) {
                return go.activeSelf;
            }) != null;
        }
    }

    public void Initialize()
    {
        if (frames.Count == 0)
        {
            return;
        }
        current_frame = Random.Range(0, frames.Count);
        if (enabled)
        {
            RefreshFrame();
        }
        else
        {
            frames.ForEach(delegate (GameObject go) { go.SetActive(false); });
        }

    }
    bool exclusion = true;
    public void RefreshFrame(Group group = null)
    {
        foreach (GameObject frame in frames)
        {
            frame.SetActive(exclusion);
        }
        if (group == null || group.objects.Contains(frames[current_frame]))
        {
            frames[current_frame].SetActive(!exclusion);
        }

    }

    public void AdvanceFrame()
    {
        current_frame++;
        if (current_frame >= frames.Count)
        {
            current_frame = 0;
        }
        if (enabled)
        {
            RefreshFrame();
        }
    }
}

[System.Serializable]
public class Group
{
    public string name;
    public bool active = true;
    public List<GameObject> objects = new List<GameObject>();

    public virtual void RefreshGroup()
    {

        objects.ForEach(delegate (GameObject go)
        {
            if (go == null)
            {
                return;
            }
            go.SetActive(active);
        });
    }

}