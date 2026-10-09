using UnityEngine;

namespace ThreeInOne.Games.Flying
{

public class SpinPropellerX : MonoBehaviour
{
    public float spinSpeed = 1000f;

    void Update()
    {
        transform.Rotate(Vector3.forward * spinSpeed * Time.deltaTime);
    }
}

}
