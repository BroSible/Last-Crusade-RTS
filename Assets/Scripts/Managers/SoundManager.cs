using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    private AudioSource AttackChannel;
    public AudioClip AttackClip; 


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }

        AttackChannel = gameObject.AddComponent<AudioSource>();
        AttackChannel.volume = 1f;
        AttackChannel.playOnAwake = false;
    }

    public void PlayAttackSound()
    {
        if(AttackChannel.isPlaying == false)
        {
            AttackChannel.PlayOneShot(AttackClip);
        }
    }
}
