using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Object_Sound
{
    [HideInInspector]
    public AudioSource source;

    public string soundName;

    public AudioClip sound;

    [Range(0f, 1f)]
    public float volume;

    [Range(0.5f, 1f)]
    public float pitch;
}
