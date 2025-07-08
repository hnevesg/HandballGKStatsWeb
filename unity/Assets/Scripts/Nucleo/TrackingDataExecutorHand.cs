using System.IO;
using Oculus.Interaction;
using Oculus.Interaction.Body.Input;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using Oculus.Interaction.Throw;
using TMPro;
using UnityEngine;

namespace Tracking
{
    public class TrackingDataExecutorHand : MonoBehaviour
    {
        [SerializeField] private SOGameConfiguration gameConfigSo;

        [SerializeField, Interface(typeof(IBody))]
        private Object _body;

        [SerializeField] private OVRCameraRig ovrCameraRig;
        [SerializeField] private int framesToSaveData = 4; // Default value number of frames to save data
        [Header("Configuración del agarre. Rellenar sólo si la interacción lo requiere")]
        [SerializeField] private GrabbingRule pinchGrabRules, palmGrabRules;

        [SerializeField] private HandGrabInteractor handGrabInteractorL, handGrabInteractorR;

        [Header("Rellenar sólo si la interacción no es de tipo agarre")]
        [SerializeField] private HandRef handRefL, handRefR;

        [Header("Rellenar cuanto la interacción es de tipo RayInteraction")]
        [SerializeField] private RayInteractor _rayInteractorR, _rayInteractorL;
        [SerializeField] private bool withDelayEnd = false;
        [SerializeField] private TMP_Text debug;
        private bool _handDetected, _handInitialized, _saveData;

        private HandRef _handRef;
        private int _lastFrame;
        private RayInteractor _rayInteractor;
        private float _currentTime;
        private BodyJointId _jointIdArm, _jointIdWrist, _jointIdPalm;
        private HandGrabInteractor _handGrab;
        private IBody Body;

        //new hand position
        private Vector3 _lastpositionR, _newpositionR;
        private Vector3 _lastpositionL, _newpositionL;

        private BodyJointId _jointIdArmR, _jointIdPalmR, _jointIdWristR, _jointIdArmL, _jointIdPalmL, _jointIdWristL;
        private int framesDelay = 20;
        private bool _autogrip;

        private void Awake()
        {
            InitializeVariables();
            TrackingDataWriter.Instance.SetPathWritter(gameConfigSo.userPath);
            SetTrackingHeader();
        }

        public void ResetAndSetNewName(string fileName)
        {
            InitializeVariables();
            TrackingDataWriter.Instance.SetPathWritterWithFileName(gameConfigSo.userPath, fileName);
            SetTrackingHeader();
            gameConfigSo.gameStarts = true;
            gameConfigSo.gameStopped = false;
        }

        private void SetTrackingHeader()
        {
            if (gameConfigSo.gameConfiguration.handInteraction == GameConfiguration.HandInteraction.Both)
            {
                if (gameConfigSo.gameConfiguration.GrabInteraction)
                {
                    TrackingDataWriter.Instance.SetHeaderGrabBothHands();
                }
                else
                {
                    TrackingDataWriter.Instance.SetHeaderRayBothHand();
                }
            }
            else
            {
                if (gameConfigSo.gameConfiguration.GrabInteraction)
                {
                    TrackingDataWriter.Instance.SetHeaderGrabOneHand();
                }
                else
                {
                    TrackingDataWriter.Instance.SetHeaderRayOneHand();
                }
            }
        }

        private void InitializeVariables()
        {
            Body = _body as IBody;
            _handDetected = _handInitialized = _saveData = false;
            _lastpositionR = Vector3.zero;

            _lastpositionL = Vector3.zero;

            _lastFrame = Time.frameCount;
            TrackingDataWriter.Instance.ResetAll();


            //  saveHmd = true;
            gameConfigSo.gameStopped = false;
            _currentTime = 0;
        }

