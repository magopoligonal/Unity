using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    [Header("Movement")]
    //variaveis
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _runningSpeed = 1f; //esta como serializeField apenas para testar a lógica
    [SerializeField, Range(0.2f,1f)] private float _runningModifier = 0.2f;
    [SerializeField, Range(10,30)] private float _jumpingForce = 10f;
    [SerializeField] private float _horizontalMovement = 0f;
    [SerializeField] private float _verticalMovement = 0f;
    
    [Header("States")] 
    private bool IsFacingRight {get; set;}
    private bool _isGrounded;
    
    [Header("References")]
    //referencias
    [SerializeField] private Rigidbody2D _rb;

    //Events
    
//metodos
        //system

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        InputManager.OnWalking += Walk;
        InputManager.OnRunning += Run;
        InputManager.OnJump += Jump;
        
        //PlayerCollision
        PlayerCollision.OnReachingGround += GroundCheck;

    }

    void OnDisable()
    {
        InputManager.OnWalking -= Walk;
        InputManager.OnRunning -= Run;
        InputManager.OnJump -= Jump;
        
        //PlayerCollision
        PlayerCollision.OnReachingGround -= GroundCheck;
       
    }

    void FixedUpdate()
    {
        /*  obs 1: talvez verificar se o movimenta está estagnado durante alguns segundo e ativar o Idle caso o estado não seja nem Spawning nem Death ?
        ainda não sei como faria isso
            obs 2: em um ataque comum não se espera que o player ande, então talvez zerar o movimento quando currentPlayerState mudar
        */
        _verticalMovement = _rb.linearVelocity.y;
        if(_verticalMovement > _jumpingForce)
        _rb.linearVelocity = new Vector3(_horizontalMovement * _runningSpeed, _jumpingForce, 0f);
        else
        _rb.linearVelocity = new Vector3(_horizontalMovement * _runningSpeed, _rb.linearVelocity.y, 0f);
    
    }

    /*
    *   Aqui estou na dúvida se eu devo diferenciar o nome dos métodos que ativam durante o mesmo evento por exemplo:
    *   É correto por Walk() em todas classes que participam dessa ação?
    *   Ou seria melhor diferenciar com algum sufixo por exemplo: PM_Walk() aqui e no PlayerController PC_Walk()
    */
    private void Walk(InputAction.CallbackContext input)
    {
        bool lastSide;
        //verifica o valor do input se foi positivo ou negativo, caso seja 0 ele armazena o último valor e assim sabemos o lado que a sprite virou por ultimo
        if(input.ReadValue<Vector2>().x > 0)
            lastSide = true;
        else if (input.ReadValue<Vector2>().x < 0)
            lastSide = false;
        else
            lastSide = IsFacingRight;
        
        IsFacingRight = lastSide;
            
        //avisa para quem quiser ouvir que o lado mudou
        PlayerController.hasSideChanged?.Invoke(IsFacingRight);

        _horizontalMovement = input.ReadValue<Vector2>().x * _speed; //aparentemente não precisa do Time.deltaTime nem Time.fixedDeltaTime
         Debug.Log("Está andando!");
         if(input.canceled)
         {
             Debug.Log("Ele Parou");
         }
    }

    private void Run(InputAction.CallbackContext input)
    {
        if(input.performed)
        _runningSpeed = (1 + _runningModifier);  //é correto usar esse 1 + var ? a ideia veio de como fazemos conta de porcentagem.
        else if(input.canceled)
        _runningSpeed = 1f;
        Debug.Log("Está correndo!");
    }

    private void Jump(InputAction.CallbackContext input)
    {
        if (input.performed && _isGrounded)
        {
            _rb.AddForceY(_jumpingForce, ForceMode2D.Impulse);
            PlayerController.OnStateChanged?.Invoke(PlayerController.PlayerState.Jumping);
        }
        else { Debug.Log("Já está no ar, não trapaceie!"); }

    }

    private void GroundCheck(bool hasHitGround)
    {
        _isGrounded = hasHitGround;
    }
    
    
    

    
}
