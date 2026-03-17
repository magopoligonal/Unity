
using UnityEngine;
using System;
public class PlayerAnimation2 : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    //events 
    public static event Action OnAttackFinished;

    private void OnEnable()
    {
        //Player Controller
        PlayerController.OnStateChanged += HandleAnimation;
        
        //Player Movement
        PlayerMovement.HasSideChanged += FlipSprite;
    }

    private void OnDisable()
    {
        //Player Controller
        PlayerController.OnStateChanged -= HandleAnimation;
        
        //Player Movement 
        PlayerMovement.HasSideChanged -= FlipSprite;
    }
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void HandleAnimation(PlayerController.PlayerState state)
    {
        _animator.SetInteger(StringsAnimation.CurrentState, (int)state);
    }

    private void FlipSprite(bool isRightSide)
    {
        Vector3 scale = transform.localScale;
        if(isRightSide)
            scale.x = 1;
        else 
            scale.x = -1;
        transform.localScale = scale;
    }
    
    public void OnAttackAnimationEnd()
    {
        OnAttackFinished?.Invoke();
        Debug.Log("AttackFinished foi chamado!");
    }
}
