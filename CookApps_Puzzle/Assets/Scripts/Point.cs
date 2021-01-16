using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Point : BaseComponent
{
    public List<NearPoint> _nearPoints;
    public int _pos;

    [SerializeField] private Transform[] _points;

    [SerializeField] private Block _block;

    public Transform _trBlock;
    public Text _text;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        _text.text = _pos.ToString();
    }

    [ContextMenu("Spawn")]
    public void Spawn_Block()
    {
        Set_Block(BlockManager.instance.GetBlock(Helper.Get_RandBlock()));
        _block.Set_Pos(_Tr.position).SetParent(_trBlock);
    }

    public void Set_Block(Block block)
    {
        _block = block;
        _block.Set_Point(this);
    }

    public void Relase_Block()
    {
        _block = null;
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

    public bool Is_Block() // 현재 블럭 없는지
    {
        return _block != null;
    }

    public Near_State Get_NearState(Dir dir) // 주변 Tile의 상태
    {
        Point nearPoint = Get_NearPoint(dir);

        if (null == nearPoint)
            return Near_State.Empty;

        Block block = nearPoint.Get_Block();

        if (null == block)
            return Near_State.Block_NonExist;
        else
            return block._isMoving ? Near_State.Block_Moving : Near_State.Block_Exist;
    }

    public Point Request_Block() // 나의 빈자리를 채워줄 블럭 (Return : Empty가 된 타일)
    {
        if (Global._gameState != Game_State.Playing)
            return null;
        if (Is_Block())
            return null;

        List<Dir> candidates = new List<Dir>() { Dir.U }; // 블럭 뺏어올 타일 후보들
        candidates.AddRange(Random.Range(0, 2) == 0 ? new List<Dir>() { Dir.RU, Dir.LU } : new List<Dir>() { Dir.LU, Dir.RU });

        for(int i=0; i<candidates.Count; ++i)
        {
            Near_State state = Get_NearState(candidates[i]);

            if (Near_State.Block_Exist == state)
            {
                Point point = Get_NearPoint(candidates[i]); // 후보 선정

                if (candidates[i] != Dir.U) // 왼,오른쪽 블럭인 경우엔 머리 위에 블럭이 없어야함 + 아래로 내려갈 수 있는 경우면 그 기회를 빼앗으면 안됨
                {
                    // 위 상태 확인
                    Near_State candidateState = point.Get_NearState(Dir.U); 

                    if (candidateState == Near_State.Block_Exist || candidateState == Near_State.Block_Moving)
                        continue;

                    // 후보의 아래 상태 확인
                    candidateState = point.Get_NearState(Dir.D); 

                    if (candidateState == Near_State.Block_NonExist)
                        continue;
                }

                point.Get_Block().Go_Down(this, () => { });

                //point.Request_Block(); // 빼앗긴 타일도 블럭을 채워주자

                return point;
            }
        }

        return null;
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
