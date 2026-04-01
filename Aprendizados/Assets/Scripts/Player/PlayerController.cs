using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        // 0-1 Instantâneos
        Spawning = 0,
        Dead = 1,
        // 2-6 Movimentação
        Idle = 2,
        Walking = 3,
        Running = 4,
        Jumping = 5,
        Falling = 6,
        // 7-x Combate
        Attacking01 = 7,
        Attacking02 = 8,
        Attacking03 = 9,
    }

    // Fields
    [SerializeField] private PlayerState _currentPlayerState = PlayerState.Idle;

    // Properties
    [field: SerializeField] public bool IsGrounded { get; private set; }
    [field: SerializeField] public bool CanCombo { get; private set; }

    // Calculada — lê _currentPlayerState no momento que é consultada, sem Update
    private bool _isAttacking => _currentPlayerState is PlayerState.Attacking01
                                                      or PlayerState.Attacking02
                                                      or PlayerState.Attacking03;

    public PlayerState CurrentPlayerState
    {
        get => _currentPlayerState;
        private set => _currentPlayerState = value;
    }

    // Events
    public static event Action<PlayerState> OnStateChanged;
    public static event Action OnAttackPressed;


    // System
    private void OnEnable()
    {
        // InputManager
        InputManager.OnWalking += Walk;
        InputManager.OnRunning += Run;
        InputManager.OnJump += Jump;
        InputManager.OnAttack += Attack;

        // PlayerController
        OnStateChanged += HandleStateChange;

        // PlayerCollision
        PlayerCollision.OnReachingGround += CheckGround;

        // PlayerMovement
        PlayerMovement.OnFalling += CheckFalling;
        PlayerMovement.OnIdle += CheckIdle;

        // PlayerAnimator
        PlayerAnimator.OnAttackFinished += CheckAttackFinished;

        // PlayerCombat
        PlayerCombat.OnComboWindowOpen += CheckCombo;
        PlayerCombat.OnComboChange += RequestChangeState;
    }

    private void OnDisable()
    {
        // InputManager
        InputManager.OnWalking -= Walk;
        InputManager.OnRunning -= Run;
        InputManager.OnJump -= Jump;
        InputManager.OnAttack -= Attack;

        // PlayerController
        OnStateChanged -= HandleStateChange;

        // PlayerCollision
        PlayerCollision.OnReachingGround -= CheckGround;

        // PlayerMovement
        PlayerMovement.OnFalling -= CheckFalling;
        PlayerMovement.OnIdle -= CheckIdle;

        // PlayerAnimator
        PlayerAnimator.OnAttackFinished -= CheckAttackFinished;

        // PlayerCombat
        PlayerCombat.OnComboWindowOpen -= CheckCombo;
        PlayerCombat.OnComboChange -= RequestChangeState;
    }


    // Input handlers
    private void Attack(InputAction.CallbackContext input)
    {
        if (!input.performed) return;

        if (!_isAttacking || CanCombo)
            OnAttackPressed?.Invoke();
    }

    private void Walk(InputAction.CallbackContext input)
    {
        if (input.performed && input.ReadValue<Vector2>().x != 0)
            RequestChangeState(PlayerState.Walking);
    }

    private void Run(InputAction.CallbackContext input)
    {
        if (input.performed)
            RequestChangeState(PlayerState.Running);
    }

    private void Jump(InputAction.CallbackContext input)
    {
        if (input.performed && IsGrounded)
            RequestChangeState(PlayerState.Jumping);
    }


    // Event listeners
    private void CheckGround(bool hasHitGround)
    {
        IsGrounded = hasHitGround;
    }

    private void CheckCombo(bool isComboWindowOpened)
    {
        CanCombo = isComboWindowOpened;
    }

    private void CheckFalling()
    {
        RequestChangeState(PlayerState.Falling);
    }

    private void CheckIdle()
    {
        // PlayerMovement não interrompe ataque — só o PlayerCombat encerra via OnComboChange
        if (_isAttacking) return;
        RequestChangeState(PlayerState.Idle);
    }

    private void CheckAttackFinished()
    {
        RequestChangeState(PlayerState.Idle);
    }

    private void HandleStateChange(PlayerState state)
    {
        CurrentPlayerState = state;
    }


    // State machine
    private void RequestChangeState(PlayerState newState)
    {
        // Dead não tem saída (exceto Spawning por precaução)
        if (_currentPlayerState == PlayerState.Dead && newState != PlayerState.Spawning) return;

        // Sem mudança
        if (_currentPlayerState == newState) return;

        // Durante ataque: só Idle, Dead e ataques do combo podem interromper
        if (_isAttacking)
        {
            bool canInterrupt = newState is PlayerState.Idle
                                         or PlayerState.Dead
                                         or PlayerState.Attacking01
                                         or PlayerState.Attacking02
                                         or PlayerState.Attacking03;
            if (!canInterrupt) return;
        }

        // Pulo só no chão
        if (newState == PlayerState.Jumping && !IsGrounded) return;

        // Running só a partir de Walking
        if (newState == PlayerState.Running && _currentPlayerState != PlayerState.Walking) return;

        // Transiciona
        _currentPlayerState = newState;
        OnStateChanged?.Invoke(_currentPlayerState);
    }
}