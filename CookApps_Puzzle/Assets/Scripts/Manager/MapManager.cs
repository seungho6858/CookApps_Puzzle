using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : Singleton_Awake<MapManager>
{
    public List<Point> _listPoints;

    public MaskableGraphic _block;

    private void Awake()
    {
        Check(this);
    }

    private void Start()
    {
        Global._gameState = Game_State.Ready;

        int[] spinBlock = Helper.Pick_Random(_listPoints.Count,6); // 맵에 6개는 Spin-Block
        
        while(true)
        {
            for (int i = 0; i < _listPoints.Count; ++i)
            {
                if (System.Array.Exists(spinBlock, x => x == i))
                    _listPoints[i].Spawn_Block(Block_Type.Spin);
                else
                    _listPoints[i].Spawn_Block(Helper.Get_RandBlock());
            }
            
            if (Get_Explodes().Count == 0)
                break;

            _listPoints.ForEach(x => x.Return_Block());
        }

        Global._gameState = Game_State.Playing;
    }

    public Point Get_Point(int pos)
    {
        return _listPoints[pos];
    }

    public void Restrict_Input(bool b)
    {
        _block.raycastTarget = b;
    }

    private HashSet<Block> Get_Explodes() // 현재 맵에서 터질 수 있는 Block들 체크 & Return
    {
        HashSet<Block> explodes = new HashSet<Block>();

        for (int i = _listPoints.Count - 1; i >= 0; --i)
        {
            Block block = _listPoints[i].Get_Block();

            List<Block> candidates = Helper.Get_Explosive(block);

            if(candidates.Count != 0)
            {
                for (int j = 0; j < candidates.Count; ++j)
                    explodes.Add(candidates[j]);
            }
        }

        return explodes;
    }

    public bool Check_Explodes()
    {
        HashSet<Block> explodes = MapManager.instance.Get_Explodes();

        if (explodes.Count == 0)
            return false;
        
        List<Point> points = new List<Point>();

        foreach (var block in explodes)
        {
            points.Add(block.Get_Point());
            block.Explode(); // 먼저 Block들을 회수시킨다
        }
        
        // 터진 블록들 중에서 주변에 '공격' 받을 블럭이 있었는지?
        HashSet<Block> spins = new HashSet<Block>();
        foreach (var block in explodes)
            Helper.Check_Damaged(ref spins, block);
        foreach (var spinBlock in spins)
        {
            if(spinBlock.Get_Damage()) // '공격' 받아서 파괴된 블럭
                points.Add(spinBlock.Get_Point());
        }
            

        Request(points);

        void Request(List<Point> requests)
        {
            if (requests.Count == 0)
                return;

            List<Point> next = new List<Point>();

            for (int i = 0; i < requests.Count; ++i) // 회수된 Tile들의 빈자리를 채워준다
            {
                Point empty = requests[i].Request_Block();

                if (null != empty)
                    next.Add(empty);
            }

            Request(next);
        }

        return true;
    }
}
