using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resize3DModel : MonoBehaviour
{
  public Transform hmdTransform;
  public GameObject gkModel;
  //private float heightTallThreshold = 1.7f;
  //private float heightShortThreshold = 1.45f;
  //private Vector3 scaledTallSize = new Vector3(1.1f, 1.1f, 1.1f);
  //private Vector3 scaledShortSize = new Vector3(0.9f, 0.9f, 0.9f);
  private float defaultHeight = 1.65f;

  void Start()
  {
    StartCoroutine(InitializeScale());
  }

  private IEnumerator InitializeScale()
  {
    // Wait for one frame to ensure VR system is initialized
    yield return null;
    AdjustModelScale(hmdTransform.position);
  }

  private void AdjustModelScale(Vector3 hmdPosition)
  {
    Debug.Log("HMD Y value: " + hmdPosition.y);
    float scaleFactor = hmdPosition.y / defaultHeight;
    gkModel.transform.localScale = new Vector3(1, scaleFactor, 1);

    /*
    3 models
        if (hmdPosition.y > heightTallThreshold)
        {
          gkModel.transform.localScale = scaledTallSize;
        }
        else if (hmdPosition.y < heightShortThreshold)
        {
          gkModel.transform.localScale = scaledShortSize;
        }
        else
        {
          gkModel.transform.localScale = Vector3.one;
        }*/

  }

}
