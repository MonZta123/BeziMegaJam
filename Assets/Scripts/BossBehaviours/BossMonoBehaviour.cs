using System;
using System.Collections;
using UI;
using UnityEngine;

public abstract class BossMonoBehaviour : MonoBehaviour
{
    [SerializeField]
    private int health;

    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private Material flashMaterial;

    private Material[] _materials;
    private Material[] _flashMaterials;

    private void OnDestroy()
    {
        HealthSystem.Instance.HideBossHealth();
    }

    public abstract void OnDeath();

    private void Awake()
    {
        HealthSystem.Instance.ShowBossHealth(health);

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
