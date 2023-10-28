using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_PlayerSpecial : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Player Special Settings")]
    [SerializeField]
    private int _specialCapValue;

    private int _currentSpecialValue;
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

    private void IncreaseSpecial()
    {
        if (_maxedSpecial)
            return;

        _currentSpecialValue++;

        if (_currentSpecialValue == _specialCapValue)
        {
            _maxedSpecial = true;
            print("maxed");
        }
    }
}
