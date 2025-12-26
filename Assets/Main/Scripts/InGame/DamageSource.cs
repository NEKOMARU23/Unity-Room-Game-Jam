using UnityEngine;

namespace Main.Damage
{
    public class DamageSource : MonoBehaviour
    {
        [SerializeField] private int damageAmount = 1;
        public int DamageAmount => damageAmount;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!this.enabled) return;
        }
    }
}