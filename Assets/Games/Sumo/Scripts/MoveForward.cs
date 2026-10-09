using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ThreeInOne.Games.Sumo
{

public class MoveForward : MonoBehaviour
{ 
    public float Speed = 3.0f;

    [HideInInspector]
    public Vector3 LookDirection;


    private void Start()
    {
        Vector3 startPosition = transform.position;
        transform.Translate(Vector3.up);
        LookDirection = transform.position - startPosition;
        transform.position = startPosition;
    }


    void Update()
    {
        transform.Translate(new Vector3(0, Time.deltaTime * Speed, 0));
    }
}

}
