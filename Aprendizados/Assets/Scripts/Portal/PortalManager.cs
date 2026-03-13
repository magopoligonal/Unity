using System;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    
    [SerializeField] private Transform[] _colliderPositions;
    [SerializeField] private Transform _ResetPosition;

    private void OnEnable()
    {
        PortalCollision.OnPortalExit += TeleportToPortal;
    }

    private void OnDisable()
    {
        PortalCollision.OnPortalExit -= TeleportToPortal;
    }

    private void TeleportToPortal(Transform player, Transform portal)
    {
        if (portal.transform.position == _colliderPositions[0].position) //Portal L_UP
        player.position = _colliderPositions[1].position;
        else if(portal.transform.position == _colliderPositions[1].position) //Portal R_UP
            player.position = _colliderPositions[0].position;
        else if(portal.transform.position == _colliderPositions[2].position)
        {
            player.position = _ResetPosition.position;
        }
        
    }
}
