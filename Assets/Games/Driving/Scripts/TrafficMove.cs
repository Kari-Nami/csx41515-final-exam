using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeInOne.Games.Driving
{

public class TrafficMove : MonoBehaviour
{
    public float speed = 10.0f;
    
    void Update()
    {
        transform.Translate(0, 0, speed);
    }
}

}
