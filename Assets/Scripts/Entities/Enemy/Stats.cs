using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entities.Enemy
{
    public class Stats : MonoBehaviour
    {
        [SerializeField] private float health;
        [SerializeField] private GameObject deadSoldier;
        private Service _gameManagerData;

        private void OnEnable()
        {
            _gameManagerData = ServiceProvider.TryAddService("gameManagerData");

            ServiceProvider.ChangeAccess(_gameManagerData, AccessType.GetSet, GetType());

            var amountEnemies = ServiceProvider.Get(_gameManagerData, "amountEnemies", GetType());

            ServiceProvider.Put(_gameManagerData, "amountEnemies", GetType(),
                (amountEnemies != null ? (int)amountEnemies : 0) + 1);
        }

        public void ReceiveDamage(float amount)
        {
            if (health > 0)
            {
                health -= amount;
            }
            else
            {
                var amountEnemies = Kill();

                if (amountEnemies - 1 <= 0)
                {
                    NextLevel(); //TODO scene manager
                }
            }
        }
        private int Kill()
        {
            Instantiate(deadSoldier);
            deadSoldier.transform.position = transform.position;
            deadSoldier.SetActive(true);
            Destroy(gameObject);
            var amountEnemies =
                (int)ServiceProvider.Get(_gameManagerData, "amountEnemies", GetType())!; //cant be nullable here
                
            ServiceProvider.Put(_gameManagerData, "amountEnemies", GetType(), amountEnemies - 1);
            return amountEnemies;
        }

        private static void NextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}