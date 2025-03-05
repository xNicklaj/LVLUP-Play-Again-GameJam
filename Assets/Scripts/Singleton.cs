using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
    where T : Singleton<T>
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