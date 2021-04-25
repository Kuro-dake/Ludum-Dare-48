using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.RepresentationModel;

[System.Serializable]
public abstract class Entity
{
    public YamlMappingNode node { get; protected set; }
    public YamlMappingNode parent_node { get; protected set; }
    public static T Create<T>(YamlMappingNode n, YamlMappingNode parent = null) where T : Entity, new()
    {
        T ret = new T();
        ret.node = n;
        ret.parent_node = parent;
        return ret;
    }

    public Dictionary<T, T1> ConvertDictionaryKeyToEnum<T, T1>(Dictionary<string, T1> d) where T : System.Enum
    {
        Dictionary<T, T1> ret = new Dictionary<T, T1>();
        foreach (KeyValuePair<string, T1> kv in d)
        {
            ret.Add((T)System.Enum.Parse(typeof(T), kv.Key), kv.Value);
        }
        return ret;
    }

    public Dictionary<string, string> GetStringDictionary(YamlMappingNode map)
    {
        Dictionary<string, string> ret = new Dictionary<string, string>();
        foreach (KeyValuePair<YamlNode, YamlNode> kv in map)
        {
            ret.Add(kv.Key.ToString(), kv.Value.ToString());
        }
        return ret;
    }

    public Dictionary<string, int> GetIntDictionary(YamlMappingNode map)
    {
        Dictionary<string, int> ret = new Dictionary<string, int>();
        foreach (KeyValuePair<YamlNode, YamlNode> kv in map)
        {
            ret.Add(kv.Key.ToString(), int.Parse(kv.Value.ToString()));
        }
        return ret;
    }

    public Dictionary<string, int[]> GetIntArrayDictionary(YamlMappingNode map)
    {
        Dictionary<string, int[]> ret = new Dictionary<string, int[]>();
        foreach (KeyValuePair<YamlNode, YamlNode> kv in map)
        {
            ret.Add(kv.Key.ToString(), kv.Value.ToIntArray());
        }
        return ret;
    }

    public Dictionary<string, float> GetFloatDictionary(YamlMappingNode map)
    {
        Dictionary<string, float> ret = new Dictionary<string, float>();
        foreach (KeyValuePair<YamlNode, YamlNode> kv in map)
        {
            ret.Add(kv.Key.ToString(), float.Parse(kv.Value.ToString()));
        }
        return ret;
    }
    Dictionary<string, string> cache = new Dictionary<string, string>();
    public string Get(string what)
    {
        if (!cache.ContainsKey(what))
        {
            cache.Add(what, node.TryGet(what));
        }
        return cache[what];
    }
    Dictionary<string, Entity> entity_cache = new Dictionary<string, Entity>();
    /// <summary>
    /// Method for getting and caching subentities
    /// </summary>
    public T Get<T>(string what) where T : Entity, new()
    {
        if (!entity_cache.ContainsKey(what))
        {
            YamlMappingNode n = null;
            try { n = node.GetNode<YamlMappingNode>(what); }
            catch { }
            T to_add = n != null ? Create<T>(n) : null;
            entity_cache.Add(what, to_add);
        }
        return (T)entity_cache[what];
    }

    public List<string> GetStringArray(string what)
    {
        List<string> ret = new List<string>();
        YamlSequenceNode srds = node.TryGetNode<YamlSequenceNode>(what);
        if (srds != null)
        {
            foreach (YamlNode val in srds)
            {
                ret.Add(val.ToString());
            }
        }
        return ret;
    }

    public virtual void CheckIntegrity() { }

    static int current_unid = 1;
    protected int unid;
    protected Entity() { unid = current_unid++; }

    public override int GetHashCode()
    {
        return unid;
    }

}

public class BlockPreset : Entity
{
    public EntityList<EnemyPreset> enemies => node.HasProperty("enemies") ? new EntityList<EnemyPreset>(node.GetNode<YamlSequenceNode>("enemies")) : new EntityList<EnemyPreset>();
    public List<string> dialogue_lines => node.HasProperty("dialogue") ? GetStringArray("dialogue") : new List<string>();
}

public class EnemyPreset : Entity
{
    public string type => node.Get("type");
    public int hp => node.GetInt("hp");
    public Vector2 position => node.GetVector2Int("position");

    public int wave => node.TryGetInt("wave", 0);
}