using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Player Health Properties")] 
    [SerializeField] private int _maxFocus = 1; //1 como base? -- 1: confiante, 0: insegura, mais um hit e perde. -- no mario sao 2 se considerar que temos como guardar um item extra, não? 
    [SerializeField] private int _currentFocus;
    [SerializeField] private bool _isHyped = false; //fazendo a alegoria de estar animado com o curso
    public bool IsHyped { get => _isHyped; private set => _isHyped = value; } //tentando me acostumar a syntax
    //events
    /*
     * -- Eventos que vai emitir: -- OnPlayerDeath -- OnDamageTaken
     * Eventos que vai reagir: -- OnNewInformationCollected 
     */
    public static event Action<bool> OnPlayerDeath;
    public static event Action OnDamageTaken;

    private void OnEnable()
    {
        //OnNewInformationCollected += HandleCollectable;
    }

    private void OnDisable()
    {
        //OnNewInformationCollected -= HandleCollectable;
    }

    private void Awake()
    {
        _currentFocus =  _maxFocus;
    }

    public void TakeDamage(int amount)
    {
        //verifica se está muito animada (vida extra)
        if (IsHyped) 
            IsHyped = false;
        else
            _currentFocus -= amount;
        
        OnDamageTaken?.Invoke();
        
        if(_currentFocus < 0)
            OnPlayerDeath?.Invoke(true);
    }

    private void HandleCollectable()
    {
        if (IsHyped) return; //retorna caso já esteja hypada
        
        if(_currentFocus >= _maxFocus)
            IsHyped = true;
        else
            _currentFocus++;
    }
    
    
}
