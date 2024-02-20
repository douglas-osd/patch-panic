using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] AudioClip menuMusic, buttonAudio;

    private void Start()
    {
        highScoreText.text = $"High Score: {PlayerPrefs.GetInt("highScore")}";
        SoundManager.Instance.PlayMusic(menuMusic);
    }

    public void PlayGame()
    {
        SoundManager.Instance.PlaySound(buttonAudio);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        SoundManager.Instance.PlaySound(buttonAudio);
        Application.Quit();
    }

}
