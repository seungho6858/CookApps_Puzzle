using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public static Block_Type Get_RandBlock()
    {
        return (Block_Type)Random.Range(0, (int)Block_Type.Max);
    }

    public static Dir Get_Dir(float angle)
    {
        if (angle >= 30f && angle < 90f)
            return Dir.RU;

        else if (angle >= 90f && angle < 150f)
            return Dir.RD;

        else if (angle >= 150f && angle < 210f)
            return Dir.D;

        else if (angle >= 210f && angle < 270f)
            return Dir.LD;

        else if (angle >= 270f && angle < 330f)
            return Dir.LU;

        else
            return Dir.U;
    }
    
    public static List<Block> Get_Explosive(Block block) // 특정 방향으로 3-Match
    {
        Block_Type type = block._type;

        List<Block> explodes = new List<Block>();

        for(int i=0; i<(int) Dir.Max; ++i)
        {
            List<Block> candidates = new List<Block>();

            Check((Dir)i, block, ref candidates);

            if (candidates.Count >= 3)
                explodes.AddRange(candidates);
        }

        return explodes;

        void Check(Dir dir, Block _block, ref List<Block> candidates)
        {
            candidates.Add(_block);

            Point nearPoint = _block.Get_Point().Get_NearPoint(dir);

            if (null == nearPoint) // 해당 방향으로 블럭 x
                return;

            Block nearBlock = nearPoint.Get_Block();

            if (nearBlock._type == type) // 같은 타입!
                Check(dir, nearBlock, ref candidates);
            else
                return;
        }
    }
}

