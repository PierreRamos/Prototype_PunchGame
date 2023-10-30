using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class System_PlayerSpecial : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Player Special Settings")]
    [SerializeField]
    private float _specialCapValue;

    [SerializeField]
    private float _specialDuration;

    private float _currentSpecialValue;
    private bool _maxedSpecial;
    private bool _specialActive;

    private void Awake()
    {
        EventHandler = System_EventHandler.Instance;
    }

    private void OnEnable()
    {
        EventHandler.Event_DefeatedEnemy += (enemy) =>
        {
            IncreaseSpecial();
        };
        EventHandler.Event_ActivateSpecialInput += () =>
        {
            ActivateSpecial();
        };
    }

    private void Start()
    {
        EventHandler.Event_SetSpecialMeterMaxValue?.Invoke(_specialCapValue);
    }

    private void IncreaseSpecial()
    {
        if (_maxedSpecial)
            return;

        _currentSpecialValue++;

        if (_currentSpecialValue == _specialCapValue)
        {
            _maxedSpecial = true;
            EventHandler.Event_MaxedSpecialMeter?.Invoke();
        }

        EventHandler.Event_SpecialMeterValueChange?.Invoke(_currentSpecialValue);
    }

    private void ActivateSpecial()
    {
        if (!_maxedSpecial || _specialActive)
            return;

        _specialActive = true;
        EventHandler.Event_SpecialActive?.Invoke(true, _specialDuration);
        EventHandler.Event_SpecialMeterValueChange?.Invoke(_currentSpecialValue);

        StartCoroutine(SpecialTimer());

        IEnumerator SpecialTimer()
        {
            yield return new WaitForSeconds(_specialDuration);
            _specialActive = false;
            _maxedSpecial = false;
            _currentSpecialValue = 0;
            EventHandler.Event_SpecialActive?.Invoke(false, _specialDuration);
        }
    }
}
