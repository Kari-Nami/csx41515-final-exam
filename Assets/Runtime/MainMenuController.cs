using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ThreeInOne
{
    public sealed class MainMenuController : MonoBehaviour
    {
        private void Awake()
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }

        public void PlayDrivingGame()
        {
            LoadGame(SceneNames.DrivingGame);
        }

        public void PlayFlyingGame()
        {
            LoadGame(SceneNames.FlyingGame);
        }

        public void PlaySumoGame()
        {
            LoadGame(SceneNames.SumoGame);
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        private static void LoadGame(string sceneName)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            SceneManager.LoadScene(sceneName);
        }
    }
}
