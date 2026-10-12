using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_DifficultyManager : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    [Header("Difficulty Settings")]
    [SerializeField]
    int _baseDifficulty;

    [SerializeField]
    float _difficultyIncrementPercentageIncrease;

    [SerializeField]
    [Range(0f, 0.5f)]
    float _enemySpawnModifierPercentage;

    [Range(0, 100)]
    [SerializeField]
    float _enemyMovementSpeedPercentage;

    int _currentDifficultyMilestone;

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
        _currentDifficultyMilestone = _baseDifficulty;
        EvaluateGameDifficulty(GlobalValues.GetDifficulty());
    }

    void UpdateGameDifficulty(int enemiesDefeated)
    {
        if (_currentDifficultyMilestone <= enemiesDefeated)
        {
            GlobalValues.AddDifficulty();

            var baseDifficultyAddition = _baseDifficulty * GlobalValues.GetDifficulty();

            _currentDifficultyMilestone += baseDifficultyAddition;
        }
    }

    void EvaluateGameDifficulty(int difficulty)
    {
        if (difficulty == 0)
            return;

        var enemySpawnModifier = GlobalValues.GetEnemySpawnModifier();
        var enemyMovementSpeed = GlobalValues.GetEnemyMovementSpeed();

        enemySpawnModifier = difficulty * _enemySpawnModifierPercentage;

        var enemyMovementSpeedIncrement =
            enemyMovementSpeed * (_enemyMovementSpeedPercentage / 100);

        enemyMovementSpeed = enemyMovementSpeed + enemyMovementSpeedIncrement;

        GlobalValues.SetEnemySpawnModifier(enemySpawnModifier);
        GlobalValues.SetEnemyMovementSpeed(enemyMovementSpeed);
    }
}
