using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class Pause : MonoBehaviour
    {
        [SerializeField] private InputActionReference pause;
        [SerializeField] private GameObject pauseMenu;
        private bool _isPaused;

        private void OnEnable()
        {
            pauseMenu.SetActive(false);
            pause.action.started += _ => TogglePause();
        }

        private void TogglePause()
        {
            _isPaused = !_isPaused;
            Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _isPaused;
            pauseMenu.SetActive(_isPaused);
            Time.timeScale = _isPaused ? 0f : 1f;
        }

        public void Continue() => TogglePause();
        public void Return() {
            TogglePause();
            SceneManager.LoadScene(0);
        }
        public void Quit() => Application.Quit();
    }
}