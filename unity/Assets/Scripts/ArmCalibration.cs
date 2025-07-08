using UnityEngine;
using System.Collections;

public class ArmCalibration : MonoBehaviour
{
    public Transform hmdTransform;
    public Transform rightControllerTransform;
    public Transform leftControllerTransform;
    public GameObject rArmModel;
    public GameObject lArmModel;
    private float defaultArmLength = 0.7f;
    public GameObject calibrationCanvas;
    public GameObject mainCanvas;
    [SerializeField]
    private GameObject model3D_trunk;

    void OnEnable()
    {
        model3D_trunk.GetComponent<SkinnedMeshRenderer>().enabled = true;
        //active wait for the player to be in T pose
        StartCoroutine(WaitForTPose());
    }

    private void CalibrateCharacterArms()
    {
        float armLength;

        //when the player is in T pose measure the distance between the head and the arm
        if (rightControllerTransform == null)
        {
            armLength = Vector3.Distance(hmdTransform.position, leftControllerTransform.position);
        }
        else
        {
            armLength = Vector3.Distance(hmdTransform.position, rightControllerTransform.position);
        }
        float armScaleFactor = armLength / defaultArmLength;
        rArmModel.transform.localScale = new Vector3(1, armScaleFactor, 1);
        lArmModel.transform.localScale = new Vector3(1, armScaleFactor, 1);
        
       // DisableRenderer();

        calibrationCanvas.SetActive(false);
        mainCanvas.SetActive(true);

        Debug.Log("Arms calibrated correctly");
    }

    private void DisableRenderer()
    {
        Invoke("DisableTrunkRenderer", 1.5f);
    }

    private void DisableTrunkRenderer()
    {
        model3D_trunk.GetComponent<SkinnedMeshRenderer>().enabled = false;
    }

    bool TriggerPressedForTPose()
    {
        return OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) > 0.8f ||
               OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) > 0.8f;
    }

    private IEnumerator WaitForTPose()
    {
        while (!TriggerPressedForTPose())
        {
            yield return null;
        }

        CalibrateCharacterArms();
    }
}