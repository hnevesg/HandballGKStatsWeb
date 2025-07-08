using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static float sharedTime = 0f;
    public static int sharedFrame = 0;

    void Update()
    {
        sharedFrame = Time.frameCount;
        sharedTime += Time.deltaTime;
    }
}
