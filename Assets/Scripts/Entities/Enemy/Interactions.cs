using Services;
using UnityEngine;

namespace Entities.Enemy
{
    public class Interactions : MonoBehaviour
    {
        private Service _playerData;
        private void OnEnable()
        {
            _playerData = ServiceProvider.GetService("playerData");
        }
    }
}
