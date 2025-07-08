using System.IO;
using Oculus.Interaction.Throw;
using TMPro;
using Tracking;
using UnityEngine;
using UnityEngine.Serialization;

public class TrackingDataExecutorController : MonoBehaviour
{
    [SerializeField] private SOGameConfiguration gameConfigSo;
    [SerializeField] private TMP_Text debugText;
    
    [SerializeField] private OVRCameraRig ovrCameraRig;

    // Variables for tracking time and frame count
    private float _currentTime;
    private int _frameCountBefore;

    // Last processed frame
    private int _lastFrame;

    private Vector3 _lastGlobalPosR, _lastGlobalPosL;
    private Vector3 _currentGlobalPosR, _currentGlobalPosL;
    private Quaternion _currentGlobalRotR, _currentGlobalRotL;
    private int _partialFrameCount;
    private bool _startTracking;

    // Partial timer (game scene started)
    private float _partialTimer;

    private bool _saveData;

    // Configurable frame interval for writing data
    [SerializeField] private int writeEveryNFrames = 1;
    private int _framesSinceLastWrite;
    
    private BallTrackingData _currentBallData = new BallTrackingData();
    private BallCreator _currentBall;
    private int _lastBallId = -1;

    public void SetCurrentBall(BallCreator ball)
    {
        if (ball != null && ball.ball_id != _lastBallId)
        {
            _currentBall = ball;
            _lastBallId = ball.ball_id;
        }
    }
    private void Awake()
    {
        //ResetAll();
        ResetTRackingVariables();
    }

    private void Start()
    {
        SetPathWritter();
    }
    
    private void ResetAll()
    {
        _saveData = true;
        TimeManager.sharedTime = 0;
        _currentTime = 0;
        _lastGlobalPosR = _lastGlobalPosL = Vector3.zero;
        _lastFrame = TimeManager.sharedFrame;
        _framesSinceLastWrite = 0;
        
        TrackingDataWriter.Instance.ResetAll();
        TrackingDataWriter.Instance.SetHeaderControllerHGTrainer();
        //ToDo remove from here when the object is in the initial menu
        gameConfigSo.gameConfiguration.handInteraction = GameConfiguration.HandInteraction.Both;
    }

   

    private void Update()
    {
        if (gameConfigSo.gameStarts)
        {
            HandleGameUpdate();
        }
    }

    private void HandleGameUpdate()
    {
        var currentFrame = TimeManager.sharedFrame;
        _framesSinceLastWrite++;

        // Determine if we should save data based on frame count
        bool saveData = _framesSinceLastWrite >= writeEveryNFrames;

        if (!_startTracking)
        {
            InitializeTracking();
        }

        if (!gameConfigSo.gameStopped)
        {
            ProcessFrame(currentFrame, saveData);

            if (saveData)
            {
                _framesSinceLastWrite = 0; // Reset counter
            }
        }
        else
        {
            EndTracking();
        }

        _lastFrame = currentFrame;
    }

    private void SetPathWritter()
    {
        if (gameConfigSo.userPath == "")
        {
            var dirParent = Directory.GetParent(Application.persistentDataPath);
            if (dirParent != null)
            {
                var parentPath = dirParent.ToString();
                gameConfigSo.userPath =
                    Path.Combine(parentPath, RehabConstants.DirBbt, RehabConstants.DefaultUser);
            }
        }
    }

  

    private void ResetTRackingVariables()
    {
        _startTracking = false;
        gameConfigSo.gameStarts = false;
        gameConfigSo.gameStopped = false;
    }

    private void EndTracking()
    {
        if (_saveData)
        {
            TrackingDataWriter.Instance.WriteTrackingData();
            ResetTRackingVariables();
            _saveData = false;
        }
    }

    private void InitializeTracking()
    {
        ResetAll();
        TrackingDataWriter.Instance.SetPathWritter(gameConfigSo.userPath);
        _startTracking = true;
    }

  
    
    public void ProcessFrame(int currentFrame, bool saveData)
    {
        if (currentFrame <= _lastFrame) return;

        _currentTime += Time.deltaTime;
        UpdateGlobalHandData();
        
        Vector3 speedR = CalculateGlobalSpeed(_lastGlobalPosR, _currentGlobalPosR);
        Vector3 speedL = CalculateGlobalSpeed(_lastGlobalPosL, _currentGlobalPosL);

        _lastGlobalPosR = _currentGlobalPosR;
        _lastGlobalPosL = _currentGlobalPosL;

        if (saveData)
        {
            // Check ball data
            int ballId = -1;
            string ballStatus = "";
            Vector3 ballPosition = Vector3.zero;
            
            if (_currentBall != null)
            {
                ballId = _currentBall.ball_id;
                ballStatus = _currentBall.ball_status;
                ballPosition = _currentBall.gameObject.transform.position;
            }

            SaveTrackingData(speedR, speedL, ballId, ballStatus, ballPosition);
        }
        
        _lastFrame = currentFrame;
    }

    private void UpdateGlobalHandData()
    {
        _currentGlobalPosR = ovrCameraRig.rightControllerAnchor.position;
        _currentGlobalPosL = ovrCameraRig.leftControllerAnchor.position;
        _currentGlobalRotR = ovrCameraRig.rightControllerAnchor.rotation;
        _currentGlobalRotL = ovrCameraRig.leftControllerAnchor.rotation;
    }

    //Global velocity
    private Vector3 CalculateGlobalSpeed(Vector3 lastPos, Vector3 currentPos)
    {
        return VelocityCalculatorUtilMethods.ToLinearVelocity(lastPos, currentPos, Time.deltaTime);
    }


   

    private void SaveTrackingData(Vector3 speedR, Vector3 speedL, int ballId, string ballStatus, Vector3 ballPosition)
    {
        var trackingData = new TrackingDataBall(
            TimeManager.sharedFrame,
            TimeManager.sharedTime,
            true, true, true, true,
            _currentGlobalPosR, _currentGlobalRotR.eulerAngles,
            _currentGlobalPosL, _currentGlobalRotL.eulerAngles,
            ovrCameraRig.centerEyeAnchor.position,
            ovrCameraRig.centerEyeAnchor.rotation.eulerAngles,
            speedR, speedL,
            0.0f, 0.0f,
            ballId, ballStatus, ballPosition
        );
        
        TrackingDataWriter.Instance.SaveTrackingData(trackingData.ToString());
    }

}