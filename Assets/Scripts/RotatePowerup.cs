using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePowerup : MonoBehaviour
{
    private float rotateSpeed = 1f;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(0, rotateSpeed, 0, Space.World);    
    }
}
