using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    //Events
    

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
        
        PlayerController.hasSideChanged += FlipSprite;

    }
    void OnDisable()
    {
        //Animator
        InputManager.OnWalking -= Walk;
        InputManager.OnRunning -= Run;
        InputManager.OnJump -= Jump;
        InputManager.OnAttack -= Attack;
        //SpriteRenderer
        
        PlayerController.hasSideChanged -= FlipSprite;
    }

    /*  Ok nesse estagio de programar os metodos eu me peguei pensando: ter o controle de .started .performed e .canceled pode ser util para as animacoes?
     *  Ou isso só vai atrapalhar?
     */
    private void Walk(InputAction.CallbackContext input)
    {
        //TODO   
        if(input.started)
        {
            /*
            * Eu já utilizei uma vez uma abordagem de ter uma classe só para guardar o nome das strings para evitar erro de digitacao
            * será que pode ser o caso aqui ? E isso funcionaria com Scriptable Object? qual é o mais recomendado se isso for uma abordagem válida e por que?
            */
            _animator.SetBool("isWalking", true);
            //aqui eu teria que avisar o Player Controller de alguma forma?
        }else if (input.canceled)
        {
            _animator.SetBool("isWalking", false);
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
        _animator.SetTrigger("Attack");
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

/**
* Classe responsavel por ouvir eventos e desencadear animações
*/