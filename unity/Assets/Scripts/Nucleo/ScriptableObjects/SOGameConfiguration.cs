
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SOGameConfiguration", order = 1)]
public class SOGameConfiguration : ScriptableObject
{
 
  //if autogrip, grabIdentifier: the grabbed block unique identifier.
   public string grabIdentier;
   public bool moveCorrectly;
   public bool gameStopped;
   public bool gameStarts;
   public string userPath;
   public string gameName;
   public GameConfiguration gameConfiguration;
   public CalibrationConfiguration calibrationConfiguration;

    //Default values
    private SOGameConfiguration()
    {
      resetAll();
    }

    public void resetAll()
    {
      grabIdentier = "";
      gameConfiguration = new GameConfiguration();
      calibrationConfiguration = new CalibrationConfiguration();
      userPath = "";
      gameStarts = gameStopped = moveCorrectly= false;
      

    }

    public string GameName
    {
      get => gameName;
      set => gameName = value;
    }
}