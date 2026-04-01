using UnityEngine;
using System;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Player Combat Settings")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRadius;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField, Range(20, 60)] private int[] comboWindowFrameAmount = { 20, 20, 20 };
    [SerializeField] private int[] _weaponDamage = { 1, 2, 2 };

    private bool _hitboxActive;
    private bool _comboWindowOpen = false;
    [SerializeField]private int _currentCombo;
    private Coroutine _comboWindowRoutine;

    // Events
    public static event Action<Collider2D[], int> OnHit;
    public static event Action<bool> OnComboWindowOpen;
    public static event Action<PlayerController.PlayerState> OnComboChange;


    // System
    private void OnEnable()
    {
        PlayerController.OnStateChanged += HandleStateChange;
        PlayerController.OnAttackPressed += TryCombo;
    }

    private void OnDisable()
    {
        PlayerController.OnStateChanged -= HandleStateChange;
        PlayerController.OnAttackPressed -= TryCombo;
    }


    // Chamados pelo Animation Event
    public void EnableHitbox()
    {
        _hitboxActive = false;
        _hitboxActive = true;
        CheckHit();
    }

    public void DisableHitbox()
    {
        _hitboxActive = false;
    }

    /// <summary>
    /// Verifica colisores dentro da área de ataque e dispara OnHit se houver contato.
    /// </summary>
    private void CheckHit()
    {
        int currentDamage = _weaponDamage[Mathf.Clamp(_currentCombo - 1, 0, _weaponDamage.Length - 1)];
        Collider2D[] hit = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRadius, _enemyLayer);
        if (hit.Length > 0)
            OnHit?.Invoke(hit, currentDamage);
    }


    // Combo mechanics
    private void OpenComboWindow() // chamado pelo Animation Event
    {
        _comboWindowOpen = true;
        OnComboWindowOpen?.Invoke(_comboWindowOpen);

        if (_comboWindowRoutine != null)
        {
            StopCoroutine(_comboWindowRoutine);
            _comboWindowRoutine = null;
        }

        int index = Mathf.Clamp(_currentCombo - 1, 0, comboWindowFrameAmount.Length - 1);
        _comboWindowRoutine = StartCoroutine(ComboTimeOut(comboWindowFrameAmount[index]));
    }

    private IEnumerator ComboTimeOut(float windowOpenTimerInFrames)
    {
        int comboAtStart = _currentCombo;
        for (int i = 0; i < windowOpenTimerInFrames; i++)
            yield return null;

        // Se não houve novo ataque durante a janela, encerra o combo
        if (_currentCombo == comboAtStart)
            OnComboChange?.Invoke(PlayerController.PlayerState.Idle);

        _comboWindowOpen = false;
        OnComboWindowOpen?.Invoke(_comboWindowOpen);
    }


    // Reage a mudanças de estado
    private void HandleStateChange(PlayerController.PlayerState state)
    {
        bool isAttacking = state is PlayerController.PlayerState.Attacking01
                                  or PlayerController.PlayerState.Attacking02
                                  or PlayerController.PlayerState.Attacking03;

        if (!isAttacking)
        {
            _hitboxActive = false;
            _currentCombo = 0;

            if (_comboWindowRoutine != null)
            {
                StopCoroutine(_comboWindowRoutine);
                _comboWindowRoutine = null;
                _comboWindowOpen = false;
                OnComboWindowOpen?.Invoke(_comboWindowOpen);
            }
        }
    }

    private void TryCombo()
    {
        if (_currentCombo == 3) return;
        _hitboxActive = false;

        switch (_currentCombo)
        {
            case 0:
                _currentCombo = 1;
                _comboWindowRoutine = null;
                OnComboChange?.Invoke(PlayerController.PlayerState.Attacking01);
                break;
            case 1:
                if (!_comboWindowOpen) return;
                _currentCombo = 2;
                OnComboChange?.Invoke(PlayerController.PlayerState.Attacking02);
                break;
            case 2:
                if (!_comboWindowOpen) return;
                _currentCombo = 3;
                OnComboChange?.Invoke(PlayerController.PlayerState.Attacking03);
                break;
        }
    }


    // Editor
    private void OnDrawGizmos()
    {
        if (_hitboxActive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRadius);
        }
    }
}