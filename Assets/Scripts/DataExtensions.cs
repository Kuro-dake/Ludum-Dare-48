using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;



using YamlDotNet.Serialization;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Samples.Helpers;

public static class YAML
{

    static Dictionary<string, YamlMappingNode> files = new Dictionary<string, YamlMappingNode>();
    public static void ClearBuffer(string file = "")
    {
        if (file == "")
        {
            files.Clear();
        }
        else
        {
            if (files.ContainsKey(file))
            {
                files.Remove(file);
            }
        }
    }
    public static string GetYamlFilePath(string filename)
    {
        return "YAML/" + filename;
    }
    public static YamlMappingNode Read(string filename)
    {
        if (!files.ContainsKey(filename))
        {
            string ymlstring = Resources.Load<TextAsset>(GetYamlFilePath(filename)).text;
            System.IO.StringReader r = new System.IO.StringReader(ymlstring);

            YamlStream yaml = new YamlStream();
            yaml.Load(r);

            files.Add(filename, (YamlMappingNode)yaml.Documents[0].RootNode);
        }

        return files[filename];
    }
}

public class Setup
{

    static Dictionary<string, Setup> named_setup = new Dictionary<string, Setup>();
    public static Setup GetSetup(string name)
    {
        if (!named_setup.ContainsKey(name))
        {
            named_setup.Add(name, new Setup(name));
        }
        return named_setup[name];
    }
    public static bool SetupExists(string filename)
    {
        return Resources.Load<TextAsset>(YAML.GetYamlFilePath(filename)) != null;
    }


    string file;
    public Setup(string filename)
    {
        file = filename;
    }
    public static void ClearBuffer(string setup = "")
    {
        YAML.ClearBuffer(setup);
        if (setup == "")
        {
            named_setup.Clear();
        }
        else
        {
            if (named_setup.ContainsKey(setup))
            {
                named_setup.Remove(setup);
            }
        }
    }

    Dictionary<string, YamlNode> buffer = new Dictionary<string, YamlNode>();

    public T GetChainedNode<T>(string s) where T : YamlNode
    {
        s = s.Replace(":", ";");
        if (!buffer.ContainsKey(s))
        {
            string[] steps = s.Split(new char[] { ';' });
            YamlMappingNode current = YAML.Read(file);
            int i = 0;
            try
            {
                for (i = 0; i < steps.Length - 1; i++)
                {
                    current = current.GetNode<YamlMappingNode>(steps[i]);
                }
                buffer.Add(s, current.GetNode<T>(steps[steps.Length - 1]));
            }
            catch (KeyNotFoundException e)
            {
                Debug.Log("failed at " + i + " : " + steps[i] + " of : " + s);
                throw e;
            }
        }
        return buffer[s] as T;
    }
    string GetChainedNodeValue(string s)
    {
        return GetChainedNode<YamlNode>(s).ToString();
    }

    public float GetFloat(string name) { return float.Parse(GetChainedNodeValue(name)); }
    public int GetInt(string name) { return int.Parse(GetChainedNodeValue(name)); }
    public string Get(string name) { return GetChainedNodeValue(name); }

    public float[] AGetFloat(string name) { return GetChainedNodeValue(name).ToFloatArray(); }
    public int[] AGetInt(string name) { return GetChainedNodeValue(name).ToIntArray(); }
    public string[] AGet(string name) { return GetChainedNodeValue(name).ToArray(); }


    public static YamlMappingNode GetFile(string filename)
    {
        return YAML.Read(filename);
    }

}



public static class DataExtensions
{

