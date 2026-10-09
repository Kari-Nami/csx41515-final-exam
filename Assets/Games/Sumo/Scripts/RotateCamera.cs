using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeInOne.Games.Sumo
{

public class RotateCamera : MonoBehaviour
{
    public float RotationSpeed = 10.0f;


    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * (Time.deltaTime * RotationSpeed * horizontalInput));
    }
}

}
