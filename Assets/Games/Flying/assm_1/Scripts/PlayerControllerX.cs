using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeInOne.Games.Flying
{

public class PlayerControllerX : MonoBehaviour
{
    public float speed = 15.0f;
    public float rotationSpeed = 10.0f;

    [SerializeField, Range(0.0f, 0.5f)]
    private float inputDeadZone = 0.05f;

    private float verticalInput;
    private Rigidbody planeRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        planeRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Raw input stops rotation immediately when the key is released. The plane's
        // current orientation is intentionally preserved so it can loop and fly inverted.
        verticalInput = Input.GetAxisRaw("Vertical") * -1.0f;

        if (Mathf.Abs(verticalInput) > inputDeadZone)
        {
            transform.Rotate(Vector3.right, rotationSpeed * verticalInput * Time.deltaTime, Space.Self);
        }

        // Collisions and direct Transform rotation on a dynamic Rigidbody can leave
        // residual angular velocity. Clear only that velocity; do not alter the angle.
        if (planeRigidbody != null)
        {
            planeRigidbody.angularVelocity = Vector3.zero;
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}

}
