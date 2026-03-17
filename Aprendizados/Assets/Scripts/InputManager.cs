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
        Debug.Log("AttackPressed chamado1111111111");
        if (input.performed)
        {
            Debug.Log("Attack performed");
            OnAttack?.Invoke(input);
        }
    }

    public void WalkPressed(InputAction.CallbackContext input)
    {
        
            OnWalking?.Invoke(input);
        
    }

    public void JumpPressed(InputAction.CallbackContext input)
    {
        if(input.performed) //eu devo fazer uma verificao no metodo de playercontroller para ver se o personagem está no chão?
        {
            OnJump?.Invoke(input);
        }
    }

    public void RunPressed(InputAction.CallbackContext input)
    {
        //retirado para ver se o valor do input passa corretamente. Não estou conseguindo pegar o input.performed e input.canceled em player movement
        OnRunning?.Invoke(input);
    }
}
