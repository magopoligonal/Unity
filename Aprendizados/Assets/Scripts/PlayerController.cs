using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //variables - Properties
    public bool IsGrounded {get; private set;}
    //enums
    public enum PlayerState
    {
        Spawning, //fico imaginando que seria interessante ter isso para que eu possar por uma animação toda vez que o player nascer/renascer, mas não sei a melhor forma de implementar, talvez aqui mesmo nessa classe?
        Idle,
        Walking,
        Running,
        Attacking,
        Jumping,
        Dead
    }
    private PlayerState currentPlayerState = PlayerState.Spawning; //mesmo não sabendo como implementar o Spawning ainda, acho que faz sentido ser o valor default
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
    }

    void OnDisable()
    {
        InputManager.OnWalking -= Walk;
        InputManager.OnRunning -= Run;
        InputManager.OnJump -= Jump;
        InputManager.OnAttack -= Attack;
    }

    //playerController
    private void Attack(InputAction.CallbackContext input) //esse parametro está vindo lá do InputManager
    {
        //TO-DO porque ainda não sei
        //ativar animação de ataque
        //tocar som
        if(currentPlayerState == PlayerState.Jumping) //Coloquei apenas para testar a lógica
        return;

        currentPlayerState = PlayerState.Attacking;
        //imagino que não seja aqui que vejamos se o dano foi aplicado, ele apenas ataca né?
        //eu imagino uma interação de ataque onde a personagem esteja pulando como no Grand Chase, mas caso contrario eu poderia verificar se o currentPlayerState é Jumping para impedir né ? 
    }

    private void Run(InputAction.CallbackContext input)
    {
        currentPlayerState = PlayerState.Running;
    }

    private void Walk(InputAction.CallbackContext input)
    {
        currentPlayerState = PlayerState.Walking;
    }

    private void Jump(InputAction.CallbackContext input)
    {
        if(!IsGrounded)
        return;
        currentPlayerState = PlayerState.Jumping;
    }
    
    
}



/*
* Aqui será executado o controle da jogabilidade exercido pelas outras classes como PlayerMovement que controla o movimento do jogador
* a partir da escuta de eventos
*/