using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class System_SoloBattleTimer : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    [Header("Initialization")]
    [Space]
    [SerializeField]
    Slider _timerSlider;

    [Header("Solo Battle Timer Settings")]
    [SerializeField]
    float _secondsPerMove;

    [SerializeField]
    float _incorrectInputPenalty;

    [Range(0f, 0.5f)]
    [SerializeField]
    float _secondsPerDifficulty;

    float _currentTime;

    int _currentDifficultyCache;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_SetSoloBattleTimer += (moveCount) =>
        {
            ActivateSoloBattleTimer(moveCount);
        };
        EventHandler.Event_IncorrectInput += () =>
        {
            DecreaseTime();
        };
    }

    private void OnDisable()
    {
        EventHandler.Event_IncorrectInput -= DecreaseTime;
        EventHandler.Event_SetSoloBattleTimer -= ActivateSoloBattleTimer;
    }

    private void Update()
    {
        var gameState = GlobalValues.GetGameState();
        if (gameState == GameState.GameOver || gameState == GameState.Paused)
            return;

        SoloBattleTimer();
    }

    void ActivateSoloBattleTimer(int moveCount)
    {
        _currentTime = moveCount * _secondsPerMove;
        _timerSlider.value = _currentTime;
        _timerSlider.maxValue = _currentTime;
        _currentDifficultyCache = GlobalValues.GetDifficulty();
    }

    void SoloBattleTimer()
    {
        if (_currentTime > 0)
            _currentTime -=
                (1 + (_currentDifficultyCache * _secondsPerDifficulty)) * Time.unscaledDeltaTime;
        else
            EventHandler.Event_SoloBattleTimerFinished?.Invoke(false);

        _timerSlider.value = _currentTime;
    }

    void DecreaseTime()
    {
        _currentTime -= _incorrectInputPenalty;
    }
}
