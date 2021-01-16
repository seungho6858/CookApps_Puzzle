using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Popup_Confirm _popup;
    
    public Text _tMove;
    private int _move;
    
    private void Awake()
    {
        _move = 20;
        _tMove.text = _move.ToString();
    }

    private void OnEnable()
    {
        Block._blockMoved += MoveBlock;
    }

    private void OnDisable()
    {
        Block._blockMoved -= MoveBlock;
    }

    private void MoveBlock(Block block)
    {
        _move--;
        _tMove.text = _move.ToString();
        
        if(_move == 0)
            Show_Popup("게임오버", GameManager.instance.ReGame);
    }
    
    //

    public void Show_Popup(string text, System.Action callBack)
    {
        _popup.Set_Ui(text, callBack);
    }
}
