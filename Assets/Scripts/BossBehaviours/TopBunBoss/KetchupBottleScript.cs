using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class KetchupBottleScript : MonoBehaviour
{
    [SerializeField]
    private float flightDuration = 1.5f;
    
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private GameObject puddle;
    
    [SerializeField]
    private float torqueStrength = 10f;

    private void OnValidate()
    {
        if (!rb)
            rb = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        var playerPos = Player.Instance.transform.position;
        var target = new Vector3(playerPos.x, 0f, playerPos.z);

        var start = transform.position;

        var t = flightDuration;
        
        var horizontalTarget = new Vector3(
            target.x,
            start.y,
            target.z
        );

        var horizontalVelocity = (horizontalTarget - start) / t;

        var verticalVelocity =
            (target.y - start.y - 0.5f * Physics.gravity.y * t * t) / t;

        var velocity = horizontalVelocity;
        velocity.y = verticalVelocity;

        rb.AddTorque(Random.insideUnitSphere * torqueStrength, ForceMode.Impulse);
        
        rb.linearVelocity = velocity;
    }

    public void OnCollisionEnter(Collision other)
    {
        var pos = transform.position;
        pos.y = 0;
        Instantiate(puddle, pos, Quaternion.identity);
        Destroy(gameObject);
    }
}
