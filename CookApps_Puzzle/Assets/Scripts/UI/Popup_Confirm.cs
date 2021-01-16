using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Confirm : BaseComponent
{
    public Text _text;
    public System.Action _callBack;

    public void Set_Ui(string text, System.Action callBack)
    {
        _text.text = text;
        _callBack = callBack;
        
        SetActivate(true);
    }
    
    public void OnClick_Confirm()
    {
        _callBack.Invoke();
        
        SetActivate(false);
    }
}
