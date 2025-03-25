using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamHolder : MonoBehaviour
{
    public Transform cameraHolder;

    private void Update()
    {
        transform.position = cameraHolder.position;
    }
}
