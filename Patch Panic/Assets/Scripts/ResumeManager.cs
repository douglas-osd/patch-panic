using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeManager : MonoBehaviour
{

    public AudioSource buttonAudio;
    
    public void OnResumeButtonPress()
    {
        buttonAudio.Stop();
        buttonAudio.Play();
        GameManager.Instance.UpdateGameState(GameState.Playing);
    }

}
