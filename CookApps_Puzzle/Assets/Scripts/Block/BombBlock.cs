using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombBlock : Block
{
    public Image _image;

    private void OnEnable()
    {
        _type = Helper.Get_RandBlock();

        switch (_type)
        {
            case Block_Type.Green:
                _image.color = Color.green;
                break;
            case Block_Type.Yellow:
                _image.color = Color.yellow;
                break;
            case Block_Type.Purple:
                _image.color = Color.magenta;
                break;
            case Block_Type.Blue:
                _image.color = Color.blue;
                break;
            case Block_Type.Red:
                _image.color = Color.red;
                break;
        }
    }

    public override List<Block> ExplodeOthers()
    {
        List<Block> explodes = new List<Block>();

        for (int i=0; i< (int) Dir.Max; ++i)
        {
            Block block = Get_Point().Get_NearBlock((Dir)i);
            if (null != block)
                explodes.Add(block);
        }

        explodes.ForEach( x => x.Show_Lines(true));

        return explodes;
    }

    public override void Return()
    {
        _type = Block_Type.BombBlock;

        base.Return();
    }
}
