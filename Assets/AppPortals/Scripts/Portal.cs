using System.Collections;
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

    private void RenderCamera(Portal inPortal)
    {
        if (otherPortal == null) return;

        Transform inTransform = inPortal.transform;
        Transform outTransform = otherPortal.transform;
        Transform outPortalCamera = otherPortal.thisCam.transform;
        Camera Maincamera = RecursiveCameraCustom.Instance.MyCam;

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

        // Set the camera's oblique view frustum.
        Plane p = new Plane(otherPortal.forwardRevert.forward, outTransform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(otherPortal.thisCam.worldToCameraMatrix)) * clipPlane;

        var newMatrix = Maincamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        otherPortal.thisCam.projectionMatrix = newMatrix;


        // Render the camera to its render target.
        otherPortal.thisCam.Render();
    }

    private void Test(Portal inPortal)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = otherPortal.transform;
        Transform outPortalCamera = otherPortal.thisCam.transform;
        Camera Maincamera = RecursiveCameraCustom.Instance.MyCam;

        Quaternion beforeOtherRot = otherPortal.forwardRevert.rotation;
        Vector3 beforeOtherPos = otherPortal.forwardRevert.position;
        Quaternion beforeInForwardRot = inPortal.forwardRevert.rotation;
        Vector3 beforeInForwardPos = inPortal.forwardRevert.position;

        var vectorCamToPortalWorld = inTransform.position +
             (inTransform.position - Maincamera.transform.position);
        otherPortal.forwardRevert.localPosition = inTransform.InverseTransformPoint(vectorCamToPortalWorld);
        // Rotate the camera to look through the other portal.
        // Quaternion relativeRot2 =
        // Quaternion.LookRotation(otherPortal.forwardRevert.forward);
        // relativeRot *=
        //  Quaternion.Inverse(inPortal.transform.rotation);
        // relativeRot *= otherPortal.transform.rotation;
        // relativeRot *= Quaternion.Euler(0.0f, 180.0f, 0.0f);
        // relativeRot *=
        //  Quaternion.LookRotation( vectorCamToPortal);
        // otherPortal.forwardRevert.localPosition =  vectorCamToPortal;

        // relativeRot *= Quaternion.FromToRotation(Vector3.zero, vectorCamToPortal);

        // Rotate the camera to look through the other portal.
        Quaternion relativeRot =
        // Quaternion.Inverse(inTransform.rotation);
        // relativeRot *= Quaternion.FromToRotation(otherPortal.forwardRevert.forward, vectorCamToPortal);
        // relativeRot *= 
        Quaternion.Euler(Maincamera.transform.rotation.eulerAngles - inTransform.rotation.eulerAngles);
        outPortalCamera.LookAt(otherPortal.forwardRevert);
        // outPortalCamera.rotation = 
        // // outTransform.rotation * 
        // relativeRot;

        if (!Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Maincamera.transform.position, vectorCamToPortalWorld);

            // Gizmos.DrawLine(outTransform.position, outTransform.position + outPortalCamera.forward*5);
            Gizmos.DrawLine(outTransform.position, outTransform.position + outPortalCamera.forward * 5);

        }
        otherPortal.forwardRevert.position = beforeOtherPos;
        otherPortal.forwardRevert.rotation = beforeOtherRot;
        inPortal.forwardRevert.position = beforeInForwardPos;
        inPortal.forwardRevert.rotation = beforeInForwardRot;
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

        // var previousVelocity = warpObj.velocity;

        // // Position
        // Vector3 localPos = transform.worldToLocalMatrix.MultiplyPoint3x4(warpObj.position);
        // localPos = new Vector3(-localPos.x, localPos.y, -localPos.z);
        // warpObj.position = otherPortal.transform.localToWorldMatrix.MultiplyPoint3x4(localPos);

        // // Rotation
        // Quaternion difference = otherPortal.transform.rotation * 
        //     Quaternion.Inverse(transform.rotation * Quaternion.Euler(0, 180, 0));
        // warpObj.rotation = difference * warpObj.rotation;
        // previousVelocity = difference * previousVelocity;
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
