using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TMP_Text highScoreText;
    [SerializeField] AudioSource buttonAudio;

    private void Start()
    {
        highScoreText.text = $"High Score: {PlayerPrefs.GetInt("highScore")}";
    }

    public void PlayGame()
    {
        buttonAudio.Stop();
        buttonAudio.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        buttonAudio.Stop();
        buttonAudio.Play();
        Application.Quit();
    }

}
