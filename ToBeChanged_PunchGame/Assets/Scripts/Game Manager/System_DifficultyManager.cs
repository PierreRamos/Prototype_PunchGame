using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_DifficultyManager : MonoBehaviour
{
    System_EventHandler EventHandler;

    System_GlobalValues GlobalValues;

    [Header("Difficulty Settings")]
    [Space]
    [SerializeField]
    int _baseDifficultyIncrement;

    [SerializeField]
    [Range(0f, 0.5f)]
    float _enemySpawnModifierPercentage,
        _enemyMovementSpeedPercentage;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_EnemyDefeatedValueChange += UpdateGameDifficulty;
        EventHandler.Event_DifficultyValueChange += EvaluateGameDifficulty;
    }

    private void OnDisable()
    {
        EventHandler.Event_EnemyDefeatedValueChange -= UpdateGameDifficulty;
        EventHandler.Event_DifficultyValueChange -= EvaluateGameDifficulty;
    }

    private void Start()
    {
        EvaluateGameDifficulty(GlobalValues.GetDifficulty());
    }

    void UpdateGameDifficulty(int value)
    {
        if (_baseDifficultyIncrement <= value)
        {
            GlobalValues.AddDifficulty();
            _baseDifficultyIncrement =
                _baseDifficultyIncrement + (int)(_baseDifficultyIncrement * 1.25f);
        }
    }

    void EvaluateGameDifficulty(int value)
    {
        var difficulty = value;
        var enemySpawnModifier = GlobalValues.GetEnemySpawnModifier();
        var enemyMovementSpeed = GlobalValues.GetEnemyMovementSpeed();

        enemySpawnModifier = difficulty * _enemySpawnModifierPercentage;
        enemyMovementSpeed =
            enemyMovementSpeed * (1 + (difficulty * _enemyMovementSpeedPercentage));

        GlobalValues.SetEnemySpawnModifier(enemySpawnModifier);
        GlobalValues.SetEnemyMovementSpeed(enemyMovementSpeed);
    }
}
