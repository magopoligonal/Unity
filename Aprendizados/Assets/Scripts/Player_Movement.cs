using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    //variaveis
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _runningSpeed = 1f; //esta como serializeField apenas para testar a lógica
    [SerializeField, Range(0.2f,1f)] private float _runningModifier = 0.2f;
    [SerializeField] private float _horizontalMovement = 0f;
    public bool IsFacingRight {get; private set;}
    //referencias
    [SerializeField] private GameObject _player;
//metodos
        //system

    void Awake()
    {
        if(_player == null)
        _player = GameObject.FindWithTag("Player");
    }

    void OnEnable()
    {
        InputManager.OnWalking += Walk;
        InputManager.OnRunning += Run;
    }

    void OnDisable()
    {
        InputManager.OnWalking -= Walk;
        InputManager.OnRunning -= Run;
    }

    void FixedUpdate()
    {
        /*  obs 1: talvez verificar se o movimenta está estagnado durante alguns segundo e ativar o Idle caso o estado não seja nem Spawning nem Death ?
        ainda não sei como faria isso
            obs 2: em um ataque comum não se espera que o player ande, então talvez zerar o movimento quando currentPlayerState mudar
        */

    }

    /*
    *   Aqui estou na dúvida se eu devo diferenciar o nome dos métodos que ativam durante o mesmo evento por exemplo:
    *   É correto por Walk() em todas classes que participam dessa ação?
    *   Ou seria melhor diferenciar com algum sufixo por exemplo: PM_Walk() aqui e no PlayerController PC_Walk()
    */
    private void Walk(InputAction.CallbackContext input)
    {
        _horizontalMovement = input.ReadValue<Vector2>().x * _speed * Time.deltaTime;
        _horizontalMovement *= IsFacingRight ? 1f : -1f; //verifica se está virado para direita caso esteja o X(movimento horizontal) multiplica por 1 para ir para direita
        _horizontalMovement *= _runningSpeed;
         transform.position += new Vector3(_horizontalMovement, 0f, 0f);
    }

    private void Run(InputAction.CallbackContext input)
    {
        if(input.performed)
        _runningSpeed += _runningModifier; 
        else if(input.canceled)
        _runningSpeed = 1f;
    }
}
