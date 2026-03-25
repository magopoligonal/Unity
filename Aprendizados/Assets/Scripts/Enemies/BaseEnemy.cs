using System;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    [Header("Enemies Parameters")]
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _armor;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _movementModifier; //será que conseguimos por terrenos diferentes? ou um estado de frenesi? acho mais complexo essa interação mas quem sabe no futuro.
    
    // Propriedades
    public int Health
    {
        get => _currentHealth;
        private set => _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
    }
    
    public int Armor{get => _armor; private set => _armor = value;}
    
    //métodos system
    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    void OnEnable()
    {
        PlayerCombat.OnHit += HandleHit;
    }

    void OnDisable()
    {
        PlayerCombat.OnHit -= HandleHit;
    }

    //métodos Interface
    public virtual void TakeDamage(int amount)
    {
        Health -= amount;
    }

    //métodos Abstract
    protected abstract void HandleHit(Collider2D[] collider, int weaponDamage); //dai cada inimigo implementa a forma como vai lidar?
    protected abstract void Die();
}
