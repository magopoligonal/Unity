using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;


public class PlayerController : MonoBehaviour
{
    //enums
    public enum PlayerState
    {
        //0-1 Instantaneos
        Spawning = 0, 
        Dead = 1,
        //2-6 Movimentacao
        Idle = 2,
        Walking = 3,
        Running = 4,
        Jumping = 5,
        Falling = 6,
        //7-x Combate
        Attacking01 = 7,
        Attacking02 = 8,
        Attacking03 = 9,
    }
//variables 
    //fields
    [SerializeField] private PlayerState _currentPlayerState = PlayerState.Idle;
    
    //Properties
    public bool IsGrounded {get; private set;}
    public PlayerState CurrentPlayerState
    {
        get{ return _currentPlayerState; }
        set
        {
            _currentPlayerState = value;
        }
    }

    //Events
    public static event  Action<PlayerState> OnStateChanged;
    public static event Action OnAttackPressed;
    
    

//methods
    //system
    void OnEnable()
    {
        //InputManager
        InputManager.OnWalking += Walk;
        InputManager.OnRunning += Run;
        InputManager.OnJump += Jump;
        InputManager.OnAttack += Attack;
        
        //PlayerController
        OnStateChanged += HandleStateChange;
        
        //PlayerCollision                  
        PlayerCollision.OnReachingGround += CheckGround;

        //PlayerMovement
        PlayerMovement.OnFalling += CheckFalling;
        PlayerMovement.OnIdle += CheckIdle;
        
        //Player Animator
        PlayerAnimator.OnAttackFinished += CheckAttackFinished;
        
        //Player Combat
        
    }

    void OnDisable()
    {
        //InputManager
        InputManager.OnWalking -= Walk;
        InputManager.OnRunning -= Run;
        InputManager.OnJump -= Jump;
        InputManager.OnAttack -= Attack;
        
        //PlayerController
        OnStateChanged -= HandleStateChange;
        
        //PlayerMovement
        PlayerMovement.OnFalling -= CheckFalling;
        PlayerMovement.OnIdle -= CheckIdle;
        
        
        //PlayerCollision
        PlayerCollision.OnReachingGround -= CheckGround;
        
        //PlayerAnimator
        PlayerAnimator.OnAttackFinished -= CheckAttackFinished;
        
        //PlayerCombat
        
    }

    //playerController
    private void Attack(InputAction.CallbackContext input) //esse parametro está vindo lá do InputManager
    {
        if (input.performed)
        {
            OnAttackPressed?.Invoke();
            Debug.Log($"PLAYER CONTROLLER: Ataque pressionado");
        }
        
    }

    private void Run(InputAction.CallbackContext input)
    {
        if(input.performed)
            OnStateChanged?.Invoke(PlayerState.Running); 
    }

    private void Walk(InputAction.CallbackContext input)
    {
        if(input.performed)
            OnStateChanged?.Invoke(PlayerState.Walking);
    }

    private void Jump(InputAction.CallbackContext input)
    {
        if(!IsGrounded) return;
        
        IsGrounded = false; 
        OnStateChanged?.Invoke(PlayerState.Jumping);
        
    }

    private void CheckGround(bool hasHitGround)
    {
        IsGrounded = hasHitGround;
    }

    private void CheckFalling()
    {
        OnStateChanged?.Invoke(PlayerState.Falling);
    }

    private void CheckIdle()
    {
        OnStateChanged?.Invoke(PlayerState.Idle);
    }

    private void CheckAttackFinished()
    {
        OnStateChanged?.Invoke(PlayerState.Idle);
    }

    public static void ChangeState(PlayerState newState)
    {
        OnStateChanged?.Invoke(newState);
    }
    
    

    private void HandleStateChange(PlayerState state)
    {
        CurrentPlayerState = state;
    }

}



/*
* Aqui será executado o controle da jogabilidade exercido pelas outras classes como PlayerMovement que controla o movimento do jogador
* a partir da escuta de eventos
*/