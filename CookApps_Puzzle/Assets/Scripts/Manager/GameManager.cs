using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton_Awake<GameManager>
{
    private void Awake()
    {
        Check(this);

        Input.multiTouchEnabled = false;
    }

    [ContextMenu("ReGame")]
    public void ReGame()
    {
        SceneManager.LoadScene("InGame");
    }
}
