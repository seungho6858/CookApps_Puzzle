using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton_Awake<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance;

    protected bool Check(T t)
    {
        if (null == instance)
        {
            instance = t;
            return true;
        }
            
        if (instance != this)
        {
            Destroy(gameObject);
            return false;
        }

        return true;
    }

    protected  virtual void OnDestroy()
    {
        if(this == instance)
            instance = null;
    }
}
