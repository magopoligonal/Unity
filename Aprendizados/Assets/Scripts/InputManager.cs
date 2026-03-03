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
        if (input.performed)
        {
            //TODO
            OnAttack?.Invoke(input);
        }
    }

    public void WalkPressed(InputAction.CallbackContext input)
    {
        if(input.performed)
        {
            OnWalking?.Invoke(input);
        }
    }

    public void JumpPressed(InputAction.CallbackContext input)
    {
        if(input.performed) //eu devo fazer uma verificao no metodo de playercontroller para ver se o personagem está no chão?
        {
            OnJump?.Invoke(input);
        }
    }
}
