using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Service Container class
/// </summary>

public class SC : MonoBehaviour
{
    static Dictionary<System.Type, Service> services;

    static T Get<T>() where T : Service
    {

        CheckIfInitialized();
        try
        {
            return services[typeof(T)] as T;
        }
        catch (System.Exception)
        {
            throw new System.Exception(typeof(T).ToString() + " was not found");
        }

    }
    public static T GetServiceUninitializedObject<T>() where T : Service => Resources.LoadAll<T>(services_folder)[0];
    public static bool initialized { get; protected set; } = false;


    static SC inst;
    public static void CheckIfInitialized()
    {
        if (!initialized)
        {
            throw new UnityException("The Service container was not initialized.");
        }

        if (inst == null)
        {
            throw new UnityException("The Service container was not initialized on scene load.");
        }


    }

    public static void SceneStartInitServices()
    {
        inst = new GameObject("ServiceContainer", new System.Type[] { typeof(SC) }).GetComponent<SC>();
        foreach (KeyValuePair<System.Type, Service> kv in services)
        {
            kv.Value.SceneStartInitialize();
        }

    }
    static bool first_scene = true;
    public static void Initialize()
    {
        if (first_scene)
        {
            GameStartInitServices();
        }
        SceneStartInitServices();
        if (first_scene)
        {
            foreach (KeyValuePair<System.Type, Service> kv in services)
            {
                kv.Value.PostGameStartInitServices();
            }
            first_scene = false;
        }
    }
    const string services_folder = "Services";
    public static void GameStartInitServices()
    {
        if (initialized)
        {
            return;
        }
        initialized = true;

        services = new Dictionary<System.Type, Service>();
        List<Service> all_services = new List<Service>(Resources.LoadAll<Service>(services_folder));
        foreach (Service s in all_services)
        {
            if (!services.ContainsKey(s.GetType()))
            {
                services.Add(s.GetType(), s);
            }

        }
        Debug.Log("initialized " + services.Count + " services");
        foreach (Service s in all_services)
        {
            s.GameStartInitialize();
        }
    }

    private void Update()
    {
        /*System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        string bench = "";*/
        foreach (KeyValuePair<System.Type, Service> kv in services)
        {
            if (!kv.Value.active)
            {
                continue;
            }
            //sw.Start();
            kv.Value.Update();
            /*sw.Stop();
            bench += kv.Key.ToString() + ": " + sw.ElapsedMilliseconds + "\n";
            sw.Reset();*/
        }
        //Debug.Log(bench);
    }

    [SerializeField]
    static Routines _routines;
    public static Routines routines
    {
        get
        {
            //CheckIfInitialized();

            if (_routines == null)
            {
                Routines[] service = Resources.LoadAll<Routines>("Services");
                _routines = service[0];
            }

            return _routines;

        }
    }

    public static GameManager game => Get<GameManager>();
    public static UIManager ui => Get<UIManager>();
    public static PoolManager pool => Get<PoolManager>();
    public static EnemyManager enemies => Get<EnemyManager>();
    public static EnvManager env => Get<EnvManager>();
    public static PlayerStatsManager player_stats => Get<PlayerStatsManager>();
    public static Controls controls => Get<Controls>();

}
