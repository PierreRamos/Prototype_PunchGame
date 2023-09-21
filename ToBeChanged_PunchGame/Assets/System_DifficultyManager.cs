using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_DifficultyManager : MonoBehaviour
{
    System_EventHandler EventHandler;

    System_GlobalValues GlobalValues;

    [SerializeField]
    int _baseDifficultyIncrement;

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
        var enemyMovementSpeed = GlobalValues.GetEnemyMovementSpeed();

        enemyMovementSpeed = enemyMovementSpeed * (1 + (GlobalValues.GetDifficulty() * 0.1f));

        GlobalValues.SetEnemyMovementSpeed(enemyMovementSpeed);
    }
}
