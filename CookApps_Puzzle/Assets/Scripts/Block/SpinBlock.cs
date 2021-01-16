using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinBlock : Block
{
    private int _hp;
    
    private void OnEnable()
    {
        _hp = 2;
    }

    public override bool Get_Damage()
    {
        base.Get_Damage();

        _hp--;

        if (_hp <= 0)
        {
            Explode();
            return true;
        }

        return false;
    }

    private void Update()
    {
        if (_hp > 1)
            return;
        
        _Tr.Rotate(0f, 0f, Time.deltaTime * 360f, Space.World);
        
    }

    public override void Explode()
    {
        Get_Point().Relase_Block();
        
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        float time = 1f;

        Vector2 vDir = UIManager.instance._tSpin.transform.position - _Tr.position;
        
        while (true)
        {
            time -= Time.deltaTime;
            if (time <= 0f)
                break;

            _Tr.Translate(vDir * Time.deltaTime, Space.World);
            
            yield return null;
        }
        
        BlockManager.instance.ReturnBlock(this);
        UIManager.instance.Get_Spin();
    }
}
