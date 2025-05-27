using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private int playIndex;
        [SerializeField] private int creditsIndex;

        public void Play() => SceneManager.LoadScene(playIndex);
        public void Credits() => SceneManager.LoadScene(creditsIndex);
        public void Exit() => Application.Quit();
    }
}
