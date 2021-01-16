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

        while(true)
        {
            _listPoints.ForEach(x => x.Spawn_Block());

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

    public HashSet<Block> Get_Explodes() // 현재 맵에서 터질 수 있는 Block들 체크 & Return
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
}
