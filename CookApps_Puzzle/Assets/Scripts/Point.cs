using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : BaseComponent
{
    public List<NearPoint> _nearPoints;
    public int _pos;

    [SerializeField] private Transform[] _points;

    [SerializeField] private Block _block;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
    }

    [ContextMenu("Spawn")]
    public void Spawn_Block()
    {
        Set_Block(BlockManager.instance.GetBlock(Helper.Get_RandBlock()));
        _block.Set_Pos(_Tr.position).SetParent(_Tr);
    }

    public void Set_Block(Block block)
    {
        _block = block;
        _block.Set_Point(this);
    }

    [ContextMenu("Return")]
    public void Return_Block()
    {
        if(null != _block)
        {
            BlockManager.instance.ReturnBlock(_block);
            _block = null;
        }
    }

    public Point Get_NearPoint(Dir dir)
    {
        NearPoint near = _nearPoints.Find(x => x.dir == dir);

        if(null != near)
            return near.point;
        return null;
    }
    public Block Get_NearBlock(Dir dir)
    {
        return Get_NearPoint(dir)._block;
    }
    public Block Get_Block()
    {
        return _block;
    }

    public bool Is_Block()
    {
        return _block != null;
    }

    public Point Request_Down() // 현재 내가 Block이 없으니 위에서 달라고 요청
    {
        Dir dir = Dir.Max;

        if (Check(Dir.U)) 
            dir = Dir.U;
        else
        {
            Point point = Get_NearPoint(Dir.U);

            if(null == point) // 위 Tile이 없는 경우, 좌우에서 가져오자
            {
                if (Random.Range(0, 2) == 0)
                {
                    if (Check(Dir.LU)) dir = Dir.LU;
                    else if (Check(Dir.RU)) dir = Dir.RU;
                }
                else
                {
                    if (Check(Dir.RU)) dir = Dir.RU;
                    else if (Check(Dir.LU)) dir = Dir.LU;
                }
            }
        }

        return Get_NearPoint(dir);

        bool Check(Dir _dir)
        {
            Point point = Get_NearPoint(_dir);
            if (null == point)
                return false;

            return point.Is_Block();
        }
    }

    public void Block_Down(Point empty, System.Action finish)
    {
        Block block = Get_Block();

        if(null == block)
        {
            finish.Invoke();
            return;
        }
        else
            block.Check_Down(empty, finish);
    }

    [System.Serializable]
    public class NearPoint
    {
        public string name;

        public Point point = null;
        public Dir dir;
    }

#if UNITY_EDITOR
    [ContextMenu("Near")]
    public void Near()
    {
        List<Point> listPoints = GameObject.Find("Map").GetComponent<MapManager>()._listPoints;

        _pos = listPoints.FindIndex(x => x == this);

        _nearPoints.Clear();

        GetComponent<PolygonCollider2D>().enabled = false;

        for (int i = 0; i < (int) Dir.Max; ++i)
        {
            Dir dir = (Dir)i;

            NearPoint near = new NearPoint();
            near.dir = dir;
            near.name = dir.ToString();

            RaycastHit2D hit = Physics2D.Raycast(Get_Pos(), _points[i].position - Get_Pos());
            
            if(null != hit.collider)
                near.point = hit.collider.GetComponent<Point>();

            _nearPoints.Add(near);
        }

        GetComponent<PolygonCollider2D>().enabled = true;
    }

    [ContextMenu("TestSpawn")]
    public void TestNear()
    {
        for (int i = 0; i < _nearPoints.Count; ++i)
        {
            if (_nearPoints[i].point != null)
                _nearPoints[i].point.Spawn_Block();
        }
            
    }
#endif

}