    public static string Get(this YamlMappingNode n, string what)
    {
        return n.GetNode(what).ToString();
    }
    public static string TryGet(this YamlMappingNode n, string what, string _default = "")
    {
        try
        {
            return n.Get(what);
        }
        catch (KeyNotFoundException)
        {
            return _default;
        }

    }
    public static float GetFloat(this YamlMappingNode n, string what)
    {
        return float.Parse(n.GetNode(what).ToString());
    }
    public static int GetInt(this YamlMappingNode n, string what)
    {
        return int.Parse(n.GetNode(what).ToString());
    }
    public static int TryGetInt(this YamlMappingNode n, string what, int _default = 0)
    {
        try
        {
            return n.GetInt(what);
        }
        catch (KeyNotFoundException)
        {
            return _default;
        }
    }
    public static float TryGetFloat(this YamlMappingNode n, string what, float _default = 0)
    {
        try
        {
            return n.GetFloat(what);
        }
        catch (KeyNotFoundException)
        {
            return _default;
        }
    }
    public static bool TryGetBool(this YamlMappingNode n, string what, bool _default = false)
    {
        try
        {
            return n.GetBool(what);
        }
        catch (KeyNotFoundException)
        {
            return _default;
        }
    }
    public static bool GetBool(this YamlMappingNode n, string what)
    {
        return n.Get(what) == "true" ? true : (n.Get(what) == "false" ? false : n.GetInt(what) != 0);
    }
    public static string[] TryGetArray(this YamlMappingNode n, string what, string[] _default = null)
    {
        try
        {
            return n.GetArray(what);
        }
        catch
        {
            return _default == null ? new string[] { } : _default;
        }
    }
    public static string[] GetArray(this YamlMappingNode n, string what)
    {
        return n.Get(what).Split(new char[] { ',' });
    }
    public static float[] GetFloatArray(this YamlMappingNode n, string what)
    {
        return new List<string>(n.Get(what).Split(new char[] { ',' })).ConvertAll<float>(delegate (string input) {
            return float.Parse(input);
        }).ToArray();
    }
    public static Color TryGetColor(this YamlMappingNode n, string what, Color _default = default)
    {
        return n.HasProperty(what) ? n.GetColor(what) : _default;
    }
    public static Color GetColor(this YamlMappingNode n, string what)
    {
        Color ret;
        ColorUtility.TryParseHtmlString(n.Get(what), out ret);
        return ret;
    }
    public static Color[] GetColorArray(this YamlMappingNode n, string what)
    {
        return new List<string>(n.Get(what).Split(new char[] { ',' })).ConvertAll(delegate (string input) {
            Color ret;

            ColorUtility.TryParseHtmlString(input, out ret);

            return ret;
        }).ToArray();
    }
    public static int[] ToIntArray(this YamlNode n)
    {
        return new List<string>(n.ToString().Split(new char[] { ',' })).ConvertAll<int>(delegate (string input) {
            return int.Parse(input);
        }).ToArray();
    }
    public static int[] GetIntArray(this YamlMappingNode n, string what)
    {
        return new List<string>(n.Get(what).Split(new char[] { ',' })).ConvertAll<int>(delegate (string input) {
            return int.Parse(input);
        }).ToArray();
    }
    public static Vector2Int GetVector2Int(this YamlMappingNode n, string what)
    {
        int[] coords = n.GetIntArray(what);
        return new Vector2Int(coords[0], coords[1]);
    }
    public static Vector2Int TryGetVector2Int(this YamlMappingNode n, string what, int default_x = 0, int default_y = 0)
    {
        return n.HasProperty(what) ? n.GetVector2Int(what) : new Vector2Int(default_x, default_y);
    }
    public static bool HasProperty(this YamlMappingNode n, string what)
    {
        return n.Children.ContainsKey(what);
    }
    public static IntRange TryGetIntRange(this YamlMappingNode n, string what, int default_min = 1, int default_max = 2)
    {
        try
        {
            return n.GetIntRange(what);
        }
        catch
        {
            return new IntRange(default_min, default_max);
        }
    }
    public static IntRange GetIntRange(this YamlMappingNode n, string what)
    {
        int[] range = n.GetIntArray(what);
        return new IntRange(range[0], (range.Length > 1 ? range[1] : range[0]));
    }
    public static FloatRange TryGetFloatRange(this YamlMappingNode n, string what, float default_min = 0f, float default_max = 0f)
    {
        try
        {
            return n.GetFloatRange(what);
        }
        catch
        {
            return new FloatRange(default_min, default_max);
        }
    }
    public static FloatRange GetFloatRange(this YamlMappingNode n, string what)
    {
        return new FloatRange(n.GetFloatArray(what));
    }
    public static ColorRange TryGetColorRange(this YamlMappingNode n, string what, Color default_min = default, Color default_max = default)
    {
        return n.HasProperty(what) ? n.GetColorRange(what) : new ColorRange(default_min == default ? Color.white : default_min, default_max == default ? Color.white : default_max);
    }
    public static ColorRange GetColorRange(this YamlMappingNode n, string what)
    {
        return new ColorRange(n.GetColorArray(what));
    }

    public static YamlNode GetNode(this YamlMappingNode n, string what)
    {
        return n.Children[new YamlScalarNode(what)];
    }
    public static T TryGetNode<T>(this YamlMappingNode n, string what) where T : YamlNode
    {
        try
        {
            return n.GetNode<T>(what);
        }
        catch
        {
            return null;
        }
    }
    public static T GetNode<T>(this YamlMappingNode n, string what) where T : YamlNode
    {
        return (T)n.GetNode(what);
    }

