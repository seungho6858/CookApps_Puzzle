using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : BaseComponent
{
    public GameObject _active;

    public Block_Type _type;
    public float _dragLength;
    public bool _isMoving;

    [SerializeField] private Point _point;
    private Vector3 _vDragInit;
    private bool _dragging;

    public void Set_Point(Point point)
    {
        _point = point;
    }
    public Point Get_Point()
    {
        return _point;
    }

    public void Explode()
    {
        _point.Return_Block();
    }

    public void BeginDrag()
    {
        _vDragInit = Get_Pos();
        _dragging = true;
    }

    public void Drag()
    {
        Vector3 vDir = Input.mousePosition - _vDragInit;

        if(_dragging && vDir.magnitude > _dragLength)
        {
            float angle = Mathf.Atan2(vDir.normalized.x, vDir.normalized.y) * Mathf.Rad2Deg;

            if (angle < 0)
                angle += 360f;

            Dir dir = Helper.Get_Dir(angle);

            Point nearPoint = _point.Get_NearPoint(dir); // 이동할 Point

            if(null != nearPoint)
            {
                _dragging = false;

                StartCoroutine( Change_Block(nearPoint));
            }
        }
    }

    // 두 블럭 교체
    private IEnumerator Change_Block(Point nearPoint)
    {
        MapManager.instance.Restrict_Input(true); // user input

        Point originPoint = _point;

        int finish = 0;

        this.Move(nearPoint, () => finish++); ; // 내 블럭
        nearPoint.Get_Block().Move(_point, () => finish++); // 상대 블럭

        //
        yield return new WaitUntil(() => finish == 2);
        //

        HashSet<Block> explodes = MapManager.instance.Get_Explodes();

        if (explodes.Count == 0) // 실패!
        {
            finish = 0;
            nearPoint = originPoint;

            this.Move(nearPoint, () => finish++); ; // 내 블럭
            nearPoint.Get_Block().Move(_point, () => finish++); // 상대 블럭

            yield return new WaitUntil(() => finish == 2);

            MapManager.instance.Restrict_Input(false); // user input
        }
        else // 성공
        {
            List<Point> points = new List<Point>();

            foreach (var block in explodes)
            {
                points.Add(block.Get_Point());
                block.Explode(); // 먼저 Block들을 회수시킨다
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
                    Set_Point(null);

                    if (null != empty)
                        next.Add(empty);
                }

                Request(next);
            }

            
        }
    }

    public void Move(Point nearPoint, System.Action finish) // 이동 by hand
    {
        Vector3 vDir = nearPoint.Get_Pos() - Get_Pos();

        StartCoroutine(Move(vDir, nearPoint.Get_Pos(),
            () => 
            {
                nearPoint.Set_Block(this);

                finish.Invoke();
            }));
    }

    private IEnumerator Move(Vector3 vDir, Vector3 vEnd, System.Action finish)
    {
        float time = 0.3f; // 이동 시간

        _isMoving = true;
        vDir /= time;

        while (true)
        {
            time -= Time.deltaTime;
            if (time <= 0f)
                break;

            _Tr.Translate(vDir * Time.deltaTime);

            yield return null;
        }

        _Tr.position = vEnd;
        _isMoving = false;

        finish.Invoke();
    }

    // 아래로 내려가줄지 판단
    public void Check_Down(System.Action finish)
    {
        List<Dir> candidates = new List<Dir>() { Dir.D }; // 바로 아래부터 체크
        candidates.AddRange(Random.Range(0, 2) == 0 ? new List<Dir>() { Dir.RD, Dir.LD } : new List<Dir>() { Dir.LD, Dir.RD }); // 좌, 우 체크는 랜덤

        for (int i = 0; i < candidates.Count; ++i)
        {
            Near_State state = Get_Point().Get_NearState(candidates[i]);

            if (state == Near_State.Block_NonExist)
            {
                Point point = Get_Point().Get_NearPoint(candidates[i]); // 내려갈 곳이 있다!

                if(candidates[i] != Dir.D) // 좌,우 이동인 경우 내 머리위에 블럭이 없어야함 + 이동할 블럭 머리위에서 내려올 수 있는 기회를 뺏으면 안됨
                {
                    Near_State candidateState = Get_Point().Get_NearState(Dir.U);

                    if (candidateState == Near_State.Block_Exist) // 내 위에 블럭이 있으면 내려가지 못한다
                        continue;

                    candidateState = point.Get_NearState(Dir.U);

                    if (candidateState == Near_State.Block_Exist || candidateState == Near_State.Block_Moving)
                        continue;
                }

                Go_Down(point, finish);
                break;
            }
        }

        finish.Invoke(); // 아래로 갈 수 없음
    }

    // 밑으로 내려가자
    public void Go_Down(Point dstPoint, System.Action finish) // 이동 by AI
    {
        Point srcPoint = Get_Point();

        Debug.Log(" : " + srcPoint.name + " -> " + dstPoint);

        Vector3 vDir = dstPoint.Get_Pos() - Get_Pos();

        dstPoint.Set_Block(this); // 내 자리임을 선언

        srcPoint.Relase_Block();
        //srcPoint.Request_Block(); // 내가 가버리면서 빈 블럭을 채운다

        StartCoroutine(Move(vDir, dstPoint.Get_Pos(),
            () =>
            {
                Check_Down(finish); // 더 내려갈 수 있는지 판단
            }));
    }
}
