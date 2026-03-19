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
