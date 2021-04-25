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
    public EntityList<EnemyPositionPreset> enemies => node.HasProperty("enemies") ? new EntityList<EnemyPositionPreset>(node.GetNode<YamlSequenceNode>("enemies")) : new EntityList<EnemyPositionPreset>();
    public List<string> dialogue_lines => node.HasProperty("dialogue") ? GetStringArray("dialogue") : new List<string>();
}
public class EnemyPositionPreset : Entity
{
    public string id => node.Get("id");
    public EnemyPreset enemy_preset => SC.enemies.GetEnemyPresetById(id);
    public Vector2 position => node.GetVector2Int("position");
    public int wave => node.TryGetInt("wave", 0);


}
public class EnemyPreset : Entity
{
    public string id => node.Get("id");
    public string type => node.Get("type");
    public int hp => node.GetInt("hp");
    public int attack => node.TryGetInt("attack", 1);
    public FloatRange pursuit_delay => node.TryGetFloatRange("pursuit_delay", .5f, 1f);
    public int bullets_number => node.TryGetInt("bullets", 1);
    public float bullet_delay => node.TryGetFloat("bullet_delay", .3f);

    public string bullet_type => node.TryGet("bullet_type", "bullet");

    public float attack_range => node.TryGetFloat("attack_range", 1f);
    public float attack_delay => node.TryGetFloat("attack_delay", 1f);

    public FloatRange angle_range => node.TryGetFloatRange("angle_range", -.15f, .15f);
    public FloatRange radius_range => node.TryGetFloatRange("angle_range", 5f, 7f);

    public float move_speed => node.TryGetInt("move_speed", 5);

    public FloatRange scale_range => node.TryGetFloatRange("scale_range", .95f, 1.05f);

    public FloatRange move_shoot_delay => node.TryGetFloatRange("move_shoot_delay", .3f, .6f);
    public float bullet_spread => node.TryGetFloat("bullet_spread", .3f);

}