using UnityEngine;
using UnityEngine.Serialization;

namespace Weapon
{
    public class Weapon : MonoBehaviour
    {
        [FormerlySerializedAs("prefabricatedBullet")] [SerializeField]
        private Bullet prefabBullet;

        [SerializeField] private Transform tip;
        [SerializeField] private float power;
        [SerializeField] private float damage;
        [SerializeField] private bool isHitScan;

        public void Fire()
        {
            if(!isHitScan)
            {
                var newBullet = Instantiate(prefabBullet, tip.position, tip.rotation);
                newBullet.gameObject.SetActive(true);
                newBullet.force = power;
                newBullet.Fire();
            }
            else
            {
                Debug.DrawRay(tip.position, tip.forward * power, Color.red, 3);

                if (Physics.Raycast(tip.position, tip.forward, out var hit))
                {
                    var objective = hit.transform.gameObject.GetComponent<Enemy.Stats>();
                    if (objective && objective.CompareTag("Enemy"))
                    {
                        objective.ReceiveDamage(damage);
                    }
                }
                
            }
        }
    }
}