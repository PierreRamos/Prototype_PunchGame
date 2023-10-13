using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class System_HoldBattle : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    [Header("Initialization")]
    [SerializeField]
    GameObject _holdBattlePanel;

    [SerializeField]
    Image _highlight; //Highlight of area that needs to be hit

    [SerializeField]
    Slider _holdBattleSlider;

    [Header("Hold Battle Settings")]
    [Range(0, 1)]
    [SerializeField]
    float _sliderIncrementValue;

    [Range(0, 1)]
    [SerializeField]
    float _highlightPercentage;

    [Range(0, 0.5f)]
    [SerializeField]
    float _minimumOffsetPercentage;

    [Range(0f, 0.5f)]
    [SerializeField]
    float _secondsPerDifficulty;

    GameObject _currentEnemy;

    float _offsetPercentage;

    float _currentDifficultyCache;

    bool _isHeld;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_TriggeredHoldBattle += TriggerHoldBattle;
        EventHandler.Event_StoppedHoldInput += StopHold;
    }

    private void OnDisable()
    {
        EventHandler.Event_TriggeredHoldBattle -= TriggerHoldBattle;
        EventHandler.Event_StoppedHoldInput -= StopHold;
    }

    private void Update()
    {
        IncrementSlider();
    }

    private void IncrementSlider()
    {
        if (_isHeld)
        {
            _holdBattleSlider.value +=
                (_sliderIncrementValue + (_currentDifficultyCache * _secondsPerDifficulty))
                * Time.unscaledDeltaTime;

            if (_holdBattleSlider.value == _holdBattleSlider.maxValue)
                StopHold();
        }
    }

    void TriggerHoldBattle(GameObject enemy)
    {
        _currentEnemy = enemy;

        if (!_holdBattlePanel.activeSelf)
            _holdBattlePanel.SetActive(true);

        _isHeld = true;
        _holdBattleSlider.value = 0;
        _currentDifficultyCache = GlobalValues.GetDifficulty();
        CalculatePercentagePosition();
    }

    void DeactivateHoldBattle()
    {
        if (_holdBattlePanel.activeSelf)
            _holdBattlePanel.SetActive(false);
    }

    void StopHold()
    {
        _isHeld = false;
        EvaluateValues();
        GlobalValues.SetGameState(GameState.Normal);
    }

    void EvaluateValues()
    {
        var sliderValue = _holdBattleSlider.value;

        var minRange = _offsetPercentage;
        var maxRange = _offsetPercentage + _highlightPercentage;

        if (sliderValue >= minRange && sliderValue <= maxRange)
        {
            //Inside range
            EventHandler.Event_DefeatedEnemy(_currentEnemy);
            EventHandler.Event_EnemyHitConfirm(_currentEnemy);
        }
        else
        {
            //Outside range
            EventHandler.Event_PlayerHit(1);
            EventHandler.Event_EnemyHitPlayer(_currentEnemy);
        }

        DeactivateHoldBattle();
        EventHandler.Event_StopSlowTime?.Invoke();
        EventHandler.Event_StoppedHoldBattle?.Invoke();
        EventHandler.Event_NormalHealthUI?.Invoke();
    }

    //Test
    void CalculatePercentagePosition()
    {
        var parentWidth = _holdBattleSlider.GetComponent<RectTransform>().rect.width;

        _offsetPercentage = UnityEngine.Random.Range(
            _minimumOffsetPercentage,
            1 - _highlightPercentage
        );

        //Initial gap before highlight
        float initialOffset = _offsetPercentage * parentWidth;

        //Percentage width of the highlight
        float fillWidth = _highlightPercentage * parentWidth;

        //Subtracting half of parent width since localPosition sets the 0 at x axis to center of parent not pivot
        _highlight.transform.localPosition = new Vector2(
            initialOffset - parentWidth / 2,
            _highlight.transform.localPosition.y
        );

        // Set the width of the highlight image
        _highlight.rectTransform.sizeDelta = new Vector2(
            fillWidth,
            _highlight.rectTransform.sizeDelta.y
        );
    }
}
