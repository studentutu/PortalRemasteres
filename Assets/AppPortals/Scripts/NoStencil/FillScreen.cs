// Copyright (C) Stanislaw Adaszewski, 2013
// http://algoholic.eu

using UnityEngine;
using System.Collections;
using System;

public class FillScreen : MonoBehaviour
{
    public Transform portal1;
    public Camera portal1Cam;

    public Transform portal2;
    public Camera portal2Cam;

    private Color portal1C = Color.blue;
    private Color portal2C = Color.red;

    // Update is called once per frame
    private void Update()
    {
        //sky.position = cam.transform.position;

        // Quaternion q = Quaternion.FromToRotation(-portal1.up, cam.transform.forward);
        // portal1Cam.transform.position = portal2.position + (cam.transform.position - portal1.position);
        // portal1Cam.transform.LookAt(portal1Cam.transform.position + q * portal2.up, portal2.transform.forward);
        // portal1Cam.nearClipPlane = (portal1Cam.transform.position - portal2.position).magnitude - 0.3f;

        // q = Quaternion.FromToRotation(-portal2.up, cam.transform.forward);
        // portal2Cam.transform.position = portal1.position + (cam.transform.position - portal2.position);
        // portal2Cam.transform.LookAt(portal2Cam.transform.position + q * portal1.up, portal1.transform.forward);
        // portal2Cam.nearClipPlane = (portal2Cam.transform.position - portal1.position).magnitude - 0.3f;

        var tuplePortal1 = new Tuple<Transform, Camera>(portal1, portal1Cam);
        var tuplePortal2 = new Tuple<Transform, Camera>(portal2, portal2Cam);

        RenderCamera(tuplePortal1, tuplePortal2);
        RenderCamera(tuplePortal2, tuplePortal1);

        // Vector3[] scrPoints = new Vector3[4];
        // scrPoints[0] = new Vector3(0, 0, 0.1f);
        // scrPoints[1] = new Vector3(1, 0, 0.1f);
        // scrPoints[2] = new Vector3(1, 1, 0.1f);
        // scrPoints[3] = new Vector3(0, 1, 0.1f);

        // for (int i = 0; i < scrPoints.Length; i++)
        // {
        //     scrPoints[i] = transform.worldToLocalMatrix.MultiplyPoint(cam.ViewportToWorldPoint(scrPoints[i]));
        // }

        // int[] tris = new int[6] { 0, 1, 2, 2, 3, 0 };

        // mf.mesh.Clear();
        // mf.mesh.vertices = scrPoints;
        // mf.mesh.triangles = tris;
        // mf.mesh.RecalculateBounds();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;

        var tuplePortal1 = new Tuple<Transform, Camera>(portal1, portal1Cam);
        var tuplePortal2 = new Tuple<Transform, Camera>(portal2, portal2Cam);

        RenderCamera(tuplePortal1, tuplePortal2);
        RenderCamera(tuplePortal2, tuplePortal1);
    }
    private void RenderCamera(Tuple<Transform, Camera> inPortal, Tuple<Transform, Camera> otherPortal)
    {
        if (otherPortal == null) return;

        Transform inTransform = inPortal.Item1.transform;
        Transform outTransform = otherPortal.Item1.transform;
        Transform outPortalCamera = otherPortal.Item2.transform;
        Camera Maincamera = Camera.main;

        // Position the camera behind the other portal.
        Vector3 relativePos = inTransform.InverseTransformPoint(Maincamera.transform.position);
        relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        outPortalCamera.position = outTransform.TransformPoint(relativePos);
        // Rotate the camera to look through the other portal.
        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * Maincamera.transform.rotation;
        relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        outPortalCamera.rotation = outTransform.rotation * relativeRot;


        #region  tests
        // Quaternion beforeOtherRot = otherPortal.forwardRevert.rotation;
        // Vector3 beforeOtherPos = otherPortal.forwardRevert.position;
        // Quaternion beforeInForwardRot = inPortal.forwardRevert.rotation;
        // Vector3 beforeInForwardPos = inPortal.forwardRevert.position;

        // var vectorCamToPortalWorld = inTransform.position +
        //     (inTransform.position - Maincamera.transform.position);
        // otherPortal.forwardRevert.localPosition = outTransform.InverseTransformPoint(vectorCamToPortalWorld);

        // outPortalCamera.LookAt(otherPortal.forwardRevert.position, outTransform.up);
        // Rotate the camera to look through the other portal.
        // Quaternion relativeRot = outPortalCamera.rotation;
        // Quaternion.LookRotation(
        //     otherPortal.forwardRevert.localPosition.normalized
        //     ,
        //     outTransform.up
        // );
        // Quaternion.LookRotation(otherPortal.forwardRevert.forward, otherPortal.forwardRevert.up);
        // relativeRot *= Quaternion.Inverse(inPortal.transform.rotation);
        // relativeRot *= Quaternion.Euler(0.0f, 180.0f, 0.0f);
        // relativeRot *= otherPortal.transform.rotation;
        // relativeRot *= Quaternion.Euler(Maincamera.transform.rotation.eulerAngles - relativeRot.eulerAngles);
        // outPortalCamera.rotation = relativeRot;
        #endregion


        // if (!Application.isPlaying)
        // {
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawLine(Maincamera.transform.position, vectorCamToPortalWorld);

        //     // Gizmos.DrawLine(outTransform.position, outTransform.position + outPortalCamera.forward*5);
        //     Gizmos.DrawLine(outTransform.position, outTransform.position + outPortalCamera.forward * 5);

        // }

        // otherPortal.forwardRevert.position = beforeOtherPos;
        // otherPortal.forwardRevert.rotation = beforeOtherRot;
        // inPortal.forwardRevert.position = beforeInForwardPos;
        // inPortal.forwardRevert.rotation = beforeInForwardRot;
        otherPortal.Item2.aspect = Maincamera.aspect;
        otherPortal.Item2.fieldOfView = Maincamera.fieldOfView;
        otherPortal.Item2.farClipPlane = Maincamera.farClipPlane;

        // Set the camera's oblique view frustum.
        Plane p = new Plane(-otherPortal.Item1.transform.forward, outTransform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(otherPortal.Item2.worldToCameraMatrix)) * clipPlane;
        // var previousClipping = Maincamera.nearClipPlane;
        // Maincamera.nearClipPlane = (otherPortal.Item2.transform.position - otherPortal.Item1.position).magnitude;
        var newMatrix = Maincamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        otherPortal.Item2.projectionMatrix = newMatrix;
        // otherPortal.Item2.projectionMatrix = Maincamera.projectionMatrix;
        // Maincamera.nearClipPlane  = previousClipping;
        otherPortal.Item2.nearClipPlane = (otherPortal.Item2.transform.position - otherPortal.Item1.position).magnitude;

        if (!Application.isPlaying)
        {
            if (inPortal.Item1 == portal1)
            {
                Gizmos.color = portal1C;
            }
            else
            {
                Gizmos.color = portal2C;
            }
            Gizmos.matrix = newMatrix;
            Gizmos.DrawFrustum(otherPortal.Item2.transform.position, otherPortal.Item2.fieldOfView, otherPortal.Item2.farClipPlane, otherPortal.Item2.nearClipPlane, otherPortal.Item2.aspect);

        }

        // Render the camera to its render target.
        // otherPortal.thisCam.Render();
    }
}
