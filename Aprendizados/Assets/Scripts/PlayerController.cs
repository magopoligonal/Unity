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
        Falling,
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
        Player_Movement.OnFalling += HandleStateChange;
        Player_Movement.OnIdle += HandleStateChange;
    }

    void OnDisable()
    {
        InputManager.OnWalking -= Walk;
        InputManager.OnRunning -= Run;
        InputManager.OnJump -= Jump;
        InputManager.OnAttack -= Attack;
        
        //PlayerController
        OnStateChanged -= HandleStateChange;
        
        //PlayerMovement
        Player_Movement.OnFalling -= HandleStateChange;
        Player_Movement.OnIdle -= HandleStateChange;
        
        
        //PlayerCollision
        PlayerCollision.OnReachingGround -= CheckGround;
    }

    //playerController
    private void Attack(InputAction.CallbackContext input) //esse parametro está vindo lá do InputManager
    {
        //TO-DO porquê ainda não sei
        //ativar animação de ataque
        //tocar som
        OnStateChanged?.Invoke(PlayerState.Attacking);
        //imagino que não seja aqui que vejamos se o dano foi aplicado, ele apenas ataca né?
        //eu imagino uma interação de ataque onde a personagem esteja pulando como no Grand Chase, mas caso contrario eu poderia verificar se o currentPlayerState é Jumping para impedir né ? 
    }

    private void Run(InputAction.CallbackContext input)
    {
        OnStateChanged?.Invoke(PlayerState.Running); 
        /*revisando o código e testando in-game percebi que essa linha talvez entre no mesmo caso de não ser responsabilidade dela resolver o estado.
        * No momento ele não mantem running caso eu caia em um plano, mas caso seja em uma rampa, ele mantem o running e não troca pra Idle no Player_Movement. Por enquanto vou manter pq imagino que seja engraçado com a animacao de corrida.
         */
    }

    private void Walk(InputAction.CallbackContext input)
    {
        //se não está andando está em Idle
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
            case PlayerState.Running:
                CurrentPlayerState = PlayerState.Running;
                break;
            case PlayerState.Jumping:
                CurrentPlayerState = PlayerState.Jumping;
                break;
            case PlayerState.Falling:
                CurrentPlayerState = PlayerState.Falling;
                break;
            case PlayerState.Attacking:
                CurrentPlayerState = PlayerState.Attacking;
                break;
            case PlayerState.Dead:
                CurrentPlayerState = PlayerState.Dead;
                break;
            default:
                CurrentPlayerState = PlayerState.Spawning;
                break;
                
        }
    }

}



/*
* Aqui será executado o controle da jogabilidade exercido pelas outras classes como PlayerMovement que controla o movimento do jogador
* a partir da escuta de eventos
*/