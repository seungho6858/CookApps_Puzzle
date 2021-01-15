using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Tools : MonoBehaviour
{
    [MenuItem("Tools/SpriteToImage")]
    public static void SpriteToImage()
    {
        GameObject obj = Selection.activeObject as GameObject;

        SpriteRenderer[] renderes = obj.GetComponentsInChildren<SpriteRenderer>();

        for(int i=0; i< renderes.Length; ++i)
        {
            Image image = renderes[i].gameObject.GetComponent<Image>();
            if(null == image) 
                image = renderes[i].gameObject.AddComponent<Image>();

            image.sprite = renderes[i].sprite;
            image.raycastTarget = false;

            DestroyImmediate(renderes[i]);
        }
    }
}
