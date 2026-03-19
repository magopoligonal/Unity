using UnityEngine;
using System;


public class PlayerCombat : MonoBehaviour
{
    [Header("Player Combat Settings")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRadius;
    [SerializeField] private LayerMask _enemyLayer;

    private bool _hitboxActive;
    
    //events
    public static event Action<Collider2D[]> OnHit;

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
