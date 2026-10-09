using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeInOne.Games.Flying
{

public class PlayerControllerX : MonoBehaviour
{
    public float speed = 15.0f;
    public float rotationSpeed = 10.0f;
    private float verticalInput;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    // Get the player's vertical input
    verticalInput = Input.GetAxis("Vertical") * -1;
    // Move the plane forward continuously
    transform.Translate(Vector3.forward * speed * Time.deltaTime);

    // Tilt the plane only when there is input
    transform.Rotate(Vector3.right * rotationSpeed * verticalInput * Time.deltaTime);
    }
}

}
