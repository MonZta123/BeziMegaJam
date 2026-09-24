using System;
using System.Collections;
using Gameplay.ReferenceScripts;
using UI;
using UnityEngine;

public abstract class BossMonoBehaviour : MonoBehaviour
{
    [SerializeField]
    private int health;

    [SerializeField]
    private SkinnedMeshRenderer meshRenderer;

    [SerializeField]
    private Material flashMaterial;

    private Material[] _materials;
    private Material[] _flashMaterials;
    
    [SerializeField]
    private BurgerPart reward;
    private AudioSource _deathSound;
    public bool IsDead { get; private set; }

    protected void OnDestroy()
    {
        HealthSystem.Instance.HideBossHealth();

        IsDead = true;
        
        Burger currentBurger = Burger.CurrentBurger;
        if (currentBurger)
            currentBurger.AddBurgerPart(reward);
    }

    public abstract void OnDeath();

    public static BossMonoBehaviour Instance { get; private set; }

    protected virtual void Awake()
    {
        HealthSystem.Instance.ShowBossHealth(health, health);
        _deathSound = GetComponentInChildren<AudioSource>();

        _materials = meshRenderer.sharedMaterials;
        _flashMaterials = new Material[_materials.Length];

        for (var i = 0; i < _flashMaterials.Length; i++)
            _flashMaterials[i] = flashMaterial;

        Instance = this;
        
        _currentHealth = health;
        _maxHealth = health;
    }

    public virtual void AddHealth(int amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
        
        HealthSystem.Instance.ShowBossHealth(_currentHealth, _maxHealth);
    }

    private int _maxHealth;
    private int _currentHealth;
    
    public virtual void TakeDamage(int amount)
    {
        StartCoroutine(FlashDamage(0.1f));
        _currentHealth -= amount;
        _currentHealth = Mathf.Max(_currentHealth, 0);
        
        HealthSystem.Instance.TakeDamageBoss(amount);

        if (_currentHealth <= 0)
        {
            _deathSound.Play();
            Player.Instance.GoBackToKitchen(gameObject);
        }

        StartCoroutine(FlashDamage(0.2f));
    }

    public IEnumerator FlashDamage(float duration)
    {
        meshRenderer.sharedMaterials = _flashMaterials;

        yield return new WaitForSeconds(duration);

        meshRenderer.sharedMaterials = _materials;
    }
}
