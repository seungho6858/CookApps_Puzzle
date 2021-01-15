using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseComponent : MonoBehaviour
{
    private GameObject Go;
    [HideInInspector] public GameObject _Go
    {
        get 
        { 
            if (Go == null)
            {
                Go = gameObject;
            }
            return Go; 
        }
        set { Go = value; }
    }

    private Transform Tr;
    [HideInInspector] public Transform _Tr
    {
        get { if (Tr == null) Tr = transform; return Tr; }
        set { Tr = value; }
    }

    protected virtual void Awake()
    {
        _Go = gameObject;
        _Tr = transform;
    }

    public void SetActivate(bool b)
    {
        if(null != _Go)
            _Go.SetActive(b);
    }

    public bool IsActive()
    {
        return _Go.activeSelf;
    }

    public virtual Transform Set_Pos(Vector3 vPos)
    {
        _Tr.position = vPos;
        return _Tr;
    }

    public Vector3 Get_Pos()
    {
        return _Tr.position;
    }
}
