using UnityEngine;
using System;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Player Combat Settings")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRadius;
    [SerializeField] private LayerMask _enemyLayer;
    
    private bool _hitboxActive;
    private bool _comboWindowOpen = false;
    //private int _currentCombo;
    private Coroutine _comboWindowRoutine; //armazena a referencia que o StartCoroutine vai gerar, dessa forma temos como parar manualmente o coroutine
    
    
    
    //events
    public static event Action<Collider2D[]> OnHit;
    public static event Action<bool> OnComboWindowOpen;
    
//methods
    //System
    private void OnEnable()
    {
        //PlayerController
        PlayerController.OnStateChanged += HandleStateChange;
    }

    private void OnDisable()
    {
        //PlayerController
        PlayerController.OnStateChanged -= HandleStateChange;
    }
     
    
    //serão chamados no animation event
    public void EnableHitbox()
    {
        _hitboxActive = true;
        CheckHit();
    }

    public void DisableHitbox()
    {
        _hitboxActive = false;
    }

    /// <summary>
    /// Coleta todos os colisores que estavam dentro da area de ataque durante o atk. (ele só ativa o evento para o primeiro? ou continua verificando até desabilitar a hitbox?)
    /// Se hit for > 0 quer dizer que o colisor do ataque tocou em algum inimigo (filtrado pela layer _enemyLayer).
    /// Invoca o evento que passa a informação de quem recebeu o hit.
    /// </summary>
    private void CheckHit()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRadius, _enemyLayer);
        if(hit.Length > 0)
            {
                OnHit?.Invoke(hit);
                Debug.Log("Hitamos algo!");
            }
    }

    //combo mechanics
    private void OpenComboWindow() //vai ser chamado no animation event
    {
        _comboWindowOpen = true;
        OnComboWindowOpen?.Invoke(_comboWindowOpen);
        _comboWindowRoutine = StartCoroutine(ComboTimeOut(0.2f));
        Debug.Log("Rotina Iniciada");
    }
    
    private IEnumerator ComboTimeOut(float frameAmount)
    {
        /*
        for (int i = 0; i < frameAmount; i++) //pulará o número determinado de frames dependendo da animação
        {
            yield return null; //aguarda exatamente um frame
        }*/
        yield return new WaitForSeconds(frameAmount);
        _comboWindowOpen = false;
        OnComboWindowOpen?.Invoke(_comboWindowOpen);
        Debug.Log("Combo Window Closed");
    }
    
    
    //Methods that reacts to external events
    /// <summary>
    /// Verifica se está no estado de ataque e caso não esteja em nenhum dos ataques possiveis
    /// Ele corta a hitbox do ativada pelo animation event, dessa forma temos uma forma segura de atacar.
    /// </summary>
    /// <param name="state"></param>
    private void HandleStateChange(PlayerController.PlayerState state)
    {
        //será que eu poderia ter um dictionary para armazenar o valor de isAttacking e o state atual? ou não faz sentido nenhum (não sei usar bem os dictionaries)
        //TODO - Adicionar o atk especial nessa verificação
        bool isAttacking = state == PlayerController.PlayerState.Attacking01 || state == PlayerController.PlayerState.Attacking02  || state == PlayerController.PlayerState.Attacking03;
        if (!isAttacking)
        {
            _hitboxActive = false;
            //cancela tambem a coroutine -- dúvida: precisa dos dois ou só stopcoroutine já basta?
            if (_comboWindowRoutine != null) //estava dando erro sem verificar então a partir de agora vou sempre verificar.
            {
                StopCoroutine(_comboWindowRoutine);
                _comboWindowRoutine = null; 
                _comboWindowOpen = false;
                OnComboWindowOpen?.Invoke(_comboWindowOpen);
            }
        }
    }
    
    
    
    
    
    
    
    //Editor
    private void OnDrawGizmos()
    {
        if(_hitboxActive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRadius);
        }
    }
}

/*
 * Obs: Percebi um pequeno bug que não altera em nada o funcionamento do jogo(ao menos nos meus teste), mas o Gizmo fica desenhado caso eu ataque e rapidamente pule, não dá dano, mas o circle fica lá até eu clicar no ataque novamente e somente assim sai no fim da animação.  
 */