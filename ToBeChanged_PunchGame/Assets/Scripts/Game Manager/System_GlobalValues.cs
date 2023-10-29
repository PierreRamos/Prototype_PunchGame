using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroundLocation
{
    Left,
    Center,
    Right
}

public enum Direction
{
    Left,
    Right
}

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
    Normal,
    Elite,
    Dash,
    Hold
}

public enum GameState
{
    Normal,
    SoloBattle,
    HoldBattle,
    Paused,
    GameOver
}

public class System_GlobalValues : MonoBehaviour
{
    public static System_GlobalValues Instance;
    System_EventHandler EventHandler;

    [SerializeField]
    int _targetFrameRate;

    GameState _currentGameState;
    Dictionary<EnemyType, int> _enemiesSpawnChance = new Dictionary<EnemyType, int>();
    int _difficulty;
    int _currentDefeatCount;
    int _currentPlayerHealth;
    int _movesToHitCount;
    float _playerStunTime;
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
        EventHandler.Event_PlayerHealthValueChange += SetPlayerHealth;
    }

    void OnDisable()
    {
        EventHandler.Event_DefeatedEnemy -= AddDefeatCount;
        EventHandler.Event_PlayerHealthValueChange -= SetPlayerHealth;
    }

    private void Start()
    {
        Time.timeScale = 1;
        _currentGameState = GameState.Normal;

        Application.targetFrameRate = _targetFrameRate;
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

    public void SetEnemiesSpawnChance(Dictionary<EnemyType, int> enemiesSpawnChance)
    {
        _enemiesSpawnChance = enemiesSpawnChance;
    }

    public void SetPlayerStunTime(float value)
    {
        _playerStunTime = value;
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

    public void SetPlayerHealth(int value)
    {
        _currentPlayerHealth = value;
    }

    //Getters

    public GameState GetGameState()
    {
        return _currentGameState;
    }

    public int GetEnemiesSpawnChance(EnemyType enemyType)
    {
        return _enemiesSpawnChance[enemyType];
    }

    public float GetPlayerStunTime()
    {
        return _playerStunTime;
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

    public int GetPlayerHealth()
    {
        return _currentPlayerHealth;
    }

    //Incrementers
    void AddDefeatCount(GameObject dummy)
    {
        _currentDefeatCount++;
        EventHandler.Event_EnemyDefeatedValueChange?.Invoke(GetDefeatCount());
    }
}
