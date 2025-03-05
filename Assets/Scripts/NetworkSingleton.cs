using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class NetworkSingleton<T> : NetworkBehaviour
    where T : NetworkSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType(typeof(T)) as T;

            return instance;
        }
        set
        {
            instance = value;
        }
    }
}