using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ThreeInOne
{
    public sealed class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button resumeButton;

        public bool IsPaused { get; private set; }

        private void Awake()
        {
            SetPaused(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (IsPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }

        public void PauseGame()
        {
            SetPaused(true);
            EventSystem.current?.SetSelectedGameObject(resumeButton.gameObject);
        }

        public void ResumeGame()
        {
            SetPaused(false);
            EventSystem.current?.SetSelectedGameObject(null);
        }

        public void RestartGame()
        {
            RestoreTimeAndAudio();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void BackToMainMenu()
        {
            RestoreTimeAndAudio();
            SceneManager.LoadScene(SceneNames.MainMenu);
        }

        private void OnDestroy()
        {
            RestoreTimeAndAudio();
        }

        private void SetPaused(bool paused)
        {
            IsPaused = paused;
            pausePanel.SetActive(paused);
            Time.timeScale = paused ? 0f : 1f;
            AudioListener.pause = paused;
        }

        private static void RestoreTimeAndAudio()
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }
}
