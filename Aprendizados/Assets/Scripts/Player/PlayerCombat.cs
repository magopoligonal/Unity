using UnityEngine;
using System;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Player Combat Settings")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRadius;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField, Range(0.2f, 2f)] private float[] comboWindowDurations = { 0.2f, 0.2f, 1.2f};
    private bool _hitboxActive;
    private bool _comboWindowOpen = false;
    private int _currentCombo;
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
        PlayerController.OnAttackPressed += TryCombo;
    }

    private void OnDisable()
    {
        //PlayerController
        PlayerController.OnStateChanged -= HandleStateChange;
        PlayerController.OnAttackPressed -= TryCombo;
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
        
        int index = Mathf.Clamp(_currentCombo - 1, 0, comboWindowDurations.Length - 1);
        _comboWindowRoutine = StartCoroutine(ComboTimeOut(comboWindowDurations[index]));
    }
    
    private IEnumerator ComboTimeOut(float windowOpenTimerInSeconds)
    {
        int comboAtStart = _currentCombo;
        //if (comboAtStart == 3)
          //  windowOpenTimerInSeconds += 1f;
        yield return new WaitForSeconds(windowOpenTimerInSeconds);
        _comboWindowOpen = false;
        OnComboWindowOpen?.Invoke(_comboWindowOpen);
        if(_currentCombo == comboAtStart)
            PlayerController.ChangeState(PlayerController.PlayerState.Idle); // <-- dessa forma não precisamos do OnAttackAnimationEnd para checar se acabou a animação
    }
    
    
    //Methods that reacts to external events
    /// <summary>
    /// Verifica se está no estado de ataque e caso não esteja em nenhum dos ataques possiveis
    /// Ele corta a hitbox do ativada pelo animation event, dessa forma temos uma forma segura de atacar.
    /// </summary>
    /// <param name="state"></param>
    private void HandleStateChange(PlayerController.PlayerState state)
    {
        //TODO - Adicionar o atk especial nessa verificação
        bool isAttacking = state == PlayerController.PlayerState.Attacking01 || state == PlayerController.PlayerState.Attacking02  || state == PlayerController.PlayerState.Attacking03;
        if (!isAttacking) //se não está atacando, reseta todas as propriedades do ataque.
        {
            _hitboxActive = false;
            _currentCombo = 0;
            
            if (_comboWindowRoutine != null) //estava dando erro sem verificar então a partir de agora vou sempre verificar.
            {
                StopCoroutine(_comboWindowRoutine);
                _comboWindowRoutine = null; 
                _comboWindowOpen = false;
                OnComboWindowOpen?.Invoke(_comboWindowOpen);
            }
        }
    }

    private void TryCombo()
    {
        switch (_currentCombo)
        {
            case 0:
                _currentCombo = 1;
                _comboWindowRoutine = null; //só por segurança, garante que não tem uma coroutine rodando nessa ref.
                PlayerController.ChangeState(PlayerController.PlayerState.Attacking01);
                break;
            case 1:
                if (!_comboWindowOpen) return;
                _currentCombo = 2;
                PlayerController.ChangeState(PlayerController.PlayerState.Attacking02);
                break;
            case 2:
                if(!_comboWindowOpen) return;
                _currentCombo = 3;
                PlayerController.ChangeState(PlayerController.PlayerState.Attacking03);
                break;
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