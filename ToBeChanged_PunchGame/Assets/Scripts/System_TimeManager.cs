using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    [Space]
    [SerializeField]
    float _slowTimeValue;

    System_EventHandler EventHandler;

    Coroutine _slowTimeTimer;

    void Start()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_StopSlowTime += NormalTime;
        EventHandler.Event_SlowTime += SlowTime;
        EventHandler.Event_TriggeredSoloBattle += SlowTimeIndefinitely;
        EventHandler.Event_PlayerDied += StopTime;
    }

    void OnDisable()
    {
        EventHandler.Event_StopSlowTime -= NormalTime;
        EventHandler.Event_SlowTime -= SlowTime;
        EventHandler.Event_TriggeredSoloBattle -= SlowTimeIndefinitely;
        EventHandler.Event_PlayerDied -= StopTime;
    }

    void NormalTime()
    {
        Time.timeScale = 1;

        if (_slowTimeTimer != null)
            StopCoroutine(_slowTimeTimer);
    }

    void SlowTimeIndefinitely(GameObject gameObject, List<MoveSet> listOfMoves)
    {
        Time.timeScale = _slowTimeValue;

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
            yield return new WaitForSecondsRealtime(
                System_GlobalValues.Instance.GetPlayerKnockBackTime()
            );
            NormalTime();
        }
    }

    void StopTime()
    {
        Time.timeScale = 0;
    }
}
