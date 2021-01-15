using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : BaseComponent
{
    public GameObject _active;

    public Block_Type _type;
    public float _dragLength;

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

        List<Block> explodes = MapManager.instance.Get_Explodes();

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
            explodes.ForEach(x => x.Explode());

            MapManager.instance.Go_Down( () => 
            {
                MapManager.instance.Restrict_Input(false); // user input
            });
        }
    }

    public void Move(Point nearPoint, System.Action finish) // 이동
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
        float time = 0.2f; // 이동 시간

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

        finish.Invoke();
    }

    // 밑으로 내려가자
    public void Check_Down(Point point, System.Action finish) 
    {
        Move(point, finish);
    }

}
