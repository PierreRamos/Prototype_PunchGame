using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class System_AudioManager : MonoBehaviour
{
    System_EventHandler EventHandler;

    [SerializeField]
    public string nameToPlay { private get; set; }

    [SerializeField]
    private Object_Sound[] audioList;

    bool _randomizePitch;

    private void Awake()
    {
        foreach (Object_Sound sound in audioList)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.sound;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
        }
    }

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        //Player
        EventHandler.Event_PlayerHit += (dummy) =>
        {
            SetSoundNameToPlay("Player_Hit");
        };

        EventHandler.Event_HealPlayer += (dummy) =>
        {
            SetSoundNameToPlay("Player_Heal");
        };

        EventHandler.Event_TriggerStun += () =>
        {
            SetSoundNameToPlay("Player_Miss");
        };

        EventHandler.Event_EnemyHitAnimation += (dummy) =>
        {
            _randomizePitch = true;
            SetSoundNameToPlay("Enemy_Hit");
        };
    }

    public void SetSoundNameToPlay(string name)
    {
        PlaySound(name);
    }

    public void PlaySound(string name)
    {
        Object_Sound sound = Array.Find(audioList, test => test.soundName.Equals(name));

        if (sound != null)
        {
            var setPitch = sound.pitch;

            if (_randomizePitch)
            {
                sound.source.pitch = setPitch + UnityEngine.Random.Range(-0.2f, 0.2f);
            }

            sound.source.Play();
        }
        else
        {
            print("Cannot find sound");
        }

        _randomizePitch = false;
    }
}
