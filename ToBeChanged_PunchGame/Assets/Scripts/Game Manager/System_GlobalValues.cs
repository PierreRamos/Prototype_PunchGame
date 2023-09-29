using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitType
{
    Normal,
    Solo,
    Dash,
    Hold
}

public enum MoveSet
{
    Left,
    Right,
    Up,
    Down
}

public enum EnemyType
{
    Easy,
    Medium,
    Hard,
    Elite,
    Dash,
    Hold
}

public enum GameState
{
    Normal,
    SoloBattle,
    HoldBattle,
    GameOver
}

public class System_GlobalValues : MonoBehaviour
{
    public static System_GlobalValues Instance;

    System_EventHandler EventHandler;

    GameState _currentGameState;
    int _difficulty;
    int _currentDefeatCount;
    float _playerKnockBackTime;
    float _playerAttackRange;
    float _enemyMovementSpeed;
    float _enemySpawnModifier;

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
        EventHandler.Event_DefeatedEnemy -= AddDefeatCount;
    }

    private void Start()
    {
        Time.timeScale = 1;
        _currentGameState = GameState.Normal;
    }

    //Incrementers
    void AddDefeatCount(GameObject dummy)
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

    public void SetGameState(GameState gameState)
    {
        _currentGameState = gameState;
    }

    public void SetPlayerKnockBackTime(float value)
    {
        _playerKnockBackTime = value;
    }

    public void SetEnemyMovementSpeed(float value)
    {
        _enemyMovementSpeed = value;
    }

    public void SetPlayerAttackRange(float value)
    {
        _playerAttackRange = value;
    }

    public void SetEnemySpawnModifier(float value)
    {
        _enemySpawnModifier = value;
    }

    //Getters

    public GameState GetGameState()
    {
        return _currentGameState;
    }

    public float GetPlayerKnockBackTime()
    {
        return _playerKnockBackTime;
    }

    public float GetEnemyMovementSpeed()
    {
        return _enemyMovementSpeed;
    }

    public float GetPlayerAttackRange()
    {
        return _playerAttackRange;
    }

    public float GetEnemySpawnModifier()
    {
        return _enemySpawnModifier;
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
