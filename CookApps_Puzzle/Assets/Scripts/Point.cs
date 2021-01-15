using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : BaseComponent
{
    public List<NearPoint> _nearPoints;
    public int _pos;

    private Block _block;

    [ContextMenu("Spawn")]
    public void Spawn_Block()
    {
        _block = BlockManager.instance.GetBlock(Block_Type.Green);
        _block.Set_Pos(_Tr.position).SetParent(_Tr);
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

    public bool Is_Block()
    {
        return _block != null;
    }

    [System.Serializable]
    public class NearPoint
    {
        public string name;

        public Point point = null;
        public Dir dir;
    }

    /*
#if UNITY_EDITOR
    [ContextMenu("Adjacent")]
    public void Adjacent()
    {
        List<Point> listPoints = GameObject.Find("Points").GetComponent<MapManager>()._listPoints;

        _pos = listPoints.FindIndex(x => x == this);

        _nearPoints.Clear();

        for (int i = 0; i < (int) Dir.Max; ++i)
            _nearPoints.Add(FindPoint((Dir)i));

        NearPoint FindPoint(Dir dir)
        {
            NearPoint near = new NearPoint();
            near.dir = dir;
            near.name = dir.ToString();

            int idx = _pos;

            switch(dir)
            {
                case Dir.LU:
                    if (_pos % 7 == 0) // 가장 왼쪽
                        return null;
                    idx -= 4;
                    break;
                case Dir.U:
                    idx -= 7;
                    break;
                case Dir.RU:
                    if ((_pos - 3) % 7 == 0) // 가장 오른쪽
                        return null;
                    idx -= 3;
                    break;
                case Dir.RD:
                    if ((_pos - 3) % 7 == 0) // 가장 오른쪽
                        return null;
                    idx += 4;
                    break;
                case Dir.D:
                    idx += 7;
                    break;
                case Dir.LD:
                    if (_pos % 7 == 0) // 가장 왼쪽
                        return null;
                    idx += 3;
                    break;
            }

            if(idx >= 0 && idx < listPoints.Count)
                near.point = listPoints[idx];

            return near;
        }
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
    */

}
