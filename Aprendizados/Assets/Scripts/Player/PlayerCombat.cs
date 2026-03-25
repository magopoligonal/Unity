using UnityEngine;
using System;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Player Combat Settings")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRadius;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField, Range(20, 60)] private int[] comboWindowFrameAmount = { 20, 20, 20 };
    [SerializeField] private int[] _weaponDamage = { 1, 2, 2 };
    
    private bool _hitboxActive;
    private bool _comboWindowOpen = false;
    private int _currentCombo;
    private Coroutine _comboWindowRoutine; //armazena a referencia que o StartCoroutine vai gerar, dessa forma temos como parar manualmente o coroutine
    
    
    
    //events
    public static event Action<Collider2D[], int> OnHit;
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
        
            // Forçamos o reset de qualquer estado anterior de hit
            _hitboxActive = false; 
    
            // Pequeno delay ou verificação: só ativa se o estado atual do Animator 
            // for realmente um estado de ataque (evita disparos em transições fantasmas)
            Debug.Log($"EnableHitbox disparado. Combo Atual: {_currentCombo}");
    
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
        int currentDamage = _weaponDamage[Mathf.Clamp(_currentCombo -1, 0 , _weaponDamage.Length - 1)];
        
        Collider2D[] hit = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRadius, _enemyLayer);
        if(hit.Length > 0)
            {
                OnHit?.Invoke(hit, currentDamage);
                Debug.Log($"OnHit invocado no frame: {Time.frameCount}");

            }
    }

    //combo mechanics
    private void OpenComboWindow() //vai ser chamado no animation event
    {
        //evento para o playerMovement
        _comboWindowOpen = true;
        OnComboWindowOpen?.Invoke(_comboWindowOpen);
        
        //Para a Coroutine anterior caso haja 
        if (_comboWindowRoutine != null)
        {
            StopCoroutine(_comboWindowRoutine);
            _comboWindowRoutine = null;
            Debug.Log($"Coroutine pausada: {Time.frameCount}  | Combo atual: {_currentCombo}  | Hitbox: {_hitboxActive}");
        }
        
        //Inicia uma nova corotina com o tempo customizavel
        int index = Mathf.Clamp(_currentCombo - 1, 0, comboWindowFrameAmount.Length - 1); //forma de facilitar para por os timers direto do inspector sem estourar o array
        _comboWindowRoutine = StartCoroutine(ComboTimeOut(comboWindowFrameAmount[index]));
        Debug.Log($"Coroutine iniciada: {Time.frameCount}  | Combo atual: {_currentCombo}  | Hitbox: {_hitboxActive}");
        
        
    }
    
    private IEnumerator ComboTimeOut(float windowOpenTimerInFrames)
    {
        //essencial
        int comboAtStart = _currentCombo;
        for(int i = 0; i < windowOpenTimerInFrames; i++)
            yield return null;
        //yield return new WaitForSeconds(windowOpenTimerInFrames);
        
        if(_currentCombo == comboAtStart)
            PlayerController.ChangeState(PlayerController.PlayerState.Idle); // <-- dessa forma não precisamos do OnAttackAnimationEnd para checar se acabou a animação
        
        //evento para o playerMovement
        _comboWindowOpen = false;
        OnComboWindowOpen?.Invoke(_comboWindowOpen);
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
        Debug.Log($"TryCombo chamado, _currentCombo: {_currentCombo}, _comboWindowOpen: {_comboWindowOpen}");
        if (_currentCombo == 3) return; //melhorar isso depois
        _hitboxActive = false;
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