using UnityEngine;

public class Spinning : MonoBehaviour
{
    [Header("Floating Variables")]
    [SerializeField]
    private float amplitude = 0.5f;

    [SerializeField]
    private float frequency = 1f;

    [SerializeField]
    private float degreePerSecond = 15.0f;

    private Vector3 _startPos;
    private Vector3 _endPos;

    public void Start()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        if (degreePerSecond != 0)
        {
            transform.Rotate(new Vector3(0f, degreePerSecond * Time.deltaTime, 0f), Space.World);
        }

        
    }
}
