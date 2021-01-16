using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon_Point : Point
{
    public Transform _summonPos;

    // 새로 블럭을 생성
    [ContextMenu("Summon")]
    public override Point Request_Block()
    {
        Spawn_Block();

        _block.Set_Pos(_summonPos.position);
        _block.Move(this, () =>
        {
            _block.Check_Down(() => { });

            if (!Is_Block())
                Request_Block();
        });

        return null;
    }

    public void Test()
    {
        Spawn_Block();

        _block.Check_Down(() => { });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            Test();
    }
}
