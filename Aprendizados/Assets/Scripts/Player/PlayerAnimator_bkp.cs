using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator_bkp : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    //Events

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        //Animator
        InputManager.OnWalking += Walk;
        InputManager.OnRunning += Run;
        InputManager.OnJump += Jump;
        InputManager.OnAttack += Attack;
        //falta pensar e criar um classe que disparara OnDeath e OnSpawning
        //tambem falta pensar uma forma de voltar para o Idle ou ele volta sozinho por causa do animator?

        //SpriteRenderer
        
        PlayerMovement.HasSideChanged += FlipSprite;
        
        

    }
    void OnDisable()
    {
        //Animator
        InputManager.OnWalking -= Walk;
        InputManager.OnRunning -= Run;
        InputManager.OnJump -= Jump;
        InputManager.OnAttack -= Attack;
        //SpriteRenderer
        
        PlayerMovement.HasSideChanged -= FlipSprite;
    }

    /*  Ok nesse estagio de programar os metodos eu me peguei pensando: ter o controle de .started .performed e .canceled pode ser util para as animacoes?
     *  Ou isso só vai atrapalhar?
     */
    private void Walk(InputAction.CallbackContext input)
    {
        //TODO   
        if(input.started)
        {
            _animator.SetBool(StringsAnimation.isWalking, true);
            //aqui eu teria que avisar o Player Controller de alguma forma?
        }else if (input.canceled)
        {
            _animator.SetBool(StringsAnimation.isWalking, false);
        }

        //fiquei pensando se daria para fazer um switch com todos os estados que checaria o PlayerState de forma a reduzir o numero de metodos
    }

    private void Run(InputAction.CallbackContext input)
    {
        //Não tem sprites para a corrida.
    }
    private void Jump(InputAction.CallbackContext input)
    {
        //Não tem sprites para o pulo, então vou tentar fazer depois.
    }
    private void Attack(InputAction.CallbackContext input)
    {
        if(input.performed)
            _animator.SetTrigger(StringsAnimation.Attack);
    }


    private void FlipSprite(bool isRightSide) 
    {
        Vector3 scale = transform.localScale;
        if(isRightSide)
            scale.x = 1f;
        else
            scale.x = -1f;
        transform.localScale = scale;
    }

    

}

/*
* Classe responsavel por ouvir eventos e desencadear animações
*/