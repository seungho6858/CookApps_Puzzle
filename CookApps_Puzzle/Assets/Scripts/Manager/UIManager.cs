using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton_Awake<UIManager>
{
    public Popup_Confirm _popup;
    
    // Move
    public Text _tMove;
    private int _move;
    
    // Spin
    public Text _tSpin;
    private int _spin;
    
    private void Awake()
    {
        Check(this);
        
        _move = 15;
        _tMove.text = _move.ToString();

        _spin = 6;
        _tSpin.text = _spin.ToString();
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
            Show_Popup("게임오버", ReGame);
    }
    
    //

    public void Show_Popup(string text, System.Action callBack)
    {
        _popup.Set_Ui(text, callBack);
    }

    public void ReGame()
    {
        GameManager.instance.ReGame();
    }

    public void Get_Spin()
    {
        _spin--;
        _tSpin.text = _spin.ToString();

        if (_spin == 0)
        {
            Show_Popup("게임 클리어", ReGame);
        }
    }
}
