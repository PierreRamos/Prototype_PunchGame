using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class System_EnemyController : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    [Header("Enemy Controller Settings")]
    [SerializeField]
    int _enemyDamage;

    [SerializeField]
    float _knockBackForce;

    [SerializeField]
    float _enemyMovementSpeed;

    Rigidbody2D _rigidBody;

    Collider2D _collider;

    SpriteRenderer _spriteRenderer;

    Transform _playerTransform;

    Coroutine _stunTimer;
    bool _runningStunTimer;
    bool _isFacingRight = true;
    bool _isMoving = true;
    bool _isDefeated;

    Action<GameObject> holdBattleDelegate;
    Action<GameObject, List<MoveSet>> soloBattleDelegate;
    Action<GameObject> defeatedEnemyDelegate;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        holdBattleDelegate = (enemy) =>
        {
            if (gameObject == enemy)
            {
                StopMovement(enemy);
                HaltMovement();
            }
        };

        soloBattleDelegate = (enemy, moveSet) =>
        {
            if (gameObject == enemy)
            {
                StopMovement(enemy);
                HaltMovement();
            }
        };
        defeatedEnemyDelegate = (enemy) =>
        {
            if (gameObject == enemy)
            {
                _isDefeated = true;

                if (_runningStunTimer == true)
                    StopCoroutine(_stunTimer);
            }
        };
    }

    void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_EnemyHitConfirm += StopMovement;
        EventHandler.Event_EnemyFlip += Flip;
        EventHandler.Event_EnemyHitAnimation += TriggerStun;
        EventHandler.Event_DefeatedEnemy += GiveKnockback;
        EventHandler.Event_DefeatedEnemy += TriggerDisableSelf;
        EventHandler.Event_EnemyHitPlayer += TriggerDisableSelfHitPlayer;
        EventHandler.Event_RemoveEnemy += DisableSelfOverride;

        EventHandler.Event_TriggeredHoldBattle += holdBattleDelegate;
        EventHandler.Event_TriggeredSoloBattle += soloBattleDelegate;
        EventHandler.Event_DefeatedEnemy += defeatedEnemyDelegate;

        if (_playerTransform != null)
        {
            if (_playerTransform.position.x > transform.position.x)
                _isFacingRight = true;
            else
                Flip(gameObject);
        }

        if (GlobalValues.GetEnemyMovementSpeed() == 0f)
            GlobalValues.SetEnemyMovementSpeed(_enemyMovementSpeed);

        StartMovement();
    }

    void OnDisable()
    {
        EventHandler.Event_EnemyHitConfirm -= StopMovement;
        EventHandler.Event_EnemyFlip -= Flip;
        EventHandler.Event_EnemyHitAnimation -= TriggerStun;
        EventHandler.Event_DefeatedEnemy -= GiveKnockback;
        EventHandler.Event_DefeatedEnemy -= TriggerDisableSelf;
        EventHandler.Event_EnemyHitPlayer -= TriggerDisableSelfHitPlayer;
        EventHandler.Event_RemoveEnemy -= DisableSelfOverride;

        EventHandler.Event_TriggeredHoldBattle -= holdBattleDelegate;
        EventHandler.Event_TriggeredSoloBattle -= soloBattleDelegate;
        EventHandler.Event_DefeatedEnemy -= defeatedEnemyDelegate;

        if (!_isFacingRight)
            Flip(gameObject);

        if (!_collider.enabled)
            _collider.enabled = true;

        _isDefeated = false;
    }

    void Update()
    {
        MoveTowardsPlayer();
    }

    public Transform GetPlayerPosition()
    {
        return _playerTransform;
    }

    public bool GetIsFacingRight()
    {
        return _isFacingRight;
    }

    void Flip(GameObject gameObject)
    {
        if (this.gameObject != gameObject)
            return;

        _isFacingRight = !_isFacingRight;

        // transform.Rotate(Vector3.up, 180f);

        //test
        _spriteRenderer.flipX = !_spriteRenderer.flipX;
    }

    private void DisableSelfOverride(GameObject enemy)
    {
        if (gameObject != enemy)
            return;

        gameObject.SetActive(false);
    }

    //This is for when enemy is defeated by player (no animation yet)
    void TriggerDisableSelf(GameObject enemy)
    {
        if (gameObject != enemy)
            return;

        _collider.enabled = false;
        StartCoroutine(DisableSelf());

        //
        IEnumerator DisableSelf()
        {
            yield return new WaitForEndOfFrame(); //Waits for other methods relying on defeated enemy event to happen first before disabling this game object

            EventHandler.Event_EnemyDeathAnimation?.Invoke(gameObject);
            StopMovement(enemy);
        }
    }

    //This is for when player is hit by enemy (no animation yet)
    void TriggerDisableSelfHitPlayer(GameObject enemy)
    {
        if (gameObject != enemy)
            return;

        _collider.enabled = false;
        StartCoroutine(DisableSelf());

        //
        IEnumerator DisableSelf()
        {
            yield return new WaitForEndOfFrame(); //Waits for other methods relying on defeated enemy event to happen first before disabling this game object

            // EventHandler.Event_EnemyDeathAnimation?.Invoke(gameObject);
            gameObject.SetActive(false);
        }
    }

    void MoveTowardsPlayer()
    {
        if (_isMoving && GlobalValues.GetGameState() == GameState.Normal)
        {
            // Calculate the step size based on the movement speed and time
            float step = GlobalValues.GetEnemyMovementSpeed() * Time.deltaTime;

            // Move towards the target
            transform.position = Vector3.MoveTowards(
                transform.position,
                _playerTransform.position,
                step
            );
        }
    }

    //Pushes game object back when defeated
    private void GiveKnockback(GameObject enemy)
    {
        if (gameObject != enemy)
            return;

        if (_isFacingRight)
            _rigidBody.AddForce(-transform.right.normalized * _knockBackForce, ForceMode2D.Impulse);
        else
            _rigidBody.AddForce(transform.right.normalized * _knockBackForce, ForceMode2D.Impulse);
    }

    private void TriggerStun(GameObject enemy)
    {
        StopMovement(enemy);

        if (_isDefeated)
            return;

        if (_runningStunTimer)
            StopCoroutine(_stunTimer);

        _stunTimer = StartCoroutine(StunTimer());

        IEnumerator StunTimer()
        {
            _runningStunTimer = true;
            yield return new WaitForSeconds(GlobalValues.GetPlayerStunTime());
            StartMovement();
            _runningStunTimer = false;
        }
    }

    //Starts movement towards player
    void StartMovement()
    {
        _isMoving = true;
    }

    //Stops movement towards player
    void StopMovement(GameObject enemy)
    {
        if (gameObject != enemy)
            return;

        _isMoving = false;
    }

    //Completely stops all movement
    private void HaltMovement()
    {
        _rigidBody.velocity = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            EventHandler.Event_PlayerHit(_enemyDamage);
            EventHandler.Event_EnemyHitPlayer?.Invoke(gameObject);

            // gameObject.SetActive(false); //Keep for now until has enemy animation if hit enemy
        }
    }
}
