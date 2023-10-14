using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_TimeManager : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    [Header("Time Settings")]
    [Space]
    [SerializeField]
    float _slowTimeValue;

    [SerializeField]
    float _indefiniteSlowTimeValue;

    [SerializeField]
    private float _hitStopDuration;

    private Coroutine _slowTimeTimer;
    private Coroutine _hitStopTimer;
    private bool _runningSlowTimeTimer;
    private bool _runningHitStopTimer;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_StopSlowTime += NormalTime;
        EventHandler.Event_SlowTime += SlowTime;

        EventHandler.Event_TriggeredSoloBattle += SlowTimeIndefinitely;
        EventHandler.Event_TriggeredHoldBattle += SlowTimeIndefinitely;
        EventHandler.Event_TriggerStun += NormalTime;
        EventHandler.Event_PlayerDied += StopTime;
        EventHandler.Event_Pause += PauseTime;

        EventHandler.Event_EnemyHitAnimation += HitStop;
    }

    void OnDisable()
    {
        EventHandler.Event_StopSlowTime -= NormalTime;
        EventHandler.Event_SlowTime -= SlowTime;

        EventHandler.Event_TriggeredSoloBattle -= SlowTimeIndefinitely;
        EventHandler.Event_TriggeredHoldBattle -= SlowTimeIndefinitely;
        EventHandler.Event_TriggerStun -= NormalTime;
        EventHandler.Event_PlayerDied -= StopTime;
        EventHandler.Event_Pause -= PauseTime;

        EventHandler.Event_EnemyHitAnimation -= HitStop;
    }

    void HitStop(GameObject dummy)
    {
        if (_runningHitStopTimer == true)
            StopCoroutine(_hitStopTimer);

        _hitStopTimer = StartCoroutine(HitStopTimer());

        //
        IEnumerator HitStopTimer()
        {
            _runningHitStopTimer = true;
            var currentTimescale = Time.timeScale;
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(_hitStopDuration);

            if (_runningSlowTimeTimer)
                Time.timeScale = currentTimescale;
            else
                NormalTime();

            EventHandler.Event_HitStopFinished?.Invoke();
            _runningHitStopTimer = false;
        }
    }

    void NormalTime()
    {
        Time.timeScale = 1;

        if (_slowTimeTimer != null)
            StopCoroutine(_slowTimeTimer);
    }

    void SlowTimeIndefinitely(GameObject dummy)
    {
        Time.timeScale = _indefiniteSlowTimeValue;

        if (_slowTimeTimer != null)
            StopCoroutine(_slowTimeTimer);
    }

    void SlowTimeIndefinitely(GameObject dummy, List<MoveSet> dummy2)
    {
        Time.timeScale = _indefiniteSlowTimeValue;

        if (_slowTimeTimer != null)
            StopCoroutine(_slowTimeTimer);
    }

    void SlowTime()
    {
        Time.timeScale = _slowTimeValue;

        if (_slowTimeTimer != null)
            StopCoroutine(_slowTimeTimer);

        _slowTimeTimer = StartCoroutine(SlowTimeTimer());

        IEnumerator SlowTimeTimer()
        {
            _runningSlowTimeTimer = true;
            yield return new WaitForSecondsRealtime(
                System_GlobalValues.Instance.GetPlayerKnockBackTime()
            );

            //Checks if games is paused
            var gameState = GlobalValues.GetGameState();
            while (
                gameState == GameState.GameOver
                || gameState == GameState.Paused
                || _runningHitStopTimer == true
            )
            {
                gameState = GlobalValues.GetGameState();
                yield return null;
            }

            NormalTime();
            _runningSlowTimeTimer = false;
        }
    }

    void StopTime()
    {
        Time.timeScale = 0;
    }

    void PauseTime(bool value)
    {
        if (value)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
