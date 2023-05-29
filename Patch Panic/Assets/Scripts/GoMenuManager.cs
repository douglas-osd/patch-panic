using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoMenuManager : MonoBehaviour
{

    public AudioSource buttonAudio;
    
    public void OnMainMenuButtonPress()
    {
        buttonAudio.Stop();
        buttonAudio.Play();
        SceneManager.LoadScene(0);
    }

}
