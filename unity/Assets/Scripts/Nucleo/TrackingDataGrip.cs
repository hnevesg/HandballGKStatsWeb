using System;
using System.Globalization;
using Vector3 = UnityEngine.Vector3;

namespace Tracking
{
    [Serializable]
    public class TrackingDataGrip
    {
        public Vector3 PalmPosition { get; set; }
        public bool IsPinchGrabbing { get; set; }
        public bool IsPalmGrabbing { get; set; }
        public bool IsAutogrip { get; set; }
        public float StrengthThumbIndex { get; set; }
        public float StrengthThumbMiddle { get; set; }
        public float StrengthThumbRing { get; set; }
        public float StrengthThumbPinky { get; set; }
        public float StrengthPalmIndex { get; set; }
        public float StrengthPalmMiddle { get; set; }
        public float StrengthPalmRing { get; set; }
        public float StrengthPalmPinky { get; set; }
        public string GrabbedElementIdentifier { get; set; }
        public bool GrabbedElementIsMovedCorrectly { get; set; }


        public override string ToString()
        {
            return
                PalmPosition.x.ToString("f4", CultureInfo.InvariantCulture) + ";"
                                                                            + PalmPosition.y.ToString("f4",
                                                                                CultureInfo.InvariantCulture) + ";"
                                                                            + PalmPosition.z.ToString("f4",
                                                                                CultureInfo.InvariantCulture) + ";"
                                                                            + IsPinchGrabbing.ToString() + ";"
                                                                            + IsPalmGrabbing.ToString() + ";"
                                                                            + IsAutogrip.ToString() + ";"
                                                                            + StrengthThumbIndex.ToString("f4",
                                                                                CultureInfo.InvariantCulture) + ";"
                                                                            + StrengthThumbMiddle.ToString("f4",
                                                                                CultureInfo.InvariantCulture) + ";"
                                                                            + StrengthThumbRing.ToString("f4",
                                                                                CultureInfo.InvariantCulture) + ";"
                                                                            + StrengthThumbPinky.ToString("f4",
                                                                                CultureInfo.InvariantCulture) + ";"
                                                                            + StrengthPalmIndex.ToString("f4",
                                                                                CultureInfo.InvariantCulture) + ";"
                                                                            + StrengthPalmMiddle.ToString("f4",
                                                                                CultureInfo.InvariantCulture) + ";"
                                                                            + StrengthPalmRing.ToString("f4",
                                                                                CultureInfo.InvariantCulture) + ";"
                                                                            + StrengthPalmPinky.ToString("f4",
                                                                                CultureInfo.InvariantCulture) + ";"
                                                                            + GrabbedElementIdentifier + ";"
                                                                            + GrabbedElementIsMovedCorrectly
                                                                                .ToString() + ";";
        }


        public void SetStrength(Oculus.Interaction.Input.HandFinger finger, float actualStregth, bool palm)
        {
            switch (finger)
            {
                case Oculus.Interaction.Input.HandFinger.Index:
                    if (palm)
                        StrengthPalmIndex = actualStregth;
                    else
                    {
                        StrengthThumbIndex = actualStregth;
                    }

                    break;
                case Oculus.Interaction.Input.HandFinger.Middle:
                    if (palm)
                        StrengthPalmMiddle = actualStregth;
                    else
                    {
                        StrengthThumbMiddle = actualStregth;
                    }

                    break;
                case Oculus.Interaction.Input.HandFinger.Pinky:
                    if (palm)
                        StrengthPalmPinky = actualStregth;
                    else
                    {
                        StrengthThumbPinky = actualStregth;
                    }

                    break;
                case Oculus.Interaction.Input.HandFinger.Ring:
                    if (palm)
                        StrengthPalmRing = actualStregth;
                    else
                    {
                        StrengthThumbRing = actualStregth;
                    }

                    break;
            }
        }

        public TrackingDataGrip()
        {
            IsPinchGrabbing = IsPalmGrabbing = IsAutogrip = GrabbedElementIsMovedCorrectly = false;
            StrengthThumbIndex = StrengthThumbMiddle = StrengthThumbRing = StrengthThumbPinky = 0f;
            StrengthPalmIndex = StrengthPalmMiddle = StrengthPalmRing = StrengthPalmPinky = 0f;
            GrabbedElementIdentifier = "";
        }
    }
}