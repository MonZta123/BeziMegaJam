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

    [SerializeField]
    private GameObject backgroundHealthContainer;
    
    [SerializeField]
    private GameObject fullHealthContainer;
    
    private int _maxHealth;
    private int _currentHealth;
    
    public void SetMaxHealth(int value)
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
        
        BuildBackground();
        BuildHealth();
    }

    private void BuildBackground()
    {
        for (var i = 0; i < _maxHealth; i++)
        {
            var image = Instantiate(healthImage, backgroundHealthContainer.transform);
            image.color = Color.black;
        }
    }

    private void BuildHealth()
    {
        _healthObjects.Clear();
        for (var i = 0; i < _currentHealth; i++)
        {
            var obj = Instantiate(healthImage, fullHealthContainer.transform);
            _healthObjects.Push(obj);
        }
    }

    public void SetCurrentHealth(int value)
    {
        _currentHealth = value;
        
        _currentHealth = Math.Min(_currentHealth, _maxHealth);
        
        _healthObjects.ToList().ForEach(n =>
        {
            Destroy(n.gameObject);
        });
        
        _healthObjects.Clear();
        BuildHealth();
    }

    private readonly Stack<Image> _healthObjects = new();
    
    private void Awake()
    {
        _currentHealth = Math.Min(startHealth, maxStartHealth);
        _maxHealth = maxStartHealth;

        BuildBackground();
        BuildHealth();

        Instance = this;
    }
    
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _currentHealth = Math.Max(_currentHealth, 0);

        if (_healthObjects.TryPop(out var obj))
        {
            Destroy(obj.gameObject);
        }

        if (_currentHealth <= 0)
        {
            // get cooked
        }
    }
}
