using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_PlayerHealth : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Player Health Settings")]
    [Space]
    [SerializeField]
    int _maxPlayerHealth;

    int _currentPlayerHealth;

    private void Start()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_PlayerHit += UpdatePlayerHealth;

        _currentPlayerHealth = _maxPlayerHealth;
        EventHandler.Event_PlayerHealthValueChange?.Invoke(GetPlayerHealth());
    }

    int GetPlayerHealth()
    {
        return _currentPlayerHealth;
    }

    void UpdatePlayerHealth(int damage)
    {
        _currentPlayerHealth -= damage;

        EventHandler.Event_PlayerHealthValueChange?.Invoke(GetPlayerHealth());

        if (_currentPlayerHealth <= 0)
        {
            EventHandler.Event_PlayerDied?.Invoke();
            return;
        }
    }
}
