using System;
using System.Collections.Generic;
using UnityEngine;



[Serializable] public class RehabDictionary : SerializableDictionary<string, Vector3> { }
[Serializable] public class RehabDictionaryObject : SerializableDictionary<string, string> { }
[Serializable] 
public class GameConfiguration
{
    public string userId;
    public int timer;
    //hand right, left or both
    public HandInteraction handInteraction;
    public int level;
    public bool autogripR;
    public bool autogripL;
    public RehabDictionary objectsPosition;
    //Interaction type
    public bool grabInteraction;
    public bool rayInteraction;
    public RehabDictionaryObject gameOptions;
 
    
    public enum HandInteraction
    {
        Right,
        Left,
        Both
    }

    public RehabDictionaryObject GameOptions
    {
        get => gameOptions;
        set => gameOptions = value;
    }

    public string UserId
    {
        get => userId;
        set => userId = value;
    }

    public int Timer
    {
        get => timer;
        set => timer = value;
    }


    public void AddOrUpdateOption(string keyConfiguration, string valueConfiguration )
    {
        if (gameOptions != null)
        {
            if (gameOptions.ContainsKey(keyConfiguration))
            {
                //update
                gameOptions[keyConfiguration] = valueConfiguration;
            }
            else
            {
                //add new object position
                gameOptions.Add(keyConfiguration, valueConfiguration);
            }
        }
    }
    
    public void AddOrUpdatePosition(string key, Vector3 position )
    {
        
        if (objectsPosition != null)
        {
            if (objectsPosition.ContainsKey(key))
            {
                //update
                objectsPosition[key] = position;
            }
            else
            {
                //add new object position
                objectsPosition.Add(key, position);
            }
        }
    }
    
  

    public int Level
    {
        get => level;
        set => level = value;
    }

    public bool AutogripR
    {
        get => autogripR;
        set => autogripR = value;
    }

    public bool AutogripL
    {
        get => autogripL;
        set => autogripL = value;
    }

    public Dictionary<string, Vector3> ObjectsPosition => objectsPosition;

    public bool GrabInteraction
    {
        get => grabInteraction;
        set => grabInteraction = value;
    }
    
      public bool RayInteracion
        {
            get => rayInteraction;
            set => rayInteraction = value;
        }

    //Default values
    public GameConfiguration()
    {
        timer = 60;
        level = 1;
        objectsPosition = new RehabDictionary();
        gameOptions = new RehabDictionaryObject();
        autogripR = autogripL =  false;
        handInteraction = HandInteraction.Right;
        userId = RehabConstants.DefaultUser;
        level = 1;
        grabInteraction = true;

    }

}