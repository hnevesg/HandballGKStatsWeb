using System;
using UnityEngine;

[Serializable] 
    public class CalibrationConfiguration
    {
        /**
     * Variables used to detect avoid movements
     */
        public Vector3 hmdInitialPosition;
        public Quaternion hmdInitialRotation;
        public float trunkDisplacementThreshold_X;
        public float trunkDisplacementThreshold_Z;
        
        public float neckFlexionThreshold_X;
        public float neckFlexionThreshold_Y;
        public float neckFlexionThreshold_Z;
        public float armSupinationThreshold;


        public CalibrationConfiguration(Vector3 hmdInitialPosition, Quaternion hmdInitialRotation, float trunkDisplacementThresholdX, 
            float trunkDisplacementThresholdZ, float neckFlexionThresholdX, float neckFlexionThresholdY, float neckFlexionThresholdZ,  float armSupinationThreshold)
        {
            this.hmdInitialPosition = hmdInitialPosition;
            this.hmdInitialRotation = hmdInitialRotation;
            trunkDisplacementThreshold_X = trunkDisplacementThresholdX;
            trunkDisplacementThreshold_Z = trunkDisplacementThresholdZ;
            neckFlexionThreshold_X = neckFlexionThresholdX;
            neckFlexionThreshold_Y = neckFlexionThresholdY;
            neckFlexionThreshold_Z = neckFlexionThresholdZ;
            this.armSupinationThreshold = armSupinationThreshold;
        }
        

        public CalibrationConfiguration() : 
            this(Vector3.zero, Quaternion.identity, 
                0.1f, 0.1f, 
                0f, 0f, 0f, 0f)
        {
        }

        public Vector3 HmdInitialPosition
        {
            get => hmdInitialPosition;
            set => hmdInitialPosition = value;
        }

        public Quaternion HmdInitialRotation
        {
            get => hmdInitialRotation;
            set => hmdInitialRotation = value;
        }

        public float TrunkDisplacementThresholdX
        {
            get => trunkDisplacementThreshold_X;
            set => trunkDisplacementThreshold_X = value;
        }

        public float TrunkDisplacementThresholdZ
        {
            get => trunkDisplacementThreshold_Z;
            set => trunkDisplacementThreshold_Z = value;
        }

        public float NeckFlexionThresholdX
        {
            get => neckFlexionThreshold_X;
            set => neckFlexionThreshold_X = value;
        }

        public float NeckFlexionThresholdY
        {
            get => neckFlexionThreshold_Y;
            set => neckFlexionThreshold_Y = value;
        }

        public float NeckFlexionThresholdZ
        {
            get => neckFlexionThreshold_Z;
            set => neckFlexionThreshold_Z = value;
        }

        public float ARMSupinationThreshold
        {
            get => armSupinationThreshold;
            set => armSupinationThreshold = value;
        }
    }
