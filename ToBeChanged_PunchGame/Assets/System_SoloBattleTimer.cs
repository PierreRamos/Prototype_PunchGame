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
    float _soloBattleDuration;

    float _currentTime;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        _currentTime = _soloBattleDuration;
        _timerSlider.value = _soloBattleDuration;
    }

    private void Start()
    {
        _timerSlider.maxValue = _soloBattleDuration;
    }

    private void Update()
    {
        SoloBattleTimer();
    }

    void SoloBattleTimer()
    {
        if (_currentTime > 0)
            _currentTime -= 1 * Time.unscaledDeltaTime;
        else
            EventHandler.Event_SoloBattleTimerFinished?.Invoke(false);

        _timerSlider.value = _currentTime;
    }
}
