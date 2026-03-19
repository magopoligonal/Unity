using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount); //não pode ser private já que via ser acessado por outras classes, default é public
    
}
