using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton_Awake<MapManager>
{
    public List<Point> _listPoints;

    private void Awake()
    {
        Check(this);
    }

    public Point Get_Point(int pos)
    {
        return _listPoints[pos];
    }
}
