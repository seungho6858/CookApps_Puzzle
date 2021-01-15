using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : Singleton_Awake<BlockManager>
{
    private readonly string blockPath = "Prefab/Puzzle/Polygon/Polygon_0";

    public Transform[] _parent;

    private void Awake()
    {
        Check(this);
    }

    public Block GetBlock(Block_Type type)
    {
        Block block = null;

        if (_parent[(int)type].childCount > 0)
            block = _parent[(int)type].GetChild(0).GetComponent<Block>();

        if(null == block)
            block = (Instantiate(Resources.Load($"{blockPath}{(int)type}"), _parent[(int)type]) as GameObject).GetComponent<Block>();

        block.SetActivate(true);

        return block;
    }

    public void ReturnBlock(Block block)
    {
        block._Tr.SetParent(_parent[(int)block._type]);
        block.SetActivate(false);
    }
}
