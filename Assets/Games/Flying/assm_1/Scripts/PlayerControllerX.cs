using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeInOne.Games.Flying
{

public class PlayerControllerX : MonoBehaviour
{
    public float speed = 15.0f;
    public float rotationSpeed = 10.0f;

    [SerializeField, Range(5.0f, 80.0f)]
    private float maxPitch = 35.0f;

    [SerializeField, Min(1.0f)]
    private float autoLevelSpeed = 80.0f;

    [SerializeField, Range(0.0f, 0.5f)]
    private float inputDeadZone = 0.05f;

    private float verticalInput;
    private float currentPitch;
    private Quaternion levelRotation;

    // Start is called before the first frame update
    void Start()
    {
        levelRotation = transform.localRotation;
        currentPitch = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Raw input removes the legacy Input Manager's smoothing tail after a key is released.
        verticalInput = Input.GetAxisRaw("Vertical") * -1.0f;

        if (Mathf.Abs(verticalInput) > inputDeadZone)
        {
            currentPitch += verticalInput * rotationSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, -maxPitch, maxPitch);
        }
        else
        {
            // Return to the plane's starting orientation so forward motion does not
            // continue climbing or diving after the player releases the controls.
            currentPitch = Mathf.MoveTowards(currentPitch, 0.0f, autoLevelSpeed * Time.deltaTime);
        }

        transform.localRotation = levelRotation * Quaternion.Euler(currentPitch, 0.0f, 0.0f);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}

}
