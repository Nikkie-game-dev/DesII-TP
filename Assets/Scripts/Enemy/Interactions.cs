using Services;
using UnityEngine;

namespace Enemy
{
    public class Interactions : MonoBehaviour
    {
        private Service _playerData;
        private void OnEnable()
        {
            _playerData = ServiceProvider.GetService("playerData");
        }

        private void FixedUpdate()
        {
            
        }
    }
}
