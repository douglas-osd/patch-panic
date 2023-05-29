using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitManager : MonoBehaviour
{

    public AudioSource buttonAudio;
    
    public void QuitButtonPress()
    {
        buttonAudio.Stop();
        buttonAudio.Play();
        GameManager.Instance.UpdateGameState(GameState.Quit);
    }

}
