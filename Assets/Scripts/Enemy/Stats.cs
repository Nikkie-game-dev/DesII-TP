using UnityEngine;

namespace Enemy
{
    public class Stats : MonoBehaviour
    {
        [SerializeField] private float health;

        public void ReceiveDamage(float amount)
        {
            health -= amount;
        }
    }
}
