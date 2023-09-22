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

    Transform _playerTransform;

    Coroutine _addForceTimer;

    bool _isFacingRight = true;

    bool _isMoving = true;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_EnemyHit += CheckIfHit;

        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (_playerTransform != null)
        {
            if (_playerTransform.position.x > transform.position.x)
                _isFacingRight = true;
            else
                _isFacingRight = false;

            if (!_isFacingRight)
                Flip();
        }

        _isMoving = true;
    }

    void OnDisable()
    {
        EventHandler.Event_EnemyHit -= CheckIfHit;

        if (!_isFacingRight)
            Flip();
    }

    void Start()
    {
        GlobalValues.SetEnemyMovementSpeed(_enemyMovementSpeed);
    }

    void Update()
    {
        MoveTowardsPlayer();
    }

    void Flip()
    {
        transform.Rotate(Vector3.up, 180f);
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
            _rigidBody.AddForce(-transform.right.normalized * _knockBackForce, ForceMode2D.Impulse);

            StopMovement();

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
            gameObject.SetActive(false);
        }
    }
}
