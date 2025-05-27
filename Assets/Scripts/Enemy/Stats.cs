using UnityEngine;

namespace Enemy
{
    public class Stats : MonoBehaviour
    {
        [SerializeField] private float health;

        public void ReceiveDamage(float amount)
        {
            if (health > 0)
            {
                health -= amount;
            }
            else
            {
                Destroy(gameObject);
                //TODO: ragdoll
            }
        }
    }
}
