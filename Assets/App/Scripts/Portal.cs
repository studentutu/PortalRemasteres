using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 

[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{
    [SerializeField] private bool Recursve = false;
    [SerializeField] private Camera thisCam;
    [SerializeField] private Portal otherPortal;

    [SerializeField] private Renderer outlineRenderer;

    [SerializeField] private Color portalColour;

    private List<Rigidbody> rigidbodies = new List<Rigidbody>();

    private Material material;
    [SerializeField] private new Renderer renderer;
    [SerializeField] private new BoxCollider collider;

    private Matrix4x4 normalProjectoion;
    private void Awake()
    {
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
        SetColour(portalColour);
    }

    private void OnDrawGizmosSelected()
    {
        // Debug.LogWarning(" Secelted!!!!!!!!");
        if(Recursve)
        {
            // Gizmos.DrawFrustum(Vector3.zero,thisCam.fieldOfView, thisCam.farClipPlane,thisCam);
            RenderCamera(this, otherPortal, 0);
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

        if (Recursve)
        {
            RenderCamera(this, otherPortal, 0);
        }
    }

    private void RenderCamera(Portal inPortal, Portal outPortal, int iterationID)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;

        Transform cameraTransform = outPortal.thisCam.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        // for (int i = 0; i <= iterationID; ++i)
        {
            // Position the camera behind the other portal.
            Vector3 relativePos = inTransform.InverseTransformPoint(Camera.main.transform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);
            

            // Rotate the camera to look through the other portal.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;
        }

        otherPortal.thisCam.fieldOfView =  Camera.main.fieldOfView;

                // Set the camera's oblique view frustum.
        // Plane p = new Plane(-outTransform.forward, outTransform.position);
        // Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        // Vector4 clipPlaneCameraSpace =
        //     Matrix4x4.Transpose(Matrix4x4.Inverse(otherPortal.thisCam.worldToCameraMatrix)) * clipPlane;

        // var newMatrix = Camera.main.CalculateObliqueMatrix(clipPlaneCameraSpace);
        // otherPortal.thisCam.projectionMatrix = newMatrix;

        otherPortal.thisCam.nearClipPlane = cameraTransform.localPosition.magnitude;
        // Render the camera to its render target.
        // thisCam.Render();
    }

    public void SetColour(Color colour)
    {
        material.SetColor("_Colour", colour);
        outlineRenderer.material.SetColor("_OutlineColour", colour);
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
