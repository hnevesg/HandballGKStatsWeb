using System;
using UnityEngine;

namespace Tracking
{
    /**
 * Class containing kinematics variables associated with grab exercises.
 */
    [Serializable]

    public class TrackingDataGrab : TrackingData
    {
   
        public TrackingDataGrip RightHandGrip { get; set; }
        public TrackingDataGrip LeftHandGrip { get; set; }

        public TrackingDataGrab(
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
            float wristTwist) : base(handInteraction, frame, time, handDetected, highConfidence, 
            handPosition, handRotation, headPosition, headRotation, handVelocity, wristTwist)
        {
            this.handInteraction = handInteraction;
            RightHandGrip = new TrackingDataGrip();
            LeftHandGrip = new TrackingDataGrip();
        }
    
        public TrackingDataGrab(
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
            float wristTwistL
        
        ) : base(frame, time, handDetectedR, handDetectedL, highConfidenceR, highConfidenceL, handPositionR, handRotationR, 
            handPositionL, handRotationL, headPosition, headRotation, handVelocityR, handVelocityL, wristTwistR, wristTwistL)
        {
            RightHandGrip = new TrackingDataGrip();
            LeftHandGrip = new TrackingDataGrip();
        }
        public void SetStrength(Oculus.Interaction.Input.HandFinger finger, 
            float actualStrength, bool palm, GameConfiguration.HandInteraction handInteraction)
        {
            switch (handInteraction)
            {
                case GameConfiguration.HandInteraction.Right:
                    RightHandGrip.SetStrength(finger, actualStrength, palm);
                    break;
                case GameConfiguration.HandInteraction.Left:
                    LeftHandGrip.SetStrength(finger, actualStrength, palm);
                    break;
           
                default:
                    // ToDo
                    break;
            }
        }

        public void SetGrabIdentifier(bool isRight, string identifier)
        {
            if (isRight)
            {
                RightHandGrip.GrabbedElementIdentifier = identifier;
            }
            else
            {


                LeftHandGrip.GrabbedElementIdentifier = identifier;
            }

           
        }

        public void SetGrabbedElementIsMovedCorrectly(GameConfiguration.HandInteraction handInteraction, bool movedCorrectly)
        {
            switch (handInteraction)
            {
                case GameConfiguration.HandInteraction.Right:
                    RightHandGrip.GrabbedElementIsMovedCorrectly = movedCorrectly;
                    break;
                case GameConfiguration.HandInteraction.Left:
                    LeftHandGrip.GrabbedElementIsMovedCorrectly =  movedCorrectly;
                    break;
           
                default:
                    // ToDo
                    break;
            }
        }
    
        
        public override string ToString()
        {
            string result = "";
    
            if (handInteraction == GameConfiguration.HandInteraction.Right)
            {
                result = base.ToString()    + RightHandGrip.ToString();
            }
            else if (handInteraction == GameConfiguration.HandInteraction.Left)
            {
                result =  base.ToString()  + LeftHandGrip.ToString();
            }
            else if (handInteraction == GameConfiguration.HandInteraction.Both)
            {
                result =  base.ToString()  + RightHandGrip.ToString() +  LeftHandGrip.ToString();
            }
    
            return result + "\n";
        }
    }
}