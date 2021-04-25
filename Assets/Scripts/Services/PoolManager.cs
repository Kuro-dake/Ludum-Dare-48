using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A helper class to better watch the RPG stats and aspects of the game during the development (and maybe permanently?)
/// </summary>
[CreateAssetMenu(fileName = "PoolManager", menuName = "Service/PoolManager", order = 0)]
public class PoolManager : Service
{
    [SerializeField]
    Pool<Carrier> _carriers;
    public Pool<Carrier> carriers => _carriers;
    
    public override void SceneStartInitialize()
    {
        base.SceneStartInitialize();
        

    }
}
[System.Serializable]
public class Pool<T> where T : MonoBehaviour, PoolableInterface, new()
{
    List<T> pool = new List<T>();
    List<Pair<T, T>> prefab_pool = new List<Pair<T, T>>();
    Transform _carrier_parent;
    [SerializeField]
    List<PoolPrefab<T>> prefabs = new List<PoolPrefab<T>>();
    public Transform carrier_parent => _carrier_parent == null ? (_carrier_parent = new GameObject(typeof(T).ToString() + "Pool").transform) : _carrier_parent;

    public T GetPooledObjectFromPrefab(string name = "default", bool force_new = false)
    {
        T prefab = prefabs.Find(p => p.first == name).second;
        prefab_pool.RemoveAll(p => p.second == null);
        T ret = prefab_pool.Find(obj => !obj.second.is_active && obj.first == prefab)?.second;
        if (force_new)
        {
            ret = null;
        }
        if (ret == null)
        {
            ret = Object.Instantiate(prefab);
            ret.transform.SetParent(carrier_parent);
            prefab_pool.Add(new Pair<T, T>(prefab, ret));
        }

        return ret;
    }

    public T GetPooledObject()
    {
        pool.RemoveAll(obj => obj == null);
        T ret = pool.Find(obj => !obj.is_active);
        if (ret == null)
        {
            ret = new GameObject(typeof(T).ToString(), new System.Type[] { typeof(T) }).GetComponent<T>();
            ret.transform.SetParent(carrier_parent);
            pool.Add(ret);
        }

        return ret;
    }

}
[System.Serializable]
public class PoolPrefab<T> : Pair<string, T> where T : MonoBehaviour, PoolableInterface, new()
{
    public PoolPrefab(string n, T obj) : base(n, obj) { }
}

public interface PoolableInterface
{
    bool is_active { get; }
}
