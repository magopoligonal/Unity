using UnityEngine;
using System;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer; //vai ser como uma flag para sabermos se estamos ou não no chão
    [SerializeField] private Transform[] _positionCollider; //aqui são os pontos dos raios
    [SerializeField] private float _rayDistance; //e aqui a distância que o raio pode chegar (só para ficar mais fácil de customizar no inspetor)
    [SerializeField] private PlayerController _playerController;
    private bool _wasGrounded;
   
   //events
   public static event Action<bool> OnReachingGround; //avisa para quem estiver ouvindo que o player tocou o chão


    void FixedUpdate()
    {
        bool isGrounded = Physics2D.OverlapCircle(_positionCollider[0].position, _rayDistance, _groundLayer);
        if (isGrounded != _wasGrounded)
        {
            OnReachingGround?.Invoke(isGrounded);
            _wasGrounded = isGrounded;
        }
        
    }

    
    void OnDrawGizmos() //funcao que serve para desenhar (não necessariamente está ligada com o OverlapCircle ou algo do tipo)
    {
        Gizmos.color = Color.aliceBlue;
        Gizmos.DrawWireSphere(_positionCollider[1].position, _rayDistance);
    }
    
    

}

/*
*   Essa classe ficará responsavel por controlar as colisões do player
    Seria interessante separar a colisão das armas? lembrando que a intenção é ter uma melee e uma ranged(algo como as laminas do caos do Kratos)
*/