    public static string[] ToArray(this string n)
    {
        return n.Split(new char[] { ',' });
    }
    public static float[] ToFloatArray(this string n)
    {
        return new List<string>(n.ToArray()).ConvertAll<float>(delegate (string input) {
            return float.Parse(input);
        }).ToArray();
    }
    public static int[] ToIntArray(this string n)
    {
        return new List<string>(n.ToArray()).ConvertAll<int>(delegate (string input) {
            return int.Parse(input);
        }).ToArray();
    }

    public static void x(this Transform t, float x)
    {
        Vector3 v = t.position;
        v.x = x;
        t.position = v;
    }

    public static float y(this Transform t)
    {
        return t.position.y;
    }
    public static float x(this Transform t)
    {
        return t.position.x;
    }

    public static float y(this Rigidbody2D t)
    {
        return t.position.y;
    }
    public static float x(this Rigidbody2D t)
    {
        return t.position.x;
    }

    public static float w(this GameObject g)
    {
        return g.sr().bounds.size.x;
    }

    public static void w(this GameObject g, float w)
    {
        g.transform.localScale = new Vector3(w, g.transform.localScale.y);
    }
    public static float h(this GameObject g)
    {
        return g.transform.localScale.y;
    }
    public static void h(this GameObject g, float h)
    {
        g.transform.localScale = new Vector3(g.transform.localScale.x, h);
    }
    public static int Sign(this int f)
    {
        return f == 0 ? 0 : (f > 0 ? 1 : -1);
    }
    public static float Sign(this float f)
    {
        return f == 0f ? 0f : Mathf.Sign(f);
    }
    public static int SignInt(this float f)
    {
        return Mathf.RoundToInt(f.Sign());
    }
    public static bool Axis(this float f)
    {
        return f.Sign() != 0f;
    }

    /// <summary>
    /// Set transform.position.y
    /// </summary>
    /// <param name="t">T.</param>
    /// <param name="y">Y coordinate.</param>

    public static void y(this Transform t, float y)
    {
        Vector3 v = t.position;
        v.y = y;
        t.position = v;
    }
    /// <summary>
    /// Set rigidbody2d.position.x
    /// </summary>
    /// <param name="t">T.</param>
    /// <param name="x">The x coordinate.</param>
    public static void x(this Rigidbody2D t, float x)
    {
        Vector3 v = t.position;
        v.x = x;
        t.position = v;
    }
    /// <summary>
    /// Set rigidbody2d.position.y
    /// </summary>
    /// <param name="t">T.</param>
    /// <param name="y">The y coordinate.</param>
    public static void y(this Rigidbody2D t, float y)
    {
        Vector3 v = t.position;
        v.y = y;
        t.position = v;
    }
    public static Rigidbody2D rb2d(this Component t)
    {
        return t.GetComponent<Rigidbody2D>();
    }
    public static SpriteRenderer sr(this Component t)
    {
        return t.GetComponent<SpriteRenderer>();
    }
    public static UnityEngine.UI.Image img(this Component t)
    {
        return t.GetComponent<UnityEngine.UI.Image>();
    }
    public static Rigidbody2D rb2d(this GameObject t)
    {
        return t.GetComponent<Rigidbody2D>();
    }
    public static SpriteRenderer sr(this GameObject t)
    {
        return t.GetComponent<SpriteRenderer>();
    }

    public static void DestroyChildren(this Transform t)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            GameObject.Destroy(t.GetChild(i).gameObject);
        }
        t.DetachChildren();
    }
    public static bool ContainsAny<T>(this List<T> l, List<T> contains)
    {

        foreach (T obj in contains)
        {
            if (l.Contains(obj))
            {
                return true;
            }
        }
        return false;

    }

    public static bool ContainsAll<T>(this List<T> l, List<T> contains)
    {
        bool ret = true;
        contains.ForEach(delegate (T obj) {
            if (!l.Contains(obj))
            {
                ret = false;
            }
        });
        return ret;
    }

    public static void RemoveAll<T>(this List<T> l, List<T> remove_list)
    {
        remove_list.ForEach(delegate (T obj) {
            l.Remove(obj);
        });
    }
    public static Vector3 Clamp(this Vector3 v, Vector3 lower_limit, Vector3 upper_limit)
    {
        return new Vector3(Mathf.Clamp(v.x, lower_limit.x, upper_limit.x), Mathf.Clamp(v.y, lower_limit.y, upper_limit.y), Mathf.Clamp(v.z, lower_limit.z, upper_limit.z));
    }
    public static Vector3 Vector3(this Vector2 v, float z = 0f)
    {
        return new Vector3(v.x, v.y, z);
    }
    public static Vector2 Vector2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static T RandomElement<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }
    public static T FindFirst<T>(this List<T> l, List<T> find_list)
    {
        return l.Find(delegate (T obj) {
            return find_list.Contains(obj);
        });
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}
[System.Serializable]
public class NamedAudioClip : Pair<string, AudioClip>
{
    public NamedAudioClip(string s, AudioClip a) : base(s, a) { }
}

