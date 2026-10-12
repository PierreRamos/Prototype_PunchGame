using System.Collections;
using UnityEngine;
using Cinemachine;

public class System_CameraShake : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    [Header("Initialization")]
    [SerializeField]
    CinemachineVirtualCamera _cinemachineCamera;

    [Header("Camera Shake Settings")]
    [SerializeField]
    float _hitShakeDuration;

    [SerializeField]
    float _defeatShakeDuration;

    [SerializeField]
    float _shakeAmplitude;

    [SerializeField]
    float _shakeFrequency;
    private CinemachineBasicMultiChannelPerlin noise;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_EnemyHitAnimation += TriggerEnemyHitShake;
        EventHandler.Event_DefeatedEnemy += TriggerEnemyDefeatedShake;
    }

    private void OnDisable()
    {
        EventHandler.Event_EnemyHitAnimation -= TriggerEnemyHitShake;
        EventHandler.Event_DefeatedEnemy -= TriggerEnemyDefeatedShake;
    }

    private void Start()
    {
        if (_cinemachineCamera != null)
        {
            noise =
                _cinemachineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    void TriggerEnemyHitShake(GameObject dummy)
    {
        ShakeCamera(_hitShakeDuration, _shakeAmplitude, _shakeFrequency);
    }

    void TriggerEnemyDefeatedShake(GameObject dummy)
    {
        ShakeCamera(_defeatShakeDuration, _shakeAmplitude * 1.5f, _shakeFrequency * 1.5f);
    }

    void ShakeCamera(float duration, float amplitude, float frequency)
    {
        bool playerSpecialActive = GlobalValues.GetPlayerSpecialActive();

        if (noise != null)
        {
            // Start the camera shake by modifying the noise settings
            noise.m_AmplitudeGain = playerSpecialActive ? amplitude * 2 : amplitude;
            noise.m_FrequencyGain = playerSpecialActive ? frequency * 2 : frequency;

            // Stop the camera shake after the specified duration
            StartCoroutine(StopShake(duration));
        }
    }

    private IEnumerator StopShake(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Reset the noise settings to stop the shake
        if (noise != null)
        {
            noise.m_AmplitudeGain = 0f;
            noise.m_FrequencyGain = 0f;
        }
    }
}
