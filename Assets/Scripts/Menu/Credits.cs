using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class Credits : MonoBehaviour
    {
        public void Return() => SceneManager.LoadScene(0);

        public void ProjectBifron() => Application.OpenURL("https://projectbifron.itch.io");

        public void Wolinski() => Application.OpenURL("https://www.fab.com/sellers/Mateusz%20Wolinski");

        public void Stanislau() => Application.OpenURL("https://www.fab.com/sellers/Stanislau");

        public void Quantum() => Application.OpenURL("https://www.fab.com/sellers/Quantum%20Assets");
        
        public void Glyth() => Application.OpenURL("https://assetstore.unity.com/publishers/17929");
    }
}