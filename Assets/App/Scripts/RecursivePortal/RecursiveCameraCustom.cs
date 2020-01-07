using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursiveCameraCustom : MonoBehaviour
{
    [SerializeField] private Material matherialToUse = null;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, matherialToUse);
    }
}
