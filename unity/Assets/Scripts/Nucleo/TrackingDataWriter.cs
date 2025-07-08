using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace Tracking
{
    public class TrackingDataWriter
    {
        private static readonly string _filename = "OculusTracking";
        private List<String> _trackToWrite;

        private static readonly Lazy<TrackingDataWriter> _instance =
            new Lazy<TrackingDataWriter>(() => new TrackingDataWriter());

        private const string DataHeaderRayOneHand =
            "Frame;Time;HeadPosition_x;HeadPosition_y;HeadPosition_z;HeadRotation_x;HeadRotation_y;HeadRotation_z;" +
            "HandDetected;HighConfidence;" +
            "HandPosition_x;HandPosition_y;HandPosition_z;HandRotation_x;HandRotation_y;HandRotation_z;HandVelocity_x;HandVelocity_y;HandVelocity_z;" +
            "HandRaySourcePosition_x;HandRaySourcePosition_y;HandRaySourcePosition_z;" +
            "HandRayTargetPosition_x;HandRayTargetPosition_y;HandRayTargetPosition_z;" +
            "HandRayRotation_x;HandRayRotation_y;HandRayRotation_z;WristTwist";

        private const string DataHeaderNoGrabNoRayOneHand =
            "Frame;Time;HeadPosition_x;HeadPosition_y;HeadPosition_z;HeadRotation_x;HeadRotation_y;HeadRotation_z;" +
            "HandDetected;HighConfidence;" +
            "HandPosition_x;HandPosition_y;HandPosition_z;HandRotation_x;HandRotation_y;HandRotation_z;HandVelocity_x;HandVelocity_y;HandVelocity_z;" +
            "WristTwist";

        private const string DataHeaderGrabOneHand =
            DataHeaderNoGrabNoRayOneHand +";" + "PalmPosition_x;PalmPosition_y;PalmPosition_z;IsPinchGrabbing;IsPalmGrabbing;IsAutogripGrabbing;" +
            "StrengthThumbIndex;StrengthThumbMiddle;" +
            "StrengthThumbRing;StrengthThumbPinky;" +
            "StrengthPalmIndex;StrengthPalmMiddle;StrengthPalmRing;StrengthPalmPinky;GrabIdentifier;MovedCorrectly";
        

        private const string DataHeaderRayBothHand =
            "Frame;Time;HeadPosition_x;HeadPosition_y;HeadPosition_z;HeadRotation_x;HeadRotation_y;HeadRotation_z;" +
            "HandDetectedR;" +
            "HighConfidenceR;" +
            "RHandPosition_x;RHandPosition_y;RHandPosition_z;" +
            "RHandRotation_x;RHandRotation_y;RHandRotation_z;" +
            "RHandVelocity_x;RHandVelocity_y;RHandVelocity_z;" +
            "RHandRaySourcePosition_x;RHandRaySourcePosition_y;RHandRaySourcePosition_z;" +
            "RHandRayTargetPosition_x;RHandRayTargetPosition_y;RHandRayTargetPosition_z;" +
            "RHandRayRotation_x;RHandRayRotation_y;RHandRayRotation_z;" +
            "RWristTwist;" +
            "HandDetectedL;" +
            "HighConfidenceL;" +
            "LHandPosition_x;LHandPosition_y;LHandPosition_z;" +
            "LHandRotation_x;LHandRotation_y;LHandRotation_z;" +
            "LHandVelocity_x;LHandVelocity_y;LHandVelocity_z;" +
            "LHandRayPosition_x;LHandRayPosition_y;LHandRayPosition_z;" +
            "LHandRayRotation_x;LHandRayRotation_y;LHandRayRotation_z;" +
            "LWristTwist";

       

        private const string DataHeaderControllerHGTrainer =
            "Frame;Time;HeadPosition_x;HeadPosition_y;HeadPosition_z;HeadRotation_x;HeadRotation_y;HeadRotation_z;" +
            "HandDetectedR;" +
            "HighConfidenceR;" +
            "RHandPosition_x;RHandPosition_y;RHandPosition_z;" +
            "RHandRotation_x;RHandRotation_y;RHandRotation_z;" +
            "RHandVelocity_x;RHandVelocity_y;RHandVelocity_z;" +
            "RWristTwist;" +
            "HandDetectedL;" +
            "HighConfidenceL;" +
            "LHandPosition_x;LHandPosition_y;LHandPosition_z;" +
            "LHandRotation_x;LHandRotation_y;LHandRotation_z;" +
            "LHandVelocity_x;LHandVelocity_y;LHandVelocity_z;" +
            "LWristTwist;" + "BallStatus;BallPosX;BallPosY;BallPosZ";
        private const string DataHeaderNoGrabNoRayBothHand =
            "Frame;Time;HeadPosition_x;HeadPosition_y;HeadPosition_z;HeadRotation_x;HeadRotation_y;HeadRotation_z;" +
            "HandDetectedR;" +
            "HighConfidenceR;" +
            "RHandPosition_x;RHandPosition_y;RHandPosition_z;" +
            "RHandRotation_x;RHandRotation_y;RHandRotation_z;" +
            "RHandVelocity_x;RHandVelocity_y;RHandVelocity_z;" +
            "RWristTwist;" +
            "HandDetectedL;" +
            "HighConfidenceL;" +
            "LHandPosition_x;LHandPosition_y;LHandPosition_z;" +
            "LHandRotation_x;LHandRotation_y;LHandRotation_z;" +
            "LHandVelocity_x;LHandVelocity_y;LHandVelocity_z;" +
            "LWristTwist";


        private const string DataHeaderGrabBothHand =
            DataHeaderNoGrabNoRayBothHand + ";" +
            "RPalmPosition_x;RPalmPosition_y;RPalmPosition_z;RIsPinchGrabbing;RIsPalmGrabbing;RIsAutogripGrabbing;" +
            "RStrengthThumbIndex;RStrengthThumbMiddle;" +
            "RStrengthThumbRing;RStrengthThumbPinky;" +
            "RStrengthPalmIndex;RStrengthPalmMiddle;RStrengthPalmRing;RStrengthPalmPinky;" +
            "RGrabIdentifier;RMovedCorrectly;" +
            "LPalmPosition_x;LPalmPosition_y;LPalmPosition_z;" +
            "LIsPinchGrabbing;LIsPalmGrabbing;LIsAutogripGrabbing;" +
            "LStrengthThumbIndex;LStrengthThumbMiddle;" +
            "LStrengthThumbRing;LStrengthThumbPinky;" +
            "LStrengthPalmIndex;LStrengthPalmMiddle;LStrengthPalmRing;LStrengthPalmPinky;" +
            "LGrabIdentifier;LMovedCorrectly";

        private string _dataHeader;


        private const int Batch = 10;
        public string filePath;

        private bool _setHead;


        public void SetHeaderRayOneHand()
        {
            _dataHeader = DataHeaderRayOneHand + ";";
        }


        public void SetHeaderGrabOneHand()
        {
            _dataHeader = DataHeaderGrabOneHand + ";";
        }

        public void SetHeaderNoRayNoGrabOneHand()
        {
            _dataHeader = DataHeaderNoGrabNoRayOneHand + ";";
        }

        public void SetHeaderNoGrabNoRayBothHand()
        {
            _dataHeader = DataHeaderNoGrabNoRayBothHand + ";";
        }


        public void SetHeaderGrabBothHands()
        {
            _dataHeader = DataHeaderGrabBothHand + ";";
        }

        public void SetHeaderRayBothHand()
        {
            _dataHeader = DataHeaderRayBothHand + ";";
        }
        
        public void SetHeaderControllerHGTrainer()
        {
            _dataHeader = DataHeaderControllerHGTrainer + ";";
        }


        private TrackingDataWriter()
        {
            ResetAll();
        }

        public void ResetAll()
        {
            //First write -- add header
            _setHead = true;
            _trackToWrite = new List<string>();
        }

        public void SetPathWritterWithFileName(String userPath, string fileName)
        {
            string trackingData = Path.Combine(userPath, RehabConstants.DirTracking);
            DateTime now = DateTime.Now;
            Constants_Handball.seriesDate = now;
            filePath = Path.Combine(trackingData, fileName + now.ToString(RehabConstants.DateFormat)
                                                           + RehabConstants.ExtensionCsv);
            if (!Directory.Exists(trackingData))
            {
                try
                {
                    //create new user path
                    Directory.CreateDirectory(trackingData);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error creating user directory to save data tracking: " + e.Message);
                }
            }
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public void SetPathWritter(String userPath)
        {
           SetPathWritterWithFileName(userPath, _filename);
        }


        public static TrackingDataWriter Instance => _instance.Value;


        public void SaveTrackingData(String handTracking)
        {
            try
            {
                _trackToWrite.Add(handTracking.ToString());


                if (_trackToWrite.Count > Batch)
                {
                    //write to file
                    WriteTrackingData();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error saving tracking data: " + e.Message);
            }
        }

        /**
     * Write tracking data into file
     */
        public void WriteTrackingData()
        {
            try
            {
                CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                using (StreamWriter file = new StreamWriter
                           (filePath, true, Encoding.UTF8))
                {
                    if (_setHead)
                    {
                        //write header first time
                        file.WriteLine(_dataHeader);
                        _setHead = false;
                    }


                    file.Write(String.Join("", this._trackToWrite));
                }
            }
            catch (IOException e)
            {
                Debug.LogError("Error writing tracking data: " + e.Message);
            }

            _trackToWrite = new List<String>();
        }

        public string FilePath
        {
            get => filePath;
            set => filePath = value;
        }
    }
}