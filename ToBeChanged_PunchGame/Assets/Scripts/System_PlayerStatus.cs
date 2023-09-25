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
    float _stunTimerDuration;

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

    void TriggerStun()
    {
        _isStunned = true;

        if (_stunTimer != null)
            StopCoroutine(_stunTimer);

        _stunTimer = StartCoroutine(StunTimer());

        IEnumerator StunTimer()
        {
            yield return new WaitForSeconds(_stunTimerDuration);
            _isStunned = false;
        }
    }

    void StopStun(int dummy)
    {
        if (_isStunned)
            _isStunned = false;
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
