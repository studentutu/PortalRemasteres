﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 

[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{
    [SerializeField] private bool Recursve = false;
    [SerializeField] private Transform forwardRevert = null;
    [SerializeField] private Camera thisCam;
    [SerializeField] private Portal otherPortal;
    [SerializeField] private Renderer outlineRenderer;
    private List<Rigidbody> rigidbodies = new List<Rigidbody>();
    private Material material;
    [SerializeField] private new Renderer renderer;
    [SerializeField] private new BoxCollider collider;

    private void Awake()
    {
        material = null;
        if (renderer == null)
        {
            collider = GetComponent<BoxCollider>();
            renderer = GetComponent<Renderer>();
        }
        material = renderer.material;
    }

    private void Start()
    {
        var diameter = PlayerController.instance.GetColliderRadius() * 3.0f;
        collider.size = new Vector3(collider.size.x - diameter,
            collider.size.y - diameter, collider.size.z);

        PlacePortal(transform.position, transform.forward, transform.up);
        // SetColour(portalColour);
    }

    private void OnDrawGizmosSelected()
    {
        // Debug.LogWarning(" Secelted!!!!!!!!");
        if (Recursve && !Application.isPlaying)
        {
            material = null;
            // Gizmos.DrawFrustum(Vector3.zero,thisCam.fieldOfView, thisCam.farClipPlane,thisCam);
            RenderCamera(this);
        }
    }

    private void Update()
    {
        for (int i = 0; i < rigidbodies.Count; ++i)
        {
            Vector3 objPos = transform.InverseTransformPoint(rigidbodies[i].position);

            if (objPos.z > 0.0f)
            {
                Warp(rigidbodies[i]);
            }
        }

        if (Recursve && renderer.isVisible)
        {
            RenderCamera(this);
        }
    }
    public void RenderPortal()
    {
        RenderCamera(this);
    }

    private void RenderCamera(Portal inPortal)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = otherPortal.transform;
        Transform outPortalCamera = otherPortal.thisCam.transform;
        Camera Maincamera = RecursiveCameraCustom.Instance.MyCam;


        // Position the camera behind the other portal.
        Vector3 relativePos = inTransform.InverseTransformPoint(Maincamera.transform.position);
        // relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
        outPortalCamera.position = outTransform.TransformPoint(relativePos);
        outPortalCamera.position -= otherPortal.forwardRevert.forward * outPortalCamera.localPosition.magnitude * 2;
        // outPortalCamera.RotateAround(outTransform.position, inTransform.up, 180);

        Quaternion beforeOtherRot = otherPortal.forwardRevert.rotation;
        Vector3 beforeOtherPos = otherPortal.forwardRevert.position;

        Quaternion beforeInForwardRot = inPortal.forwardRevert.rotation;
        Vector3 beforeInForwardPos = inPortal.forwardRevert.position;

        // outPortalCamera.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

        // otherPortal.forwardRevert.rotation = Camera.main.transform.rotation;
        // otherPortal.forwardRevert.position = Camera.main.transform.position;

        // Rotate the camera to look through the other portal.
        // Quaternion relativeRot = otherPortal.forwardRevert.localRotation;
        // // Quaternion.Inverse(inTransform.rotation) * Camera.main.transform.rotation;

        // 1. SceneView Mask worldScale , DislocatedimiteraImagetarget worldSclae
        // 2. ArMainCamera pos, loc postion to DislocatedimiteraImagetarget


        // var localPoint = otherPortal.forwardRevert;
        // localPoint.position = Maincamera.transform.position;
        // localPoint.rotation = Maincamera.transform.rotation; // need in order to have a correct rotation
        // outPortalCamera.localRotation = Quaternion.Euler(0,180 - localPoint.localRotation.eulerAngles.y ,0) ;
        // outPortalCamera.localRotation = localPoint.localRotation; // * inTransform.rotation;


        // 2. Vuforia/ArFoundation Camera Rotates around the image target!
        // 3. Multiply with Rotation of the Target Root Object!
        // Quaternion arCamInWorld = 
        //                     // outTransform.rotation *
        //                     // Quaternion.Euler(0f, 180.0f, 0f) *
        //                     Quaternion.Inverse(inTransform.rotation) // both are rotating!
        //                     * Maincamera.transform.rotation; // both are rotating!

        // Vector3 arCamInWorldPos = outTransform.TransformPoint( localPoint.transform.localPosition);
        // outPortalCamera.SetPositionAndRotation(arCamInWorldPos, arCamInWorld);

        // outPortalCamera.RotateAround(outTransform.position, outTransform.up, outTransform.rotation.eulerAngles.y);
        // outPortalCamera.localRotation *= beforeOtherRot; 

        // Rotate the camera to look through the other portal.
        // Quaternion relativeRot = Quaternion.Inverse(inPortal.transform.rotation) * outPortalCamera.rotation;
        // relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
        // outPortalCamera.rotation = otherPortal.forwardRevert.rotation * relativeRot;

        // float angularDifferenceBetweenPortalRotations = Quaternion.Angle(inTransform.rotation, outTransform.rotation);

        // Quaternion portalRotationalDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, inTransform.up);
        // Vector3 newCameraDirection = portalRotationalDifference * inTransform.forward;
        // outPortalCamera.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);

        otherPortal.forwardRevert.position = beforeOtherPos;
        otherPortal.forwardRevert.rotation = beforeOtherRot;

        inPortal.forwardRevert.position = beforeInForwardPos;
        inPortal.forwardRevert.rotation = beforeInForwardRot;

        // Set the camera's oblique view frustum.
        Plane p = new Plane(otherPortal.forwardRevert.forward, outTransform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(otherPortal.thisCam.worldToCameraMatrix)) * clipPlane;

        var newMatrix = Maincamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        otherPortal.thisCam.projectionMatrix = newMatrix;

        // otherPortal.thisCam.nearClipPlane = cameraTransform.localPosition.magnitude;

        // Render the camera to its render target.
        otherPortal.thisCam.Render();
    }

    /// <summary>
    /// Takes a point in the coordinate space specified 
    /// by the "from" transform and transforms it to be the correct
    /// point in the coordinate space specified by the "to" 
    /// transform applies rotation, scale and translation.
    /// </summary>
    /// <returns>Point to.</returns>
    public static Vector3 TransformPointFromTo(Transform from, Transform to, Vector3 fromPoint)
    {
        Vector3 worldPoint = (from == null) ? fromPoint : from.InverseTransformPoint(fromPoint);
        return (to == null) ? worldPoint : to.TransformPoint(worldPoint);
    }


    public void SetMaskID(int id)
    {
        material.SetInt("_MaskID", id);
    }

    public void SetTexture(RenderTexture tex)
    {
        if (material == null)
        {
            Awake();
            Start();
        }
        material.mainTexture = tex;
    }

    public bool IsRendererVisible()
    {
        return renderer.isVisible;
    }

    private void Warp(Rigidbody warpObj)
    {
        Vector3 pos = transform.InverseTransformPoint(warpObj.position);
        Vector3 upDir = transform.InverseTransformDirection(warpObj.transform.up);
        Vector3 fwdDir = transform.InverseTransformDirection(warpObj.transform.forward);

        pos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * pos;

        warpObj.transform.position = otherPortal.transform.TransformPoint(pos);

        var newUpDir = otherPortal.transform.TransformDirection(upDir);
        var newFwdDir = otherPortal.transform.TransformDirection(fwdDir);
        Quaternion lookRot = Quaternion.LookRotation(newFwdDir, newUpDir);

        warpObj.transform.rotation = Quaternion.Euler(0, 180, 0) * lookRot;

        var velocity = transform.InverseTransformDirection(warpObj.velocity);
        velocity = Quaternion.Euler(0.0f, 180.0f, 0.0f) * velocity;
        velocity = otherPortal.transform.TransformDirection(velocity);

        warpObj.velocity = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        var otherRigidbody = other.GetComponent<Rigidbody>();
        if (otherRigidbody != null)
        {
            rigidbodies.Add(otherRigidbody);

            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ChangePortalTriggerCount(1);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var otherRigidbody = other.GetComponent<Rigidbody>();

        if (rigidbodies.Contains(otherRigidbody))
        {
            rigidbodies.Remove(otherRigidbody);

            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ChangePortalTriggerCount(-1);
            }
        }
    }

    private void PlacePortal(Vector3 pos, Vector3 hitNormal, Vector3 up)
    {
        transform.position = pos;
        transform.rotation = Quaternion.LookRotation(hitNormal, up);
        transform.position -= transform.forward * 0.001f;
    }
}
