using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_GlobalValues : MonoBehaviour
{
    public static System_GlobalValues Instance;

    System_EventHandler EventHandler;

    [Header("Global Value Settings")]
    [Space]
    [SerializeField]
    float _playerKnockBackTime;

    [SerializeField]
    float _enemyMovementSpeed;

    [SerializeField]
    int _difficulty;
    int _currentDefeatCount;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_DefeatedEnemy += AddDefeatCount;
    }

    void OnDisable()
    {
        EventHandler.Event_DefeatedEnemy += AddDefeatCount;
    }

    //Incrementers
    void AddDefeatCount()
    {
        _currentDefeatCount++;
        EventHandler.Event_EnemyDefeatedValueChange?.Invoke(GetDefeatCount());
    }

    public void AddDifficulty()
    {
        _difficulty++;
        EventHandler.Event_DifficultyValueChange?.Invoke(GetDifficulty());
    }

    //Setters
    public void SetEnemyMovementSpeed(float value)
    {
        _enemyMovementSpeed = value;
    }

    //Getters

    public float GetPlayerKnockBackTime()
    {
        return _playerKnockBackTime;
    }

    public float GetEnemyMovementSpeed()
    {
        return _enemyMovementSpeed;
    }

    public int GetDefeatCount()
    {
        return _currentDefeatCount;
    }

    public int GetDifficulty()
    {
        return _difficulty;
    }
}
