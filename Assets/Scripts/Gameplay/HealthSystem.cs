using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public static HealthSystem Instance { get; private set; }

    [SerializeField]
    private Image healthImage;

    [SerializeField]
    private int startHealth = 5;

    [SerializeField]
    private int maxStartHealth = 5;

    [Header("Player")]
    [SerializeField]
    private GameObject backgroundHealthContainer;

    [SerializeField]
    private GameObject fullHealthContainer;

    [Header("Boss")]
    [SerializeField]
    private GameObject backgroundBossContainer;

    [SerializeField]
    private GameObject fullBossContainer;

    private int _maxHealth;
    private int _currentHealth;

    public void SetMaxHealthPlayer(int value)
    {
        _maxHealth = value;

        var transforms = backgroundHealthContainer.GetComponentsInChildren<Transform>().Skip(1).ToArray();
        for (var i = 0; i < transforms.Length; i++)
        {
            Destroy(transforms[i].gameObject);
        }

        _healthObjects.ToList().ForEach(n =>
        {
            Destroy(n.gameObject);
        });

        BuildBackgroundPlayer();
        BuildHealthPlayer();
    }

    private void BuildBackgroundPlayer()
    {
        for (var i = 0; i < _maxHealth; i++)
        {
            var image = Instantiate(healthImage, backgroundHealthContainer.transform);
            image.color = Color.black;
        }
    }

    private void BuildHealthPlayer()
    {
        _healthObjects.Clear();
        for (var i = 0; i < _currentHealth; i++)
        {
            var obj = Instantiate(healthImage, fullHealthContainer.transform);
            _healthObjects.Push(obj);
        }
    }

    public void SetCurrentHealthPlayer(int value)
    {
        _currentHealth = value;

        _currentHealth = Math.Min(_currentHealth, _maxHealth);

        _healthObjects.ToList().ForEach(n =>
        {
            Destroy(n.gameObject);
        });

        _healthObjects.Clear();
        BuildHealthPlayer();
    }

    private readonly Stack<Image> _healthObjects = new();

    private void Awake()
    {
        _currentHealth = Math.Min(startHealth, maxStartHealth);
        _maxHealth = maxStartHealth;

        BuildBackgroundPlayer();
        BuildHealthPlayer();

        Instance = this;
    }

    public void TakeDamagePlayer(int damage)
    {
        _currentHealth -= damage;
        _currentHealth = Math.Max(_currentHealth, 0);

        if (_healthObjects.TryPop(out var obj))
        {
            Destroy(obj.gameObject);
        }
    }

    private int _maxBossHealth;
    private int _currentBossHealth;
    private readonly Stack<Image> _healthObjectsBoss = new();

    public void ShowBossHealth(int health, int maxBossHealth)
    {
        HideBossHealth();
        _maxBossHealth = maxBossHealth;
        _currentBossHealth = health;

        BuildBackgroundBoss();
        BuildHealthBoss();
    }

    private void BuildBackgroundBoss()
    {
        for (var i = 0; i < _maxBossHealth; i++)
        {
            var image = Instantiate(healthImage, backgroundBossContainer.transform);
            image.color = Color.black;
        }
    }

    private void BuildHealthBoss()
    {
        _healthObjectsBoss.Clear();
        Debug.Log("Wird gecalled");
        for (var i = 0; i < _currentBossHealth; i++)
        {
            var obj = Instantiate(healthImage, fullBossContainer.transform);
            _healthObjectsBoss.Push(obj);
        }
    }

    public void HideBossHealth()
    {
        var transforms = backgroundBossContainer.GetComponentsInChildren<Transform>().Skip(1).ToArray();
        for (var i = 0; i < transforms.Length; i++)
        {
            Destroy(transforms[i].gameObject);
        }

        _healthObjectsBoss.ToList().ForEach(n =>
        {
            Destroy(n.gameObject);
        });

        _healthObjectsBoss.Clear();
    }

    public void AddHealthBoss(int health)
    {
        _currentBossHealth += health;
        _currentBossHealth = Math.Min(_currentBossHealth, _maxBossHealth);

        _healthObjectsBoss.Clear();
        BuildHealthBoss();
    }

    public void TakeDamageBoss(int damage)
    {
        _currentBossHealth -= damage;
        _currentBossHealth = Math.Max(_currentBossHealth, 0);

        if (_healthObjectsBoss.TryPop(out var obj))
        {
            Destroy(obj.gameObject);
        }
    }
}
