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
    protected void OnDestroy()
    {
        HealthSystem.Instance.HideBossHealth();
        
        Burger.CurrentBurger.AddBurgerPart(reward);
    }

    public abstract void OnDeath();

    protected virtual void Awake()
    {
        HealthSystem.Instance.ShowBossHealth(health);
        _deathSound = GetComponentInChildren<AudioSource>();
        Debug.Log("Awake Wird gecalled");

        _materials = meshRenderer.sharedMaterials;
        _flashMaterials = new Material[_materials.Length];

        for (var i = 0; i < _flashMaterials.Length; i++)
            _flashMaterials[i] = flashMaterial;
    }

    public virtual void TakeDamage(int amount)
    {
        StartCoroutine(FlashDamage(0.1f));
        health -= amount;
        health = Mathf.Max(health, 0);
        
        HealthSystem.Instance.TakeDamageBoss(amount);

        if (health <= 0)
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
