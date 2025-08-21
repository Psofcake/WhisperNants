using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DD_Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject prefab = Resources.Load(typeof(T).Name) as GameObject;
                GameObject singleton = Instantiate(prefab);
                _instance = singleton.GetComponent<T>();
            }
            
            return _instance;
        }
    }

    public DD_Singleton()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
    }
    
    protected virtual void Awake()
    {
        if (_instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
