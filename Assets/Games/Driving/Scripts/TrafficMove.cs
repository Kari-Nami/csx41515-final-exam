using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeInOne.Games.Driving
{

public class TrafficMove : MonoBehaviour
{
    [Tooltip("Forward movement speed in world units per second.")]
    public float speed = 10.0f;

    void Update()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime), Space.Self);
    }
}

}
