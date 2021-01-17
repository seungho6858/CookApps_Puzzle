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

    public bool Check_Explodes()
    {
        HashSet<Block> explodes = MapManager.instance.Get_Explodes();

        if (explodes.Count == 0)
            return false;

        StartCoroutine(Explode(explodes));

        return true;
    }

    private IEnumerator Explode(HashSet<Block> explodes)
    {
        List<Block> explodedByBomb = ExplodedByBomb(); // 폭탄에 의해 파괴되는 폭탄들
        for (int i = 0; i < explodedByBomb.Count; ++i)
            explodes.Add(explodedByBomb[i]);

        if (0 != explodedByBomb.Count)
            yield return new WaitForSeconds(0.5f);

        List<Point> points = new List<Point>();

        foreach (var block in explodes)
        {
            points.Add(block.Get_Point());
            block.Explode(); // 먼저 Block들을 회수시킨다
        }

        Create_Bomb(); // 특수 타일 (폭탄) 생성

        Check_Damage(); // 블록 파괴로 인해 주변 '공격' 받은 타일 체크 

        Request_Empty(points); // 회수된 Tile들의 빈자리를 채워준다

        void Request_Empty(List<Point> requests)
        {
            if (requests.Count == 0)
                return;

            List<Point> next = new List<Point>();

            for (int i = 0; i < requests.Count; ++i)
            {
                Point empty = requests[i].Request_Block();

                if (null != empty)
                    next.Add(empty);
            }

            Request_Empty(next);
        }

        void Check_Damage()
        {
            // 터진 블록들 중에서 주변에 '공격' 받을 블럭이 있었는지?
            HashSet<Block> spins = new HashSet<Block>();
            foreach (var block in explodes)
                Helper.Check_Damaged(ref spins, block);
            foreach (var spinBlock in spins)
            {
                if (spinBlock.Get_Damage()) // '공격' 받아서 파괴된 블럭
                    points.Add(spinBlock.Get_Point());
            }
        }

        void Create_Bomb()
        {
            if (explodes.Count < 4)
                return;

            foreach (var block in explodes)
            {
                if (block.GetType() == typeof(BombBlock))
                    return;
            }

            int bombIdx = Random.Range(0, explodes.Count);

            foreach (var block in explodes)
            {
                if (bombIdx == 0)
                {
                    block.Get_Point().Spawn_Block(Block_Type.BombBlock);
                    break;
                }
                bombIdx--;
            }
        }

        List<Block> ExplodedByBomb() // 공격받아서 터질 블럭들
        {
            List<Block> blockByBomb = new List<Block>();

            foreach (var block in explodes)
            {
                List<Block> temp = block.ExplodeOthers();
                if (null != temp)
                    blockByBomb.AddRange(temp);
            }

            return blockByBomb;
        }
    }

    public IEnumerator Hint()
    {
        Hide_Hint();

        yield return null;

        int rand = Random.Range(0, _listPoints.Count); // 매번 다른 순서로 힌트 검색

        for(int i = rand ; ; i++)
        {
            if (i >= _listPoints.Count)
                i = 0;

            if (i == rand - 1) // 한바퀴 다 돎
                break;

            Block block = _listPoints[i].Get_Block();

            Block.HintInfo hintInfo = block.Hint();

            if (null == hintInfo)
                continue;

            hintInfo.explosive.ForEach(x => x.Show_Lines(true));
            hintInfo.moveBlock.Move_Hint(hintInfo.moveDir);

            break;
        }

    }

    public void Hide_Hint()
    {
        _listPoints.ForEach(x => x.Get_Block().Show_Lines(false));
    }
}
