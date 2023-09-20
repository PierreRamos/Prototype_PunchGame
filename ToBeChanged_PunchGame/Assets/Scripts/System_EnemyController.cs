using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class System_EnemyController : MonoBehaviour
{
    [Header("Enemy Controller Settings")]
    [SerializeField]
    float _movementSpeed;

    [SerializeField]
    float _knockBackForce;

    [SerializeField]
    float _timeBeforeMoving;

    System_EventHandler EventHandler;

    Rigidbody2D _rigidBody;

    Transform _playerTransform;

    bool _isFacingRight;

    bool _isMoving = true;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_EnemyHit += CheckIfHit;

        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (_playerTransform.position.x > transform.position.x)
            _isFacingRight = true;
        else
            _isFacingRight = false;

        if (!_isFacingRight)
            Flip();
    }

    void OnDisable()
    {
        EventHandler.Event_EnemyHit -= CheckIfHit;
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
        if (_isMoving)
        {
            // Calculate the step size based on the movement speed and time
            float step = _movementSpeed * Time.deltaTime;

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
        if (this.gameObject == gameObject)
        {
            GiveKnockback();
        }
    }

    //Pushes game object back
    void GiveKnockback()
    {
        StartCoroutine(AddForceTimer());

        IEnumerator AddForceTimer()
        {
            _rigidBody.AddForce(-transform.right.normalized * _knockBackForce, ForceMode2D.Impulse);

            StopMovement();

            yield return new WaitForSecondsRealtime(_timeBeforeMoving);

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
            Destroy(gameObject);
        }
    }
}