        private void Start()
        {
            if (gameConfigSo.userPath == "")
            {
                var dirParent = Directory.GetParent(Application.persistentDataPath);
                if (dirParent != null)
                {
                    var parentPath = dirParent.ToString();
                    gameConfigSo.userPath = Path.Combine(parentPath, RehabConstants.DirBbt, RehabConstants.DefaultUser);
                }
            }

            TrackingDataWriter.Instance.SetPathWritter(gameConfigSo.userPath);
            _saveData = true;

            if (CaptureRightData())
            {
                _jointIdArmR = BodyJointId.Body_RightArmLower;
                _jointIdPalmR = BodyJointId.Body_RightHandPalm;
                _jointIdWristR = BodyJointId.Body_RightHandWrist;
            }

            if (CaptureLeftData())
            {
                _jointIdArmL = BodyJointId.Body_LeftArmLower;
                _jointIdPalmL = BodyJointId.Body_LeftHandPalm;
                _jointIdWristL = BodyJointId.Body_LeftHandWrist;
            }

            if (!gameConfigSo.gameConfiguration.grabInteraction)
            {
                _handRef = gameConfigSo.gameConfiguration.handInteraction == GameConfiguration.HandInteraction.Right ||
                           gameConfigSo.gameConfiguration.handInteraction == GameConfiguration.HandInteraction.Both
                    ? handRefR
                    : handRefL;
                _rayInteractor =
                    gameConfigSo.gameConfiguration.handInteraction == GameConfiguration.HandInteraction.Right ||
                    gameConfigSo.gameConfiguration.handInteraction == GameConfiguration.HandInteraction.Both
                        ? _rayInteractorR
                        : _rayInteractorL;
            }
            else
            {
                if (gameConfigSo.gameConfiguration.handInteraction == GameConfiguration.HandInteraction.Left)
                {
                    handGrabInteractorL.enabled = true;
                    _handGrab = handGrabInteractorL;

                    // Deshabilitar el agarre con la mano derecha
                    handGrabInteractorR.enabled = false;
                }
                else if (gameConfigSo.gameConfiguration.handInteraction == GameConfiguration.HandInteraction.Right)
                {
                    handGrabInteractorR.enabled = true;
                    _handGrab = handGrabInteractorR;

                    // Deshabilitar el agarre con la mano izquierda
                    handGrabInteractorL.enabled = false;
                    if (gameConfigSo.gameConfiguration.rayInteraction)
                    {
                        _rayInteractor = _rayInteractorL;
                    }
                }
                else
                {
                    handGrabInteractorL.enabled = true;
                    handGrabInteractorR.enabled = true;
                    _handGrab = handGrabInteractorR;
                    if (gameConfigSo.gameConfiguration.rayInteraction)
                    {
                        _rayInteractor = _rayInteractorR;
                    }
                }

                _handRef = (HandRef)_handGrab.GetComponent(typeof(HandRef));
            }
            _autogrip = gameConfigSo.gameConfiguration.handInteraction == GameConfiguration.HandInteraction.Right
                ? gameConfigSo.gameConfiguration.autogripR
                : gameConfigSo.gameConfiguration.autogripL; 
        }



        private void Update()
        {
            var currentFrame = Time.frameCount;
            var saveData = currentFrame > _lastFrame && Time.frameCount % framesToSaveData == 0;
            Vector3 handVelocityR = Vector3.zero;
            Vector3 handVelocityL = Vector3.zero;

            if (gameConfigSo.gameStarts)
            {
                if (!gameConfigSo.gameStopped || gameConfigSo.gameStopped && withDelayEnd && framesDelay > 0 )
                {
                    if (gameConfigSo.gameStopped && withDelayEnd)
                    {
                        framesDelay--;
                    }
                    _handDetected = _handRef != null && _handRef.IsConnected;

                    if (currentFrame > _lastFrame && _handRef != null)
                    {
                        _currentTime += Time.deltaTime;
                        if (CaptureRightData())
                        {
                            _newpositionR = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand);
                            handVelocityR =
                                VelocityCalculatorUtilMethods.ToLinearVelocity(_lastpositionR, _newpositionR,
                                    Time.deltaTime);
                        }

                        if (CaptureLeftData())
                        {
                            _newpositionL = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand);
                            handVelocityL =
                                VelocityCalculatorUtilMethods.ToLinearVelocity(_lastpositionL, _newpositionL,
                                    Time.deltaTime);
                        }

                        _lastpositionL = _newpositionL;
                        _lastpositionR = _newpositionR;
                        if (saveData)
                        {
                            if (_handRef != null && (_handInitialized || _handDetected))
                            {
                                if (!_handInitialized) _handInitialized = true;
                                var handTracking = GetTrackingData();
                                if (CaptureRightData())
                                {
                                    handTracking.handVelocityR = handVelocityR;
                                }

                                if (CaptureLeftData())
                                {
                                    handTracking.handVelocityL = handVelocityL;
                                }

                                string dataToWrite = (!gameConfigSo.gameConfiguration.GrabInteraction)
                                    ? handTracking.ToString() + "\n"
                                    : handTracking.ToString();
                                TrackingDataWriter.Instance.SaveTrackingData(dataToWrite);
                            }
                        }
                    }
                }
                else
                {
                    EndTracking();
                }
            }

