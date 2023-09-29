using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class System_SoloBattleTimer : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Initialization")]
    [Space]
    [SerializeField]
    Slider _timerSlider;

    [Header("Solo Battle Timer Settings")]
    [Space]
    [SerializeField]
    float _secondsPerMove;

    [SerializeField]
    float _incorrectInputPenalty;

    float _currentTime;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_SoloBattleWrongInput += DecreaseTime;
        EventHandler.Event_UpdateSoloBattleTimer += ActivateSoloBattleTimer;
    }

    private void OnDisable()
    {
        EventHandler.Event_SoloBattleWrongInput -= DecreaseTime;
        EventHandler.Event_UpdateSoloBattleTimer -= ActivateSoloBattleTimer;
    }

    private void Update()
    {
        SoloBattleTimer();
    }

    void ActivateSoloBattleTimer(int moveCount)
    {
        _currentTime = moveCount * _secondsPerMove;
        _timerSlider.value = _currentTime;
        _timerSlider.maxValue = _currentTime;
    }

    void SoloBattleTimer()
    {
        if (_currentTime > 0)
            _currentTime -= 1 * Time.unscaledDeltaTime;
        else
            EventHandler.Event_SoloBattleTimerFinished?.Invoke(false);

        _timerSlider.value = _currentTime;
    }

    void DecreaseTime()
    {
        _currentTime -= _incorrectInputPenalty;
    }
}
