
using UnityEngine;
using System;
public class PlayerAnimator : MonoBehaviour
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

    /// <summary>
    /// Inverte o objeto de forma segura, dessa forma não apenas a Sprite é invertida(função principal desse método)
    /// como também o objeto por completo, assim colisores, marcadores de posição etc seguem a lógica visual 
    /// </summary>
    /// <param name="isRightSide"></param>
    private void FlipSprite(bool isRightSide)
    {
        Vector3 scale = transform.localScale;
        if(isRightSide)
            scale.x = 1;
        else 
            scale.x = -1;
        transform.localScale = scale;
    }
    
    /// <summary>
    /// Função utilizada para definir o fim da animação no animation event na aba animation.
    /// </summary>
    public void OnAttackAnimationEnd()
    {
        OnAttackFinished?.Invoke();
    }
}
