using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    //enums
    public enum PlayerState
    {
        Spawning, 
        Idle,
        Walking,
        Running,
        Attacking,
        Jumping,
        Dead
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
    public static  Action<PlayerState> OnStateChanged;
    public static  Action<bool> hasSideChanged;
    

//methods
    //system
    void OnEnable()
    {
        //placeholder para OnIdle --> imagino que esse evento seja disparado pelo Player_Movement quando a velocidade for 0f ou algo assim
        //placeholder para OnDeath
        InputManager.OnWalking += Walk;
        InputManager.OnRunning += Run;
        InputManager.OnJump += Jump;
        InputManager.OnAttack += Attack;
        
        //PlayerController
        OnStateChanged += HandleStateChange;
        
        //PlayerCollision                  
        PlayerCollision.OnReachingGround += CheckGround;

        //PlayerMovement
        
        
    }

    void OnDisable()
    {
        InputManager.OnWalking -= Walk;
        InputManager.OnRunning -= Run;
        InputManager.OnJump -= Jump;
        InputManager.OnAttack -= Attack;
        
        //PlayerController
        OnStateChanged -= OnStateChanged;
        
        //PlayerMovement
        
        
        //PlayerCollision
        PlayerCollision.OnReachingGround -= CheckGround;
    }

    //playerController
    private void Attack(InputAction.CallbackContext input) //esse parametro está vindo lá do InputManager
    {
        //TO-DO porquê ainda não sei
        //ativar animação de ataque
        //tocar som
        if(CurrentPlayerState == PlayerState.Jumping) //Coloquei apenas para testar a lógica
        return;

        OnStateChanged?.Invoke(PlayerState.Attacking);
        //imagino que não seja aqui que vejamos se o dano foi aplicado, ele apenas ataca né?
        //eu imagino uma interação de ataque onde a personagem esteja pulando como no Grand Chase, mas caso contrario eu poderia verificar se o currentPlayerState é Jumping para impedir né ? 
    }

    private void Run(InputAction.CallbackContext input)
    {
        OnStateChanged?.Invoke(PlayerState.Running);
    }

    private void Walk(InputAction.CallbackContext input)
    {
        //se não está andando está em Idle
        if(input.performed)
            OnStateChanged?.Invoke(PlayerState.Walking);
        else if(input.canceled) 
            OnStateChanged?.Invoke(PlayerState.Idle);
    }

    private void Jump(InputAction.CallbackContext input)
    {
        if(!IsGrounded)
        return;
        OnStateChanged?.Invoke(PlayerState.Jumping);
        IsGrounded = false; 
        
    }

    private void CheckGround(bool hasHitGround) //achei estranho ter que criar uma função só para isso, está correto?
    {
        IsGrounded = hasHitGround;
        OnStateChanged?.Invoke(IsGrounded ? PlayerState.Idle : PlayerState.Jumping); //esta me causando estranheza não ter um estado para "caindo", não sei se isso faz sentido.
    }
        

    private void HandleStateChange(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle: 
                CurrentPlayerState = PlayerState.Idle;
                break;
            case PlayerState.Walking:
                CurrentPlayerState = PlayerState.Walking;
                break;
            case PlayerState.Attacking:
                CurrentPlayerState = PlayerState.Attacking;
                break;
            case PlayerState.Jumping:
                CurrentPlayerState = PlayerState.Jumping;
                break;
            case PlayerState.Dead:
                CurrentPlayerState = PlayerState.Dead;
                break;
            default:
                CurrentPlayerState = PlayerState.Spawning;
                break;
                
        }
    }


    private void OnBecameInvisible()
    {
        transform.position = new Vector3(-6.1f, 2.5f, 0f);
    }
}



/*
* Aqui será executado o controle da jogabilidade exercido pelas outras classes como PlayerMovement que controla o movimento do jogador
* a partir da escuta de eventos
*/