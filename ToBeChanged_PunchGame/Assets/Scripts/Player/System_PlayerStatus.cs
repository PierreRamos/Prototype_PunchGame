using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_PlayerStatus : MonoBehaviour
{
    public static System_PlayerStatus Instance { get; private set; }

    System_EventHandler EventHandler;

    [Header("Player Status Settings")]
    [Space]
    [SerializeField]
    float _stunTimerDuration,
        _currentStunTime;

    bool _isStunned;

    Coroutine _stunTimer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_TriggerStun += TriggerStun;
        EventHandler.Event_PlayerHit += StopStun;
    }

    private void OnDisable()
    {
        EventHandler.Event_TriggerStun -= TriggerStun;
        EventHandler.Event_PlayerHit -= StopStun;
    }

    private void Update()
    {
        StunTimer();
    }

    void StunTimer()
    {
        if (_currentStunTime > 0)
        {
            _currentStunTime -= 1 * Time.unscaledDeltaTime;
            EventHandler.Event_PlayerStunTimeChange?.Invoke(_currentStunTime);
        }
        else
        {
            EventHandler.Event_PlayerStunFinished?.Invoke();
            _isStunned = false;
        }
    }

    void TriggerStun()
    {
        _isStunned = true;

        _currentStunTime = _stunTimerDuration;
    }

    void StopStun(int dummy)
    {
        if (_isStunned)
        {
            _isStunned = false;
            EventHandler.Event_PlayerStunFinished?.Invoke();
        }
        else
            return;

        if (_stunTimer != null)
            StopCoroutine(_stunTimer);
    }

    public bool PlayerIsStunned()
    {
        return _isStunned;
    }
}
