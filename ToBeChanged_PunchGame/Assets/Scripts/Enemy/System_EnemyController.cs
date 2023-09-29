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

    Coroutine _addForceTimer;

    bool _isFacingRight = true;

    bool _isMoving = true;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_EnemyHit += CheckIfHit;
        EventHandler.Event_EnemyFlip += Flip;
        EventHandler.Event_DefeatedEnemy += TriggerDisableSelf;
        EventHandler.Event_EnemyHitPlayer += TriggerDisableSelfHitPlayer;

        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (_playerTransform != null)
        {
            if (_playerTransform.position.x > transform.position.x)
                _isFacingRight = true;
            else
                Flip(gameObject);
        }

        _isMoving = true;
    }

    void OnDisable()
    {
        EventHandler.Event_EnemyHit -= CheckIfHit;
        EventHandler.Event_EnemyFlip -= Flip;
        EventHandler.Event_DefeatedEnemy -= TriggerDisableSelf;
        EventHandler.Event_EnemyHitPlayer -= TriggerDisableSelfHitPlayer;

        if (!_isFacingRight)
            Flip(gameObject);

        if (!_collider.enabled)
            _collider.enabled = true;
    }

    void Start()
    {
        GlobalValues.SetEnemyMovementSpeed(_enemyMovementSpeed);

        _collider = GetComponent<Collider2D>();
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

            gameObject.SetActive(false);
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

    void CheckIfHit(GameObject gameObject)
    {
        if (
            this.gameObject == gameObject
            && GlobalValues.GetGameState() == GameState.Normal
            && gameObject.activeSelf == true
        )
        {
            GiveKnockback();
        }
    }

    //Pushes game object back
    void GiveKnockback()
    {
        if (_addForceTimer != null)
            StopCoroutine(_addForceTimer);

        _addForceTimer = StartCoroutine(AddForceTimer());

        IEnumerator AddForceTimer()
        {
            if (_isFacingRight)
                _rigidBody.AddForce(
                    -transform.right.normalized * _knockBackForce,
                    ForceMode2D.Impulse
                );
            else
                _rigidBody.AddForce(
                    transform.right.normalized * _knockBackForce,
                    ForceMode2D.Impulse
                );

            StopMovement();

            //Review this realtime vs non realtime
            yield return new WaitForSecondsRealtime(
                System_GlobalValues.Instance.GetPlayerKnockBackTime()
            );

            StartMovement();
        }
    }

    void StartMovement()
    {
        _isMoving = true;
    }

    void StopMovement()
    {
        _isMoving = false;
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