[System.Serializable]
public class NamedColor : Pair<string, Color>
{
    public NamedColor(string s, Color c) : base(s, c) { }
}


[System.Serializable]
public class Pair<T1, T2>
{
    public T1 first;
    public T2 second;
    internal Pair(T1 f, T2 s)
    {
        first = f;
        second = s;
    }

    public override string ToString()
    {
        return "Pair<" + typeof(T1).ToString() + "," + typeof(T2).ToString() + "> " + first.ToString() + ", " + second.ToString();
    }
}



public static class Pair
{
    public static Pair<T1, T2> New<T1, T2>(T1 first, T2 second)
    {
        var pair = new Pair<T1, T2>(first, second);
        return pair;
    }

}
[System.Serializable]
public class NamedSprite : Pair<string, Sprite>
{
    public NamedSprite(string n, Sprite s) : base(n, s)
    {
    }
}
[System.Serializable]
public class AbilityLevel : Pair<string, int>
{
    public string name => first;
    public int level => second;

    public AbilityLevel(string n, int l) : base(n, l) { }
}

[System.Serializable]
public class NamedObjects
{
    public NamedObjects() { }
    public List<NamedOjbectType> objects = new List<NamedOjbectType>();
    public GameObject GetByName(string name)
    {
        NamedOjbectType ret = objects.Find(delegate (NamedOjbectType e)
        {
            return e.first == name;
        });

        return ret == null ? null : ret.second;
    }
}

[System.Serializable]
public class NamedOjbectType : Pair<string, GameObject>
{
    public NamedOjbectType(string n, GameObject s) : base(n, s)
    {
    }
}
[System.Serializable]
public class IntRange : Pair<int, int>
{
    public bool constant => min == max;
    public int min { get { return first; } set { first = value; } }
    public int max { get { return second; } set { second = value; } }
    public IntRange(int a, int b) : base(a, b) { }
    public IntRange(int[] range) : base(range[0], range[1]) { }
    int _steps = 0;
    List<float> step_values = new List<float>();
    float step_value = 0f;
    public int steps
    {
        get
        {
            return _steps;
        }
        set
        {
            _steps = value;
            List<float> step_values = new List<float>();
            step_values.Clear();

            step_value = (max - min) / (float)(_steps - 1);
            for (int i = 0; i < _steps; i++)
            {
                step_values.Add(min + step_value * i);
            }
        }
    }
    public float StepValue(int step)
    {
        if (steps == 0)
        {
            throw new UnityException("Range steps have not been set");
        }
        if (step < 0)
        {
            return step_values[step_values.Count + step];
        }
        return step_values[step];
    }
    public int GetStep(float val)
    {


        if (val < min)
        {
            return 0;
        }
        return Mathf.RoundToInt((val + (step_value)) / step_value);
    }
    public float this[int step] { get { return StepValue(step); } }
    public int random { get { return min == max ? min : Random.Range(min, max); } }
    public static implicit operator int(IntRange d)
    {
        return d.random;
    }

    public override string ToString()
    {
        return string.Format("[IntRange: min={0}, max={1}, steps={2}]", min, max, steps);
    }

    public float average => ((float)min + (float)max - 1f) * .5f;
    public bool IsNumberInRangeInclusive(int number)
    {
        return number >= min && number <= max;
    }
}



[System.Serializable]
public class FloatRange : Pair<float, float>
{
    public float min { get { return first; } set { first = value; } }
    public float max { get { return second; } set { second = value; } }
    int _steps = 0;
    List<float> step_values = new List<float>();
    float step_value = 0f;
    public int steps
    {
        get
        {
            return _steps;
        }
        set
        {
            _steps = value;
            step_values.Clear();

            step_value = (max - min) / (float)(_steps - 1);
            for (int i = 0; i < _steps; i++)
            {
                step_values.Add(min + step_value * i);
            }
        }
    }
    public FloatRange(float a, float b) : base(a, b) { }
    public FloatRange() : base(0f, 0f) { }
    public FloatRange(float[] range) : base(range[0], range[1]) { }
    public float random { get { return min == max ? min : Random.Range(min, max); } }
    public static implicit operator float(FloatRange d)
    {
        return d.random;
    }

