using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField]
    private float timeToDestroy = 2.0f;

    private void Awake()
    {
        Destroy(gameObject, timeToDestroy);
    }
}
