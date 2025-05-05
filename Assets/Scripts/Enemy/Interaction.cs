using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(Stats))]
    public class Interaction : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                gameObject.GetComponent<Stats>().ReceiveDamage(other.gameObject.GetComponent<Weapon.Bullet>().damage);
            }
        }
    }
}