            _lastFrame = currentFrame;
        }


        private void EndTracking()
        {
                gameConfigSo.gameStopped = true;
                TrackingDataWriter.Instance.WriteTrackingData();
                gameConfigSo.gameStarts = false;
            
           
        }


        private TrackingData CalculateWristTwist(TrackingData handTracking)


        {
            if (_body != null)
            {
                if (gameConfigSo.gameConfiguration.handInteraction != GameConfiguration.HandInteraction.Left)
                {
                    if (Body.GetJointPose(_jointIdWristR, out Pose poseWristR) &&
                        Body.GetJointPose(_jointIdArmR, out Pose poseArmR) &&
                        Body.GetJointPose(_jointIdPalmR, out Pose poseHandPalmR))
                    {
                        Vector3 parentBoneDirectionR = poseHandPalmR.position - poseWristR.position;
                        Vector3 childBoneDirectionR = poseArmR.position - poseHandPalmR.position;
                        float angleR = Vector3.SignedAngle(parentBoneDirectionR, childBoneDirectionR, Vector3.forward);
                        handTracking.wristTwistR = angleR;
                    }
                }

                if (gameConfigSo.gameConfiguration.handInteraction != GameConfiguration.HandInteraction.Right)
                {
                    if (Body.GetJointPose(_jointIdWristL, out Pose poseWristL) &&
                        Body.GetJointPose(_jointIdArmL, out Pose poseArmL) &&
                        Body.GetJointPose(_jointIdPalmL, out Pose poseHandPalmL))
                    {
                        Vector3 parentBoneDirectionL = poseHandPalmL.position - poseWristL.position;
                        Vector3 childBoneDirectionL = poseArmL.position - poseHandPalmL.position;
                        float angleL = Vector3.SignedAngle(parentBoneDirectionL, childBoneDirectionL, Vector3.forward);
                        handTracking.wristTwistL = angleL;
                    }
                }
            }

            return handTracking;
        }


        private TrackingData GetTrackingData()
        {
            TrackingData handData;

            OVRInput.Controller _handController;

            var highConfidence = _handRef.IsHighConfidence;
            _handController = (gameConfigSo.gameConfiguration.handInteraction != GameConfiguration.HandInteraction.Left)
                ? OVRInput.Controller.RHand
                : OVRInput.Controller.LHand;

            if (!gameConfigSo.gameConfiguration.GrabInteraction)
            {
                if (gameConfigSo.gameConfiguration.handInteraction != GameConfiguration.HandInteraction.Both)
                {
                    if (!gameConfigSo.gameConfiguration.rayInteraction)
                    {
                        handData = new TrackingData(gameConfigSo.gameConfiguration.handInteraction, Time.frameCount,
                            _currentTime, _handDetected, highConfidence,
                            OVRInput.GetLocalControllerPosition(_handController),
                            OVRInput.GetLocalControllerRotation(_handController).eulerAngles,
                            ovrCameraRig.centerEyeAnchor.position,
                            ovrCameraRig.centerEyeAnchor.rotation.eulerAngles,
                            Vector3.zero,
                            0.0f);
                    }
                    else
                    {
                        handData = new TrackingData(gameConfigSo.gameConfiguration.handInteraction, true,
                            Time.frameCount,
                            _currentTime, _handDetected, highConfidence,
                            OVRInput.GetLocalControllerPosition(_handController),
                            OVRInput.GetLocalControllerRotation(_handController).eulerAngles,
                            ovrCameraRig.centerEyeAnchor.position,
                            ovrCameraRig.centerEyeAnchor.rotation.eulerAngles,
                            Vector3.zero, _rayInteractor.Origin, _rayInteractor.End,
                            _rayInteractor.Rotation.eulerAngles,
                            0.0f);
                    }
                }
                else
                {
                    HandRef handRefL = (HandRef)handGrabInteractorL.GetComponent(typeof(HandRef));
                    //both hands (_handController is Rigth)
                    if (!gameConfigSo.gameConfiguration.rayInteraction)
                    {
                        handData = new TrackingData(Time.frameCount, _currentTime, _handDetected, handRefL.IsConnected,
                            highConfidence,
                            handGrabInteractorL.Hand.IsHighConfidence,
                            OVRInput.GetLocalControllerPosition(_handController),
                            OVRInput.GetLocalControllerRotation(_handController).eulerAngles,
                            OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand),
                            OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand).eulerAngles,
                            ovrCameraRig.centerEyeAnchor.position,
                            ovrCameraRig.centerEyeAnchor.rotation.eulerAngles, Vector3.zero, Vector3.zero,
                            0.0f, 0.0f);
                    }
                    else
                    {
                        handData = new TrackingData(Time.frameCount, true, _currentTime, _handDetected,
                            handRefL.IsConnected,
                            highConfidence,
                            handGrabInteractorL.Hand.IsHighConfidence,
                            OVRInput.GetLocalControllerPosition(_handController),
                            OVRInput.GetLocalControllerRotation(_handController).eulerAngles,
                            OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand),
                            OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand).eulerAngles,
                            ovrCameraRig.centerEyeAnchor.position,
                            ovrCameraRig.centerEyeAnchor.rotation.eulerAngles, Vector3.zero, Vector3.zero,
                            _rayInteractorR.Origin,
                            _rayInteractorL.Origin,
                            _rayInteractorR.End,
                            _rayInteractorL.End,
                            _rayInteractorR.Rotation.eulerAngles, _rayInteractorL.Rotation.eulerAngles,
                            0.0f, 0.0f);
                    }
                }
            }
            else //Append grasp data
            {
                if (gameConfigSo.gameConfiguration.handInteraction != GameConfiguration.HandInteraction.Both)
                {
                    handData = new TrackingDataGrab(gameConfigSo.gameConfiguration.handInteraction, Time.frameCount,
                        _currentTime, _handDetected, highConfidence,
                        OVRInput.GetLocalControllerPosition(_handController),
                        OVRInput.GetLocalControllerRotation(_handController).eulerAngles,
                        Vector3.zero,
                        ovrCameraRig.centerEyeAnchor.position,
                        ovrCameraRig.centerEyeAnchor.rotation.eulerAngles,
                        0.0f);
                }
                else
                {
                    HandRef handGrabRefL = (HandRef)handGrabInteractorL.GetComponent(typeof(HandRef));
                    //both hands (_handController is Rigth)
                    handData = new TrackingDataGrab(Time.frameCount, _currentTime, _handDetected,
                        handGrabRefL.IsConnected,
                        highConfidence,
                        handGrabInteractorL.Hand.IsHighConfidence,
                        OVRInput.GetLocalControllerPosition(_handController),
                        OVRInput.GetLocalControllerRotation(_handController).eulerAngles,
                        OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand),
                        OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand).eulerAngles,
                        ovrCameraRig.centerEyeAnchor.position,
                        ovrCameraRig.centerEyeAnchor.rotation.eulerAngles, Vector3.zero, Vector3.zero,
                        0.0f, 0.0f);
                }
            }


            handData = CalculateWristTwist(handData);
            if (gameConfigSo.gameConfiguration.GrabInteraction)
            {
                SaveGrabDataHands(handData);
                SavePalmPosition((TrackingDataGrab)handData);
            }

            return handData;
        }

        private void SaveGrabDataHands(TrackingData handData)
        {
            if (CaptureRightData())
            {
                SaveGrabData(handData, handGrabInteractorR, true);
            }

            if (CaptureLeftData())
            {
                SaveGrabData(handData, handGrabInteractorL, false);
            }
        }

        private bool CaptureRightData()
        {
            return
                gameConfigSo.gameConfiguration.handInteraction == GameConfiguration.HandInteraction.Right ||
                gameConfigSo.gameConfiguration.handInteraction == GameConfiguration.HandInteraction.Both;
        }

        private bool CaptureLeftData()
        {
            return
                gameConfigSo.gameConfiguration.handInteraction == GameConfiguration.HandInteraction.Left ||
                gameConfigSo.gameConfiguration.handInteraction == GameConfiguration.HandInteraction.Both;
        }

        private void SaveGrabData(TrackingData handData, HandGrabInteractor handGrabInteractor, bool isRight)
        {
            if (handGrabInteractor.IsGrabbing ||
                (_autogrip && !gameConfigSo.grabIdentier.Equals("")))
            {
                //set grab identifier
                ((TrackingDataGrab)handData).SetGrabIdentifier(isRight, gameConfigSo.grabIdentier);
                for (var i = 0; i < Oculus.Interaction.Input.Constants.NUM_FINGERS; i++)
                {
                    var finger = (HandFinger)i;

                    if (_autogrip)
                    {
                        SaveFingerStrength((TrackingDataGrab)handData, finger, isRight);
                        SavePalmStrength((TrackingDataGrab)handData, finger, isRight);
                    }
                    else if (handGrabInteractor.HandGrabApi.IsHandPinchGrabbing(pinchGrabRules)) //pinch
                    {
                        SaveFingerStrength((TrackingDataGrab)handData, finger, isRight);
                    }
                    //palm grip
                    else if (handGrabInteractor.HandGrabApi.IsHandPalmGrabbing(palmGrabRules))
                    {
                        SavePalmStrength((TrackingDataGrab)handData, finger, isRight);
                    }
                }
            }

            
            ((TrackingDataGrab)handData).SetGrabbedElementIsMovedCorrectly(gameConfigSo.gameConfiguration.handInteraction, gameConfigSo.moveCorrectly);
            if(gameConfigSo.moveCorrectly)
            {
                gameConfigSo.moveCorrectly = false;
            }
        }

        private void SavePalmPosition(TrackingDataGrab handDataGrab)
        {
            if (CaptureRightData())
            {
                handDataGrab.RightHandGrip.PalmPosition = handGrabInteractorR.PalmPoint.position;
            }

            if (CaptureLeftData())
            {
                handDataGrab.RightHandGrip.PalmPosition = handGrabInteractorL.PalmPoint.position;
            }
        }


        private void SavePalmStrength(TrackingDataGrab handData, HandFinger finger, bool isRight)
        {
            if (isRight)
            {
                handData.RightHandGrip.IsPalmGrabbing = true;
                var actualStrength = handGrabInteractorR.HandGrabApi.GetFingerPalmStrength(finger);
                handData.RightHandGrip.SetStrength(finger, actualStrength, true);
            }
            else
            {
                handData.LeftHandGrip.IsPalmGrabbing = true;
                var actualStrength = handGrabInteractorL.HandGrabApi.GetFingerPalmStrength(finger);
                handData.LeftHandGrip.SetStrength(finger, actualStrength, true);
            }
        }

        private void SaveFingerStrength(TrackingDataGrab handData, HandFinger finger, bool isRight)
        {
            if (isRight)
            {
                handData.RightHandGrip.IsPinchGrabbing = true;
                var actualStrength = handGrabInteractorR.HandGrabApi.GetFingerPinchStrength(finger);
                handData.RightHandGrip.SetStrength(finger, actualStrength, false);
            }
            else
            {
                handData.LeftHandGrip.IsPinchGrabbing = true;
                var actualStrength = handGrabInteractorL.HandGrabApi.GetFingerPinchStrength(finger);
                handData.LeftHandGrip.SetStrength(finger, actualStrength, false);
            }
        }
    }
}