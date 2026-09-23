using System;
using UnityEngine;

public class PuddleScript : MonoBehaviour
{
    private bool _playerIsInPuddle;

    private void OnDestroy()
    {
        _playerIsInPuddle = false;
    }

    [SerializeField]
    private float damageFrequency = 2f;
    
    [SerializeField]
    private float aliveTime = 4f;

    private float _timer;

    private void Awake()
    {
        _timer = damageFrequency - 0.1f;

        Destroy(gameObject, aliveTime);
    }

    private void Update()
    {
        if (_playerIsInPuddle)
        {
            _timer += Time.deltaTime;
            if (_timer >= damageFrequency)
            {
                _timer = 0;
                Player.Instance.TakeDamage(1);
            }
        }
        else
        {
            _timer = damageFrequency - 0.1f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            OnPlayerEnterPuddle();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
            OnPlayerExitPuddle();
    }

    private void OnPlayerEnterPuddle()
    {
        _playerIsInPuddle = true;
    }

    private void OnPlayerExitPuddle()
    {
        _playerIsInPuddle = false;
    }
}
