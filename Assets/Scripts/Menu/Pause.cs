using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class Pause : MonoBehaviour
    {
        [SerializeField] private InputActionReference pause;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private int sceneBuildIndex;
        private bool _isPaused;

        private void OnEnable()
        {
            pauseMenu.SetActive(false);
            pause.action.started += ActionPause;
        }

        private void ActionPause(InputAction.CallbackContext _) => TogglePause(true);

        private void TogglePause(bool showCursor)
        {
            _isPaused = !_isPaused;
            Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = showCursor;
            pauseMenu.SetActive(_isPaused);
            Time.timeScale = _isPaused ? 0f : 1f;
        }

        public void Continue() => TogglePause(false);

        public void Return()
        {
            TogglePause(true);
            SceneManager.LoadScene(sceneBuildIndex);
        }

        public void Quit() => Application.Quit();

        private void OnDisable()
        {
            pause.action.started -= ActionPause;
        }
    }
}