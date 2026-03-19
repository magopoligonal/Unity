using UnityEngine;

    public class Jacintus : BaseEnemy
    {
        protected override void HandleHit(Collider2D[] collider) //pensando agora e se eles tivessem armadura, daria para passar como parametro e no PlayerCombate fazer o dano da arma - armadura ou algo assim, né?
        {
            foreach (Collider2D col in collider)
            {
                Debug.Log(col.name);
            }
        }
        
        protected override void Die()
        {
            //TODO - Enviar um evento para sistemas reagirem? tipo animação de morte? e esses eventos eu faria na classe pai?
                Destroy(this.gameObject);
        }

        public override void TakeDamage(int amount) //então aqui em tese o player vai dar o dano amount mas o Jacintus tirara o valor da armadura?
        {
            base.TakeDamage(amount - Armor);
            if (Health <= 0)
            {
                Die();
            }
        }
    }
