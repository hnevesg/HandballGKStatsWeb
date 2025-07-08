using System;
using System.Globalization;
using Tracking;
using UnityEngine;


    /**
     * Class containing ball tracking variables
     */
    [Serializable]
    public class TrackingDataBall : TrackingData
    {
        public BallTrackingData BallData { get; set; }

        public TrackingDataBall(
            GameConfiguration.HandInteraction handInteraction,
            int frame,
            float time,
            bool handDetected,
            bool highConfidence,
            Vector3 handPosition,
            Vector3 handRotation,
            Vector3 handVelocity,
            Vector3 headPosition,
            Vector3 headRotation,
            float wristTwist,
            int ballId = -1,
            string ballStatus = "",
            Vector3 ballPosition = default) 
            : base(handInteraction, frame, time, handDetected, highConfidence, 
                  handPosition, handRotation, headPosition, headRotation, 
                  handVelocity, wristTwist)
        {
            BallData = new BallTrackingData
            {
                BallId = ballId,
                BallStatus = ballStatus,
                BallPosition = ballPosition
            };
        }
    
        public TrackingDataBall(
            int frame,
            float time,
            bool handDetectedR,
            bool handDetectedL,
            bool highConfidenceR,
            bool highConfidenceL,
            Vector3 handPositionR,
            Vector3 handRotationR,
            Vector3 handPositionL,
            Vector3 handRotationL,
            Vector3 headPosition,
            Vector3 headRotation,
            Vector3 handVelocityR,
            Vector3 handVelocityL,
            float wristTwistR,
            float wristTwistL,
            int ballId = -1,
            string ballStatus = "",
            Vector3 ballPosition = default) 
            : base(frame, time, handDetectedR, handDetectedL, highConfidenceR, highConfidenceL, 
                  handPositionR, handRotationR, handPositionL, handRotationL, 
                  headPosition, headRotation, handVelocityR, handVelocityL, 
                  wristTwistR, wristTwistL)
        {
            BallData = new BallTrackingData
            {
                BallId = ballId,
                BallStatus = ballStatus,
                BallPosition = ballPosition
            };
        }

        public void UpdateBallData(int ballId, string status, Vector3 position)
        {
            BallData.BallId = ballId;
            BallData.BallStatus = status ?? "";
            BallData.BallPosition = position;
        }
    
        public override string ToString()
        {
            return base.ToString() + BallData.ToString();
        }
    }

    [Serializable]
    public class BallTrackingData
    {
        public int BallId { get; set; }
        public string BallStatus { get; set; }
        public Vector3 BallPosition { get; set; }

        public override string ToString()
        {
            return $"{BallId};{BallStatus};" +
                   $"{BallPosition.x.ToString("f4", CultureInfo.InvariantCulture)};" +
                   $"{BallPosition.y.ToString("f4", CultureInfo.InvariantCulture)};" +
                   $"{BallPosition.z.ToString("f4", CultureInfo.InvariantCulture)}\n";
        }
    }
