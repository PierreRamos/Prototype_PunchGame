using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class System_PlayerAttack : MonoBehaviour
{
    System_EventHandler EventHandler;

    System_PlayerStatus PlayerStatus;

    [Header("Player Attack Settings")]
    [SerializeField]
    float _rangeDistance;

    [SerializeField]
    float _moveToEnemyDistance;

    [SerializeField]
    bool _debugOn;
    SpriteRenderer _spriteRenderer;

    bool _isFacingRight = true;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        EventHandler = System_EventHandler.Instance;
        PlayerStatus = System_PlayerStatus.Instance;

        EventHandler.Event_AttackLeft += HitCheckLeft;
        EventHandler.Event_AttackRight += HitCheckRight;
    }

    void OnDisable()
    {
        EventHandler.Event_AttackLeft += HitCheckLeft;
        EventHandler.Event_AttackRight += HitCheckRight;
    }

    void Update()
    {
        CheckEnemyInRange();
        DebugDrawRayCast();
    }

    //Debug method: Draws attack range when debug is on
    void DebugDrawRayCast()
    {
        if (_debugOn)
        {
            Debug.DrawRay(transform.position, -transform.right * _rangeDistance, Color.red);
            Debug.DrawRay(transform.position, transform.right * _rangeDistance, Color.red);
        }
    }

    //Called in Update: Checks if enemies are in range
    void CheckEnemyInRange()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(
            transform.position,
            -transform.right,
            _rangeDistance
        );

        RaycastHit2D rightHit = Physics2D.Raycast(
            transform.position,
            transform.right,
            _rangeDistance
        );

        if (leftHit.collider != null || rightHit.collider != null)
        {
            var leftObjectHit = leftHit.collider?.gameObject;
            var rightObjectHit = rightHit.collider?.gameObject;

            if (leftObjectHit != null && leftObjectHit.CompareTag("Enemy"))
                EventHandler.Event_HasEnemyLeft?.Invoke(true);
            else if (leftObjectHit == null)
                EventHandler.Event_HasEnemyLeft?.Invoke(false);

            if (rightObjectHit != null && rightObjectHit.CompareTag("Enemy"))
                EventHandler.Event_HasEnemyRight?.Invoke(true);
            else if (rightObjectHit == null)
                EventHandler.Event_HasEnemyRight?.Invoke(false);
        }
        else
        {
            EventHandler.Event_HasEnemyLeft?.Invoke(false);
            EventHandler.Event_HasEnemyRight?.Invoke(false);
        }
    }

    void HitCheckLeft()
    {
        if (PlayerStatus.PlayerIsStunned() == false)
            HitCheckDirection(-transform.right, "left");
    }

    void HitCheckRight()
    {
        if (PlayerStatus.PlayerIsStunned() == false)
            HitCheckDirection(transform.right, "right");
    }

    //Checks if there is game object tagged "Enemy" on either left or right of player
    void HitCheckDirection(Vector2 direction, string side)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _rangeDistance);

        if (hit.collider != null)
        {
            var objectHit = hit.collider.gameObject;
            if (objectHit.CompareTag("Enemy"))
            {
                ConfirmHit(objectHit, side);
            }
            else
            {
                EventHandler.Event_TriggerStun?.Invoke();
                EventHandler.Event_ExclamationEffect?.Invoke(
                    new Vector3(transform.position.x, transform.position.y + 0.5f)
                );
            }
        }
        else
        {
            EventHandler.Event_TriggerStun?.Invoke();
            EventHandler.Event_ExclamationEffect?.Invoke(
                new Vector3(transform.position.x, transform.position.y + 0.5f)
            );
        }

        //Internal
        void ConfirmHit(GameObject objectHit, string side)
        {
            EventHandler.Event_EnemyHit?.Invoke(objectHit);
            EventHandler.Event_HitEffect?.Invoke(objectHit.transform.position);
            MoveToHitEnemy(objectHit.transform, side);
            CheckDirection(objectHit.transform.position);
        }
    }

    //Called when enemy is hit: Moves to enemy within specific distance and checks if there is enemies still close and slows time
    void MoveToHitEnemy(Transform enemyPosition, string side)
    {
        var targetPosition = enemyPosition.position;

        if (side.Equals("left"))
        {
            targetPosition += new Vector3(_moveToEnemyDistance, 0f);
        }
        else if (side.Equals("right"))
        {
            targetPosition += new Vector3(-_moveToEnemyDistance, 0f);
        }

        EventHandler.Event_MoveToEnemy?.Invoke(targetPosition);

        StartCoroutine(CheckForEnemyCloseAfterMove(targetPosition));

        //Internal
        IEnumerator CheckForEnemyCloseAfterMove(Vector3 targetPosition)
        {
            yield return new WaitForEndOfFrame();

            RaycastHit2D[] leftHits = Physics2D.RaycastAll(
                targetPosition,
                -transform.right,
                _rangeDistance
            );
            RaycastHit2D[] rightHits = Physics2D.RaycastAll(
                targetPosition,
                transform.right,
                _rangeDistance
            );

            List<GameObject> detectedEnemies = new List<GameObject>();

            foreach (var hit in leftHits)
            {
                if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                {
                    detectedEnemies.Add(hit.collider.gameObject);
                }
            }

            foreach (var hit in rightHits)
            {
                if (hit.collider != null && hit.collider.CompareTag("Enemy"))
                {
                    detectedEnemies.Add(hit.collider.gameObject);
                }
            }

            if (detectedEnemies.Count > 0)
            {
                // Invoke the slow time event or handle detectedEnemies as needed
                EventHandler.Event_SlowTime?.Invoke();
            }
            else
            {
                EventHandler.Event_StopSlowTime?.Invoke();
            }
        }
    }

    //Called when attacking: Checks orientation relative to the enemy being hit
    void CheckDirection(Vector3 target)
    {
        if (transform.position.x > target.x && _isFacingRight)
            Flip();
        else if (transform.position.x < target.x && !_isFacingRight)
            Flip();

        //Internal
        void Flip()
        {
            _isFacingRight = !_isFacingRight;
            _spriteRenderer.flipX = !_isFacingRight;
        }
    }
}
