using System;
using System.IO;
using Oculus.Interaction;
using Oculus.Interaction.Body.Input;
using Oculus.Interaction.Body.PoseDetection;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using Oculus.Interaction.Throw;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Constants = Oculus.Interaction.Input.Constants;

public class TrackingEventMovements : MonoBehaviour
{
    
    // Define the event delegate
    public delegate void TrackingWarnings();
    // Define events for different conditions
    public event TrackingWarnings NeckFlexionEventX;
    public event TrackingWarnings NeckFlexionEventY;
    public event TrackingWarnings NeckFlexionEventZ;
    
    public event TrackingWarnings TrunkDisplacementEventX;
    public event TrackingWarnings TrunkDisplacementEventZ;
    public event TrackingWarnings ArmSupinationEvent;
    [SerializeField]
    private SOGameConfiguration gameConfigSo;
    private Vector3 hmdInitialPosition;
    private Quaternion hmdInitialRotation;
    private float trunkDisplacementThreshold;
    [SerializeField]private OVRCameraRig _ovrCameraRig;
    
    
    //TODO elimiinar
    [SerializeField] private Transform baseObject; // Referencia al objeto que simula la base
    [SerializeField] private Transform trunkObject; // Referencia al objeto que simula el tronco

    [SerializeField] private Vector3 initialHMDPosition; // Posición inicial del HMD
    [SerializeField] private Quaternion initialHMDRotation;
    private OVRInput.Controller _handController;
   
    private void Awake()
    {
    }

    private void Start()

    {
        _handController = gameConfigSo.gameConfiguration.handInteraction == GameConfiguration.HandInteraction.Right
            ? OVRInput.Controller.RHand
            : OVRInput.Controller.LHand;
    }

    


    private void Update()
    {
     
        if (!gameConfigSo.gameStopped && _ovrCameraRig != null)
        {
            GetPoses();
        }
        else
        {
          
        }

        
    }


   


    private void GetPoses()


    {
        Vector3 currentHMDPosition = _ovrCameraRig.centerEyeAnchor.position;
        Quaternion currentHMDRotation = _ovrCameraRig.centerEyeAnchor.rotation;
        
        
        
        Quaternion relativeRotation = Quaternion.Inverse(gameConfigSo.calibrationConfiguration.hmdInitialRotation) * currentHMDRotation;
        Vector3 trunkDisplacement = currentHMDPosition - gameConfigSo.calibrationConfiguration.hmdInitialPosition;
        float trunkDisplacement_x = trunkDisplacement.x;
        float trunkDisplacement_z = trunkDisplacement.z;
        // Calculate the flexion angles
        float neckFlexionAngle_y = relativeRotation.eulerAngles.y;
        float neckFlexionAngle_x = relativeRotation.eulerAngles.x;
        float neckFlexionAngle_z = relativeRotation.eulerAngles.z;
        
        
       
     

        // Aplicar los ángulos de flexión al cubo del tronco
        Quaternion neckRotation_y = Quaternion.Euler(0f, neckFlexionAngle_y, 0f);
        Quaternion neckRotation_x = Quaternion.Euler(neckFlexionAngle_x, 0f, 0f);
        Quaternion neckRotation_z = Quaternion.Euler(0f, 0f, neckFlexionAngle_z);
        
        transform.rotation = neckRotation_y * neckRotation_x;
      trunkObject.transform.position = gameConfigSo.calibrationConfiguration.hmdInitialPosition + new Vector3(trunkDisplacement.x, trunkDisplacement.y , trunkDisplacement.z +0.7f);
      // trunkObject.transform.position = gameConfigSo.gameConfiguration.hmdInitialPosition + new Vector3(trunkDisplacement.x, trunkDisplacement.y , trunkDisplacement.z +0.7f);
    
       
        // Calculate the displacement of the trunk

        // Check if the flexion angles exceed the thresholds
        
      //  Debug.LogWarning("neckFlexionAngle  " + neckFlexionAngle_y + "  ;trunkFlexionAngle " + neckRotation_y +"  ;trunkDisplacement " +trunkDisplacement);
        if (Mathf.Abs(neckFlexionAngle_y) > gameConfigSo.calibrationConfiguration.neckFlexionThreshold_Y)
        {
            
            NeckFlexionEventY?.Invoke();
        }

        if (Mathf.Abs(neckFlexionAngle_x) > gameConfigSo.calibrationConfiguration.neckFlexionThreshold_X)
        {
            // Trigger the TrunkFlexionEvent
            NeckFlexionEventX?.Invoke();
        }
        if (Mathf.Abs(neckFlexionAngle_z) > gameConfigSo.calibrationConfiguration.neckFlexionThreshold_Z)
        {
            // Trigger the TrunkFlexionEvent
            NeckFlexionEventZ?.Invoke();
        }

        // Check if the trunk displacement exceeds the threshold
        if (trunkDisplacement_z > gameConfigSo.calibrationConfiguration.trunkDisplacementThreshold_Z)
        {
            // Trigger the TrunkDisplacementEvent
            TrunkDisplacementEventZ?.Invoke();
        }
        
        // Check if the trunk displacement exceeds the threshold
        if (trunkDisplacement_x > gameConfigSo.calibrationConfiguration.trunkDisplacementThreshold_X)
        {
            // Trigger the TrunkDisplacementEvent
            TrunkDisplacementEventX?.Invoke();
        }
        
        
        
        
       
        
    }

    private void ArmSuppination()
    {
        try
        {
            // Check arm supination threshold
            OVRInput.Controller activeController = OVRInput.GetActiveController();
            float armSupinationAngle = OVRInput.GetLocalControllerRotation(_handController).eulerAngles.z;
            if (Mathf.Abs(armSupinationAngle) > gameConfigSo.calibrationConfiguration.armSupinationThreshold)
            {
                ArmSupinationEvent?.Invoke();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error calculate arm suppination " + e.Message);
        }
    }
    
    

}