using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManager : MonoBehaviour
{
    //events
    //Action<InputAction.CallbackContext> significa que nosso método pode ter parametro, os de ação precisam desse 
    //para que o InputAction funcione corretamente
    //public static event Action OnIdle; 
    public static event Action<InputAction.CallbackContext> OnWalking;
    public static event Action<InputAction.CallbackContext> OnRunning;
    public static event Action<InputAction.CallbackContext> OnJump;
    public static event Action<InputAction.CallbackContext> OnAttack;
    //public static event Action OnDeath;
    
    

    public void AttackPressed(InputAction.CallbackContext input)
    {
        
       
            
            OnAttack?.Invoke(input);
        
    }

    public void WalkPressed(InputAction.CallbackContext input)
    {
        
            OnWalking?.Invoke(input);
        
    }

    public void JumpPressed(InputAction.CallbackContext input)
    {
        
            OnJump?.Invoke(input);
        
    }

    public void RunPressed(InputAction.CallbackContext input)
    {
        OnRunning?.Invoke(input);
    }
}
