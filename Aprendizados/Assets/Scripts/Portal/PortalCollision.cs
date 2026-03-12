using System;
using UnityEngine;

public class PortalCollision : MonoBehaviour
{
    //TODO - Verificar se a colisão ocorre APENAS com o player
    //TODO - Pegar um array de collision? Ou usar dictionary?
    public static event Action<Transform, Transform> OnPortalExit;
    
    private void OnTriggerExit2D(Collider2D other)
    {
        OnPortalExit?.Invoke(other.transform, this.transform);
    }
}

/*
 * Essa classe vai ser responsavel por avisar quando o #player interagiu com um portal. A classe PortalController responde
 */