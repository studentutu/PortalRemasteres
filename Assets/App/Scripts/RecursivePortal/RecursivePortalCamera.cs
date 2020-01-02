﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursivePortalCamera : MonoBehaviour
{
    [SerializeField] private Portal[] portals = new Portal[2];
    [SerializeField] private RenderTexture[] rendersTextures = new RenderTexture[2];

    [SerializeField] private Camera portalCamera;

    private RenderTexture tempTexture1;
    private RenderTexture tempTexture2;

    [SerializeField] private Camera mainCamera;

    private const int iterations = 7;

    private void Awake()
    {
        if(mainCamera == null)
        {
            mainCamera = GetComponent<Camera>();
        }
        tempTexture1 = rendersTextures[0];
        tempTexture2 = rendersTextures[1];

        // tempTexture1 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        // tempTexture2 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    }

    private void Start()
    {
        portals[0].SetTexture(tempTexture1);
        portals[1].SetTexture(tempTexture2);
    }

    private void OnPreRender()
    {
        if(portals[0].IsRendererVisible())
        {
            portalCamera.targetTexture = tempTexture1;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portals[0], portals[1], i);
            }
        }

        if(portals[1].IsRendererVisible())
        {
            portalCamera.targetTexture = tempTexture2;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portals[1], portals[0], i);
            }
        }
    }

    private void RenderCamera(Portal inPortal, Portal outPortal, int iterationID)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;

        Transform cameraTransform = portalCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        for(int i = 0; i <= iterationID; ++i)
        {
            // Position the camera behind the other portal.
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            // Rotate the camera to look through the other portal.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;
        }

        // Set the camera's oblique view frustum.
        Plane p = new Plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlane;

        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

        // Render the camera to its render target.
        portalCamera.Render();
    }
}