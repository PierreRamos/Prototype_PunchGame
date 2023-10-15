using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class System_AudioManager : MonoBehaviour
{
    System_EventHandler EventHandler;

    [SerializeField]
    public string nameToPlay { private get; set; }

    [SerializeField]
    private Object_Sound[] audioList;

    bool _randomizePitch;
    bool _startedSoloBattle;

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

        //UI
        EventHandler.Event_CorrectInput += () =>
        {
            SetSoundNameToPlay("UI_CorrectInput");
        };
        EventHandler.Event_IncorrectInput += () =>
        {
            SetSoundNameToPlay("UI_IncorrectInput");
        };
        EventHandler.Event_TriggeredHoldBattle += (dummy) =>
        {
            SetSoundNameToPlay("UI_BarCharge");
        };
        EventHandler.Event_StoppedHoldBattle += () =>
        {
            StopSound("UI_BarCharge");
        };
        EventHandler.Event_TriggeredSoloBattle += (dummy, dummy2) =>
        {
            _startedSoloBattle = true;
            SetSoundNameToPlay("UI_SoloBattle");
        };
        EventHandler.Event_StoppedSoloBattle += () =>
        {
            _startedSoloBattle = false;
            ResetPitch("UI_CorrectInput");
        };

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

        //Enemy
        EventHandler.Event_EnemyHitAnimation += (dummy) =>
        {
            _randomizePitch = true;
            SetSoundNameToPlay("Enemy_Hit");
        };
        EventHandler.Event_DefeatedEnemy += (dummy) =>
        {
            _randomizePitch = true;
            SetSoundNameToPlay("Enemy_Defeat");
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

            if (_startedSoloBattle)
                if (sound.soundName.Equals("UI_CorrectInput"))
                {
                    sound.source.pitch =
                        sound.source.pitch < 1f
                            ? sound.source.pitch + 0.05f
                            : sound.source.pitch = 1f;
                }

            sound.source.Play();
        }
        else
        {
            print("Cannot find sound");
        }

        _randomizePitch = false;
    }

    public void StopSound(string name)
    {
        Object_Sound sound = Array.Find(audioList, test => test.soundName.Equals(name));

        if (sound != null)
        {
            sound.source.Stop();
        }
        else
        {
            print("Cannot find sound");
        }
    }

    private void ResetPitch(string name)
    {
        Object_Sound sound = Array.Find(audioList, test => test.soundName.Equals(name));

        if (sound != null)
        {
            sound.source.pitch = sound.pitch;
        }
        else
        {
            print("Cannot find sound");
        }
    }
}
