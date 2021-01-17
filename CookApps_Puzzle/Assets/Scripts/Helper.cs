using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    private static readonly List<List<Dir>> _match4 = new List<List<Dir>>()
    {
        new List<Dir>() { Dir.LD, Dir.U, Dir.RU},
        new List<Dir>() { Dir.U, Dir.RD, Dir.D},
        new List<Dir>() { Dir.RD, Dir.LD, Dir.LU},
    };
    
    public static Block_Type Get_RandBlock()
    {
        return (Block_Type)Random.Range(0, (int)Block_Type.Red + 1);
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
    
    public static int[] Pick_Random(int length, int cnt)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < length; ++i)
            list.Add(i);

        int[] result = new int[cnt];

        for (int i=0; i< cnt; ++i)
        {
            int rand = Random.Range(0, list.Count);

            result[i] = list[rand];

            list.RemoveAt(rand);
        }

        return result;
    }

    public static bool Is_SpecialBlock(Block_Type type)
    {
        return type == Block_Type.Spin || type == Block_Type.BombBlock;
    }
    
    public static List<Block> Get_Explosive(Block block) // 특정 방향으로 3-Match, 4-Match
    {
        Block_Type type = block._type;

        List<Block> explodes = new List<Block>();
        
        if (Is_SpecialBlock(type))
            return explodes;

        for(int i=0; i<(int) Dir.Max; ++i)
        {
            // 3-Match
            List<Block> candidates = new List<Block>();
            
            Match_3((Dir)i, block, ref candidates);

            if (candidates.Count >= 3)
                explodes.AddRange(candidates);
        }
        
        // 4-Match
        {
            Match_4(block, ref explodes);
        }
        
        return explodes;

        void Match_3(Dir dir, Block _block, ref List<Block> candidates)
        {
            candidates.Add(_block);

            Point nearPoint = _block.Get_Point().Get_NearPoint(dir);

            if (null == nearPoint) // 해당 방향으로 블럭 x
                return;

            Block nearBlock = nearPoint.Get_Block();
            if (null == nearBlock)
                return;

            if (nearBlock._type == type) // 같은 타입!
                Match_3(dir, nearBlock, ref candidates);
            else
                return;
        }

        void Match_4(Block _block, ref List<Block> candidates)
        {
            for (int i = 0; i < _match4.Count; ++i)
            {
                List<Block> blocks = new List<Block>();

                Find_4(_match4[i], 0, _block, ref blocks);

                if (blocks.Count >= 3) // 4_match 가 성공하면, 주변에 맞닿은 모든 블럭도 포함이 된다
                {
                    Find_All(_block, ref candidates);
                }
            }

            void Find_4(List<Dir> dirs, int idx, Block standardBlock, ref List<Block> blocks)
            {
                if (dirs.Count <= idx)
                    return;
                
                Dir dir = dirs[idx];

                Point nearPoint = standardBlock.Get_Point().Get_NearPoint(dir);
                    
                if (null == nearPoint) // 해당 방향으로 블럭 x
                    return;

                Block nearBlock = nearPoint.Get_Block();
                if (nearBlock._type != type)
                    return;
                
                blocks.Add(nearBlock);

                Find_4(dirs, idx + 1, nearBlock, ref blocks);
            }

            void Find_All(Block startBlock, ref List<Block> blocks)
            {
                blocks.Add(startBlock);
                
                for (int i = 0; i < (int) Dir.Max; ++i)
                {
                    Point nearPoint = _block.Get_Point().Get_NearPoint((Dir)i);
                    
                    if(null == nearPoint)
                        continue;

                    Block nearBlock = nearPoint.Get_Block();
                    if (nearBlock._type != type)
                        continue;

                    if (blocks.Contains(nearBlock))
                        continue;

                    blocks.Add(startBlock);

                    Find_All(nearBlock, ref blocks);
                }
            }
        }

        bool Check_Same(Block standardBlock, Dir dir)
        {
            Point nearPoint = standardBlock.Get_Point().Get_NearPoint(dir);

            if (null == nearPoint)
                return false;

            return nearPoint.Get_Block()._type == type;
        }
    }

    public static void Check_Damaged(ref HashSet<Block> spins, Block block)
    {
        for (int i = 0; i < (int) Dir.Max; ++i)
        {
            Point nearPoint = block.Get_Point().Get_NearPoint((Dir)i);
                    
            if (null == nearPoint) // 해당 방향으로 블럭 x
                continue;

            Block nearBlock = nearPoint.Get_Block();
            if (null == nearBlock)
                continue;
            
            if (nearBlock._type == Block_Type.Spin)
                spins.Add(nearBlock);
        }
    }
}

