public static class RehabConstants
{

    /**
     * HandGrabInteractor's tag
     */
    public const string HandGrabInteracionR = "HandGrabInteractorR";
    public const string HandGrabInteracionL = "HandGrabInteractorL";
    //BBT
    public const string DirBbt = "BoxAndBlock";
    public const string InitBox = "InitBox";
    public const string Desk = "Desk";
    public const string Box = "Box";

    public const string DirExplorador = "Explorador";
    public const string DirGlowCubes = "GlowCubes";
    public const string DirHandBall = "HGTrainer";
    public const string DirTracking = "TrackingData";
    //HandBall
    public const string DirCompareFiability = "CompareFiability";
    public const string DirReactionSpeed = "ReactionSpeed";
    
    public const string FileConfiguration = "config.json";
    public const string FileCalibrationConfigurationHistory = "calibrationHistory.json";
    public const string DefaultUser = "default";
    public const string BlockMesh = "BlockMesh";
    public const string DateFormat = "_yyyyMMdd_HHmmss";
    public const string ExtensionCsv = ".csv";
    public const string ExtensionPng = ".png";
    
    public const string HandPalm = "HandPalm";
    public const string HandWrist = "HandWrist";
    public const string Right = "Right";
    public const string Left = "Left";
    public const string GameBbt = "Box And Block";
    public const string GameExplorer = "Explorer";
    
    //Menu: configuration    
    public const string HandRight = "RHand";
    public const string HandLeft = "LHand";
    public const string HandConfig = "HandConfiguration";
    
    //GlowCubes
    public const string GOKeySecuenceMode = "Secuence";
    public const string GOKeySecuenceNumber = "NumElements";
    //Random sequence mode
    public const string GOValueSequenceRandomStr = "Random";
    //Ascending sequence mode -- default
    public const string GoValueSequenceAscendingStr = "Ascending";
    //Normal sequence mode
    public const string GoValueSequenceNormalStr = "Normal";
    public const int GOKeySequenceRandom = -1;
    public const int GoValueSequenceAscending = 0;
    public const string GOKeyBlocksConfiguration = "BlocksConfiguration";
    public const string GOKeyGuided = "Guided";
    public const string GOKeyPlaneConfiguration = "PlaneConfiguration";
    public const string GOValueHorizontal = "Horizontal";
    public const string GOValueVertical = "Vertical";
    public const string GOKeyLevelCompleted = "LevelCompleted";
    public const string GOKeyWithHelp = "WithHelp";
    public const string GOKeyWithRotation = "Rotation";
    
    //HMD position and Rotation
    public const string HMDPositionX = "HMDPositionX";
    public const string HMDPositionY = "HMDPositionY";
    public const string HMDPositionZ = "HMDPositionX";
    public const string HMDRotationX = "HMDPositionZ";
    public const string HMDRotationY = "HMDRotationY";
    public const string HMDRotationZ = "HMDRotationZ";
    //HandBall
    
    public enum HandInteraction
    {
        Right,
        Left,
        Both
    }

}

