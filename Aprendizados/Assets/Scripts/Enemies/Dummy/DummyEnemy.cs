using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class DummyEnemy : MonoBehaviour
    {
        private void OnEnable()
        {
            //PlayerCombat
            PlayerCombat.OnHit += HandleHit;
        }

        private void OnDisable()
        {
            //PlayerCombat
            PlayerCombat.OnHit -= HandleHit;
        }

        private void HandleHit(Collider2D[] hits)
        {
            foreach (var hit in hits)
                Debug.Log($"Atigido: {hit.name}");
        }
    }
}