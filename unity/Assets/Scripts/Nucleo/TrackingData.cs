using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Tracking
{
    [Serializable]
    public class TrackingData
    {
        public int frame;
        public float time;
        public bool handDetectedR;
        public bool handDetectedL;
        public bool highConfidenceR;
        public bool highConfidenceL;
        public Vector3 handPositionR;
        public Vector3 handPositionL;
        public Vector3 handRotationR;
        public Vector3 handRotationL;
        public Vector3 headPosition;
        public Vector3 headRotation;
        public Vector3 handVelocityR;
        public Vector3 handVelocityL;
        public Vector3 handRaySourcePositionR;
        public Vector3 handRaySourcePositionL;
        public Vector3 handRayTargetPositionR;
        public Vector3 handRayTargetPositionL;
        public Vector3 handRayRotationR;
        public Vector3 handRayRotationL;
        public float wristTwistR;
        public float wristTwistL;
        public GameConfiguration.HandInteraction handInteraction;
        public bool withRay;


        public TrackingData(GameConfiguration.HandInteraction handInteraction ) : this(0, 0, false,
            false,false,false,
            Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero,Vector3.zero, Vector3.zero,Vector3.zero,
            Vector3.zero, 0.0f, 0.0f)

        {
        
 
        }
        /**
 * Constructor for the TrackingData class that represents tracking data for both hands.
 *
 * @param handInteraction Hand interaction type, can be GameConfiguration.HandInteraction.Right
 *                       for the right hand or any other value for the left hand.
 * @param frame           Frame number or frame in which the tracking data was recorded.
 * @param time            Time in seconds at which the tracking data was recorded.
 * @param handDetected    Indicates whether a hand was detected in the tracking.
 * @param highConfidence  Indicates whether the confidence in hand detection is high.
 * @param handPosition    Hand position in three-dimensional space.
 * @param handRotation    Hand rotation represented as a quaternion.
 * @param headPosition    Head position in three-dimensional space.
 * @param headRotation    Head rotation represented as a quaternion.
 * @param handVelocity    Hand velocity in three-dimensional space.
 * @param wristTwist      Wrist twist.
 */


        public TrackingData(GameConfiguration.HandInteraction handInteraction, bool withRay, int frame,
            float time, bool handDetected,
            bool highConfidence, Vector3 handPosition, Vector3 handRotation, 
            Vector3 headPosition, Vector3 headRotation, Vector3 handVelocity,  Vector3 handRaySourcePosition,Vector3 handRayTargetPosition,  Vector3 handRayRotation, float wristTwist)
        {
            this.withRay = withRay;
            this.handInteraction = handInteraction;
            this.frame = frame;
            this.time = time;
        
       
            if (handInteraction != GameConfiguration.HandInteraction.Right)
            {
                handPositionL = handPosition;
                handRotationL = handRotation;
                handVelocityL = handVelocity;
                wristTwistL = wristTwist;
                highConfidenceL = highConfidence;
                handDetectedL = handDetected;
                if (withRay)
                {
                    handRaySourcePositionL = handRaySourcePosition;
                    handRayTargetPositionL = handRayTargetPosition;
                    handRayRotationL = handRayRotation;
                }
            }
            else
            {
                handPositionR = handPosition;
                handRotationR = handRotation;
                handVelocityR = handVelocity;
                wristTwistR = wristTwist;
                highConfidenceR = highConfidence;
                handDetectedR = handDetected;
                if (withRay)
                {
                    handRaySourcePositionR = handRaySourcePosition;
                    handRayTargetPositionR = handRayTargetPosition;
                    handRayRotationR = handRayRotation;
                }
            }
        
            this.headPosition = headPosition;
            this.headRotation = headRotation;
       
        }
    
        public TrackingData(GameConfiguration.HandInteraction handInteraction, int frame,
            float time, bool handDetected,
            bool highConfidence, Vector3 handPosition, Vector3 handRotation, 
            Vector3 headPosition, Vector3 headRotation, Vector3 handVelocity,  float wristTwist)
            : this(handInteraction,false, frame, time, handDetected, highConfidence,
                handPosition, handRotation, headPosition, headRotation, 
                handVelocity, Vector3.zero, Vector3.zero, Vector3.zero, wristTwist)
        {
    
        
        }

    
        public TrackingData(int frame, float time, bool handDetectedR, bool handDetectedL,  bool highConfidenceR, bool highConfidenceL)
            : this(frame, time, handDetectedR, handDetectedL, highConfidenceR, highConfidenceL, Vector3.zero, Vector3.zero, Vector3.zero, 
                Vector3.zero,
                Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero,
                0f, 0f)
        {
       
        }
   

        public TrackingData(int frame, float time, bool handDetectedR,bool handDetectedL,
            bool highConfidenceR, bool highConfidenceL, Vector3 handPositionR, Vector3 handRotationR, Vector3 handPositionL,
            Vector3 handRotationL,
            Vector3 headPosition, Vector3 headRotation, Vector3 handVelocityR, 
            Vector3 handVelocityL, float wristTwistR, float wristTwistL): this(frame, false, time, handDetectedR, handDetectedL, 
            highConfidenceR, highConfidenceL, handPositionR, handRotationR, handPositionL, handRotationL, 
            headPosition, headRotation, handVelocityR, handVelocityL, 
            Vector3.zero, Vector3.zero,
            Vector3.zero, Vector3.zero,  Vector3.zero, Vector3.zero, 
            wristTwistR, wristTwistL)
        {
       
        }
    
        public TrackingData(int frame, bool withRay, float time, bool handDetectedR, bool handDetectedL,
            bool highConfidenceR, bool highConfidenceL, Vector3 handPositionR, Vector3 handRotationR, Vector3 handPositionL,
            Vector3 handRotationL,
            Vector3 headPosition, Vector3 headRotation, Vector3 handVelocityR, 
            Vector3 handVelocityL, Vector3 handRaySourcePositionR, Vector3 handRaySourcePositionL,
            Vector3 handRayTargetPositionR, Vector3 handRayTargetPositionL,
            Vector3 handRayRotationR, Vector3 handRayRotationL, float wristTwistR, float wristTwistL)

        {

            this.withRay = withRay;
            handInteraction = GameConfiguration.HandInteraction.Both;
            this.frame = frame;
            this.time = time;
            this.handDetectedR = handDetectedR;
            this.handDetectedL = handDetectedL;
            this.highConfidenceR = highConfidenceR;
            this.highConfidenceL = highConfidenceL;
            this.handPositionR = handPositionR;
            this.handPositionL = handPositionL;
            this.handRotationR = handRotationR;
            this.handRotationL = handRotationL;
            this.handVelocityR = handVelocityR;
            this.handVelocityL = handVelocityL;
            this.headPosition = headPosition;
            this.headRotation = headRotation;
            this.handRaySourcePositionR = handRaySourcePositionR;
            this.handRaySourcePositionL = handRaySourcePositionL;
            this.handRayTargetPositionR = handRayTargetPositionR;
            this.handRayTargetPositionL = handRayTargetPositionL;
            this.handRayRotationR = handRayRotationR;
            this.handRayRotationL = handRayRotationL;
        
            this.wristTwistR = wristTwistR;
            this.wristTwistL = wristTwistL;
        }



   

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(frame + ";")
                .Append(time.ToString("f4", CultureInfo.InvariantCulture) + ";")
                .Append(headPosition.x.ToString("f4", CultureInfo.InvariantCulture) + ";")
                .Append(headPosition.y.ToString("f4", CultureInfo.InvariantCulture) + ";")
                .Append(headPosition.z.ToString("f4", CultureInfo.InvariantCulture) + ";")
                .Append(headRotation.x.ToString("f4", CultureInfo.InvariantCulture) + ";")
                .Append(headRotation.y.ToString("f4", CultureInfo.InvariantCulture) + ";")
                .Append(headRotation.z.ToString("f4", CultureInfo.InvariantCulture) + ";");
           
           

            if (handInteraction == GameConfiguration.HandInteraction.Right || handInteraction == GameConfiguration.HandInteraction.Both)
            {
                AppendHandData(stringBuilder, handDetectedR, highConfidenceR, handPositionR, handRotationR, handVelocityR, handRaySourcePositionR, handRayTargetPositionR, handRayRotationR, wristTwistR);
            }

            if (handInteraction == GameConfiguration.HandInteraction.Left || handInteraction == GameConfiguration.HandInteraction.Both)
            {
                AppendHandData(stringBuilder, handDetectedL,highConfidenceL, handPositionL, handRotationL, handVelocityL, handRaySourcePositionL, handRayTargetPositionL, handRayRotationL, wristTwistL);
            }

            return stringBuilder.ToString();
        }

        private void AppendHandData(StringBuilder stringBuilder, bool handDetected, bool highConfidence, Vector3 position,
            Vector3 rotation, Vector3 velocity, Vector3 raySourcePosition, Vector3 rayTargetPosition, Vector3 rayRotation, float twist)
        {
            stringBuilder
                .Append( handDetected+ ";")
                .Append( highConfidence+ ";") 
                .Append(position.x.ToString("f4", CultureInfo.InvariantCulture) + ";")
                .Append(position.y.ToString("f4", CultureInfo.InvariantCulture) + ";")
                .Append(position.z.ToString("f4", CultureInfo.InvariantCulture) + ";")
                .Append(rotation.x.ToString("f4", CultureInfo.InvariantCulture) + ";")
                .Append(rotation.y.ToString("f4", CultureInfo.InvariantCulture) + ";")
                .Append(rotation.z.ToString("f4", CultureInfo.InvariantCulture) + ";")
                .Append(velocity.x.ToString("f4", CultureInfo.InvariantCulture) + ";")
                .Append(velocity.y.ToString("f4", CultureInfo.InvariantCulture) + ";")
                .Append(velocity.z.ToString("f4", CultureInfo.InvariantCulture) + ";");
            if (withRay)
            {
                stringBuilder
                    .Append(raySourcePosition.x.ToString("f4", CultureInfo.InvariantCulture) + ";")
                    .Append(raySourcePosition.y.ToString("f4", CultureInfo.InvariantCulture) + ";")
                    .Append(raySourcePosition.z.ToString("f4", CultureInfo.InvariantCulture) + ";")
                    .Append(rayTargetPosition.x.ToString("f4", CultureInfo.InvariantCulture) + ";")
                    .Append(rayTargetPosition.y.ToString("f4", CultureInfo.InvariantCulture) + ";")
                    .Append(rayTargetPosition.z.ToString("f4", CultureInfo.InvariantCulture) + ";")
                    .Append(rayRotation.x.ToString("f4", CultureInfo.InvariantCulture) + ";")
                    .Append(rayRotation.y.ToString("f4", CultureInfo.InvariantCulture) + ";")
                    .Append(rayRotation.z.ToString("f4", CultureInfo.InvariantCulture) + ";");
            }
           
            stringBuilder.Append(twist.ToString("f4", CultureInfo.InvariantCulture) + ";");
        }

    }
}