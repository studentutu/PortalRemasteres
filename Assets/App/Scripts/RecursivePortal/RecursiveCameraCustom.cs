using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable

[RequireComponent(typeof(Camera))]
// [ExecuteAlways]
[ExecuteInEditMode]
public class RecursiveCameraCustom : MonoBehaviour
{
    private static RecursiveCameraCustom _insatnce = null;
    public static RecursiveCameraCustom Instance
    {
        get
        {

            if (_insatnce == null)
            {
                _insatnce = FindObjectOfType<RecursiveCameraCustom>();
            }
            return _insatnce;
        }
    }
    [SerializeField] private Material matherialToUse = null;
    [SerializeField] private Material matherialToUse2 = null;
    [SerializeField] private Transform localPoint = null;
    [SerializeField] private Camera myCam = null;
    public Camera MyCam
    {
        get
        {
            if (myCam == null)
            {
                myCam = GetComponent<Camera>();
            }
            return myCam;
        }
    }
    public Transform LocalPoint => localPoint;


    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, matherialToUse);
        if (matherialToUse2 != null)
            Graphics.Blit(dest, dest, matherialToUse2);


        // if(Application.isPlaying) return;
        // foreach (var item in GameObject.FindObjectsOfType<Portal>())
        // {
        //     item.RenderPortal();
        // }
        // foreach (var item in Camera.allCameras)
        // {
        //     if(item != Camera.main)
        //     {
        //         item.Render();
        //     }
        // }
        // for (int i = 0; i < 5; i++)
        // {
        //     src = dest;
        // }
    }
}
