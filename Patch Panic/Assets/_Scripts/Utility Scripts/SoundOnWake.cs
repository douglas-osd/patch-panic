using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnWake : MonoBehaviour
{
    [SerializeField] AudioClip clip;

    private void OnEnable()
    {
        SoundManager.Instance.PlaySound(clip);
    }
}