    public float StepValue(int step)
    {
        if (steps == 0)
        {
            throw new UnityException("Range steps have not been set");
        }
        if (step < 0)
        {
            return step_values[step_values.Count + step];
        }
        return step_values[step];
    }
    public int GetStep(float val)
    {


        if (val < min)
        {
            return 0;
        }
        return Mathf.RoundToInt((val + (step_value)) / step_value);
    }
    public float this[int step] { get { return StepValue(step); } }
    public override string ToString()
    {
        return string.Format("[FloatRange: min={0}, max={1}, steps={2}]", min, max, steps);
    }

}


public class ColorRange : Pair<Color, Color>
{
    public ColorRange(Color a, Color b) : base(a, b) { }
    public ColorRange(Color[] a) : base(a[0], a[0])
    {

        first = a[0];
        if (a.Length > 1)
        {
            second = a[1];
        }
        else
        {
            second = a[0];
        }
    }
    public Color random => Color.Lerp(first, second, Random.Range(0f, 1f));

}

public static class BoundsExtension
{
    public static Vector2[] Borders(this Bounds b, Vector2 modify = new Vector2())
    {
        return new Vector2[] { b.min.Vector2() - modify, new Vector2(b.min.x, b.max.y) - modify, b.max.Vector2() - modify, new Vector2(b.max.x, b.min.y) - modify };
    }
}
public enum direction
{
    top,
    bottom,
    left,
    right
}
public static class EntryPointSideExtensions
{
    public static List<direction> entry_sides
    {
        get
        {
            List<direction> ret = new List<direction>((direction[])System.Enum.GetValues(typeof(direction)));
            
            return ret;
        }

    }
    public static direction OppositeSide(this direction t)
    {
        switch (t)
        {
            case direction.bottom:
                return direction.top;
            case direction.top:
                return direction.bottom;
            case direction.left:
                return direction.right;
            case direction.right:
                return direction.left;
            default:
                return t;
        }
    }
    public static bool IsDirection(this direction emps)
    {
        return true;
    }
    public static int GridXFromSideModifier(this direction epms, int x = 0)
    {
        switch (epms)
        {
            case direction.left:
                return x + 1;
            case direction.right:
                return x - 1;
            default:
                return x;
        }
    }
    public static int GridYFromSideModifier(this direction epms, int y = 0)
    {
        switch (epms)
        {
            case direction.top:
                return y - 1;
            case direction.bottom:
                return y + 1;
            default:
                return y;
        }
    }
    public static int GridXToSideModifier(this direction epms, int x = 0)
    {
        return x - epms.GridXFromSideModifier();
    }
    public static int GridYToSideModifier(this direction epms, int y = 0)
    {
        return y - epms.GridYFromSideModifier();
    }
    public static Vector2 GridToSideModifier(this direction emps)
    {
        return new Vector2(emps.GridXToSideModifier(), emps.GridYToSideModifier());
    }
}

public static class StringExtension
{
    public static string UCFirst(this string s)
    {
        return s.Substring(0, 1).ToUpper() + s.Substring(1);
    }
}

public static class Vector2IntExtensions
{

    static Vector2Int[][] range_directions;

    public static List<Vector2Int> GetPositionsInRange(this Vector2Int center, int range)
    {

        if (range_directions == null)
        {
            range_directions = new Vector2Int[][]{
                new Vector2Int[]{Vector2Int.up , Vector2Int.right },
                new Vector2Int[]{Vector2Int.one , new Vector2Int(1,-1) },
                new Vector2Int[]{Vector2Int.up * 2, Vector2Int.right * 2},
                new Vector2Int[]{new Vector2Int(2,1) , new Vector2Int(1,2), new Vector2Int(2,-1), new Vector2Int(1,-2) },
            };
        }

        List<Vector2Int> ret = new List<Vector2Int>() { center };

        for (int i = 0; i < range; i++)
        {
            foreach (Vector2Int v2i in range_directions[i])
            {
                ret.Add(center + v2i);
                ret.Add(center - v2i);
            }
        }

        return ret;
    }
}