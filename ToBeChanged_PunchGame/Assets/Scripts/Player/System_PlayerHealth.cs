using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_PlayerHealth : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    [Header("Player Health Settings")]
    [Space]
    [SerializeField]
    int _maxPlayerHealth;

    int _currentPlayerHealth;

    void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_PlayerHit += DamagePlayerHealth;
        EventHandler.Event_HealPlayer += HealPlayerHealth;
    }

    void OnDisable()
    {
        EventHandler.Event_PlayerHit -= DamagePlayerHealth;
        EventHandler.Event_HealPlayer -= HealPlayerHealth;
    }

    void Start()
    {
        _currentPlayerHealth = _maxPlayerHealth;
        EventHandler.Event_PlayerHealthValueChange?.Invoke(GetPlayerHealth());
    }

    int GetPlayerHealth()
    {
        return _currentPlayerHealth;
    }

    void HealPlayerHealth(int heal)
    {
        if (_currentPlayerHealth < _maxPlayerHealth)
            _currentPlayerHealth += heal;

        //Prevents health overflow
        if (_currentPlayerHealth > _maxPlayerHealth)
            _currentPlayerHealth = _maxPlayerHealth;

        EventHandler.Event_PlayerHealEffect?.Invoke(transform.position);
        EventHandler.Event_PlayerHealthValueChange?.Invoke(GetPlayerHealth());
    }

    void DamagePlayerHealth(int damage)
    {
        if (_currentPlayerHealth > 0)
            _currentPlayerHealth -= damage;

        if (_currentPlayerHealth <= 0)
        {
            EventHandler.Event_PlayerDied?.Invoke();
            GlobalValues.SetGameState(GameState.GameOver);
        }

        EventHandler.Event_PlayerHealthValueChange?.Invoke(GetPlayerHealth());
    }
}
