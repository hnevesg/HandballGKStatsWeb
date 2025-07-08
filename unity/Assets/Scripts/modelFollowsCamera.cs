using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class modelFollowsCamera : MonoBehaviour
{
    public Transform gk;
    [SerializeField]
    private Vector3 offset;

    void LateUpdate()
    {
        transform.position = gk.position + offset;
    }
}
