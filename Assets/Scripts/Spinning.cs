using UnityEngine;

public class Spinning : MonoBehaviour
{
    [SerializeField]
    private float degreePerSecond = 15.0f;

    private void Update()
    {
        transform.Rotate(Vector3.up, degreePerSecond * Time.deltaTime, Space.World);
    }
}
