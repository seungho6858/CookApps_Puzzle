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
        while(true)
        {
            _listPoints.ForEach(x => x.Spawn_Block());

            if (Get_Explodes().Count == 0)
                break;

            _listPoints.ForEach(x => x.Return_Block());
        }
    }

    public Point Get_Point(int pos)
    {
        return _listPoints[pos];
    }

    public void Restrict_Input(bool b)
    {
        _block.raycastTarget = b;
    }

    public List<Block> Get_Explodes()
    {
        List<Block> explodes = new List<Block>();

        for (int i = 0; i < _listPoints.Count; ++i)
        {
            Block block = _listPoints[i].Get_Block();

            explodes.AddRange(Helper.Get_Explosive(block));
        }

        return explodes;
    }

    public void Go_Down(System.Action finish)
    {
        StartCoroutine(Check_Down(finish));
    }

    public IEnumerator Check_Down(System.Action finish) // 빈 Tile 들을 채워준다
    {
        int cnt = 0;
        
        for (int i = 0; i < _listPoints.Count; ++i)
        {
            if(!_listPoints[i].Is_Block())
            {
                Point help = _listPoints[i].Request_Down(); // 도와줄 Tile

                cnt++;
                help.Block_Down(_listPoints[i], () => cnt--);
            }
        }

        yield return new WaitUntil(() => cnt == 0);

        finish.Invoke();
    }
}
