using System;
using UnityEngine;

// This behaviour disables 're-centering' on the Oculus Quest,
// instead forcing the camera origin to remain centered and
// facing the same direction within the Guardian boundaries, 
// even between app restarts.
public class RecenterResetter : MonoBehaviour
{
    [SerializeField]
    public OVRCameraRig CameraRig ;


    public enum FacingEdge
    {
        Unspecified,
        LongEdge,
        ShortEdge
    }

    [Tooltip("Specifies whether the 'Forward' (+Z) direction of the camera origin should be facing a Long or Short edge of the rectangular Guardian Play Area.")]
    public FacingEdge Facing = FacingEdge.Unspecified;

    public float RotationOffset { get; private set; } = 0.0f;
    public Vector3 CenterOffset { get; private set; } = Vector3.zero;

    // Start is called before the first frame update
  /*  void Start()
    {
        ResetRecenter();
    }
*/
    // Update is called once per frame
 /*   void Update()
    {
        if (Mathf.Floor(Time.timeSinceLevelLoad) > Mathf.Floor(Time.timeSinceLevelLoad - Time.deltaTime))
        {
            ResetRecenter();
        }
    }
*/
   public void ResetRecenter()
    {
        try
        {
            Vector3[] boundaryPoints = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
            CenterOffset = Vector3.zero;
            Vector3 v;

            for (int i = 0; i < boundaryPoints.Length; ++i)
            {
                v = boundaryPoints[i];
                v.y = 0.0f;

                CenterOffset += v;
            }

            CenterOffset /= boundaryPoints.Length;

            if (boundaryPoints.Length > 3)
            {
                float firstLineLength = (boundaryPoints[1] - boundaryPoints[0]).magnitude;
                float secondLineLength = (boundaryPoints[2] - boundaryPoints[1]).magnitude;

                Vector3 firstLineNormal = Vector3.Cross((boundaryPoints[1] - boundaryPoints[0]).normalized, Vector3.up)
                    .normalized;
                float rotationOffset = (Mathf.Atan2(firstLineNormal.x, firstLineNormal.z) * Mathf.Rad2Deg) - 90.0f;

                if (Facing == FacingEdge.LongEdge && firstLineLength > secondLineLength
                    || Facing == FacingEdge.ShortEdge && secondLineLength > firstLineLength)
                {
                    rotationOffset += 90.0f;
                }

                Quaternion rotationQuat = Quaternion.Euler(0.0f, rotationOffset, 0.0f);

                if (CameraRig != null)
                {
                    CameraRig.trackingSpace.localPosition = Quaternion.Inverse(rotationQuat) * (-CenterOffset);
                    CameraRig.trackingSpace.localRotation = Quaternion.Inverse(rotationQuat);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error recentering");
        }
    }
}
