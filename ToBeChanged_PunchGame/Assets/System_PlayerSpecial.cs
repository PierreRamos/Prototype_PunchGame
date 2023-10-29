using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_PlayerSpecial : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Player Special Settings")]
    [SerializeField]
    private float _specialCapValue;

    private float _currentSpecialValue;
    private bool _maxedSpecial;

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
}
