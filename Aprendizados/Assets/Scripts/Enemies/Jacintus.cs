using UnityEngine;

    public class Jacintus : BaseEnemy
    {
        protected override void HandleHit(Collider2D[] collider) //pensando agora e se eles tivessem armadura, daria para passar como parametro e no PlayerCombate fazer o dano da arma - armadura ou algo assim, né?
        {
            foreach( Collider2D col in collider)
                if (col.gameObject == this.gameObject)
                {
                        IDamageable damageable = col.GetComponent<IDamageable>();
                        if(damageable != null)
                            TakeDamage(1); //será que fazer um evento para capturar o dano da arma atual (já que vamos ter o teclado e o mouse) seria interessante? ou qual outra forma de fazer?
                        Debug.Log($"{gameObject.name} tem: {Health} de vida");
                }
            
        }
        
        protected override void Die()
        {
            //TODO - Enviar um evento para sistemas reagirem? tipo animação de morte? e esses eventos eu faria na classe pai?
                Destroy(this.gameObject);
        }

        public override void TakeDamage(int amount) //então aqui em tese o player vai dar o dano amount mas o Jacintus tirara o valor da armadura?
        {
            int finalDamage = amount - Armor;
            Debug.Log($"{this.gameObject.name} tomou: {finalDamage} de dano.");
            base.TakeDamage(finalDamage);
            if (Health <= 0)
            {
                Die();
            }
        }
    }
