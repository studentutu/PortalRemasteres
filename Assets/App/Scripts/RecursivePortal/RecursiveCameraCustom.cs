using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable

[RequireComponent(typeof(Camera))]
// [ExecuteAlways]
[ExecuteInEditMode]
public class RecursiveCameraCustom : MonoBehaviour
{
    [SerializeField] private Material matherialToUse = null;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, matherialToUse);
        if(Application.isPlaying) return;
        foreach (var item in GameObject.FindObjectsOfType<Portal>())
        {
            item.RenderPortal();
        }
        foreach (var item in Camera.allCameras)
        {
            if(item != Camera.main)
            {
                item.Render();
            }
        }
        // for (int i = 0; i < 5; i++)
        // {
        //     src = dest;
        // }
    }
}
