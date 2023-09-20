using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_PlayerAttack : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Player Attack Settings")]
    [SerializeField]
    float rangeDistance;

    [SerializeField]
    float moveToEnemyDistance;
    SpriteRenderer _spriteRenderer;

    bool _isFacingRight = true;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_AttackLeft += HitCheckLeft;
        EventHandler.Event_AttackRight += HitCheckRight;
    }

    private void OnDisable()
    {
        EventHandler.Event_AttackLeft += HitCheckLeft;
        EventHandler.Event_AttackRight += HitCheckRight;
    }

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, -transform.right * rangeDistance, Color.red);
        Debug.DrawRay(transform.position, transform.right * rangeDistance, Color.red);
    }

    void HitCheckLeft()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(
            transform.position,
            -transform.right,
            rangeDistance
        );

        if (leftHit.collider != null)
        {
            var objectHit = leftHit.collider.gameObject;
            if (objectHit.CompareTag("Enemy"))
            {
                ConfirmHit(objectHit, "left");
            }
        }
    }

    void HitCheckRight()
    {
        RaycastHit2D rightHit = Physics2D.Raycast(
            transform.position,
            transform.right,
            rangeDistance
        );

        if (rightHit.collider != null)
        {
            var objectHit = rightHit.collider.gameObject;
            if (objectHit.CompareTag("Enemy"))
            {
                ConfirmHit(objectHit, "right");
            }
        }
    }

    private void ConfirmHit(GameObject objectHit, string side)
    {
        EventHandler.Event_EnemyHit?.Invoke(objectHit);
        EventHandler.Event_HitEffect?.Invoke(objectHit.transform.position);
        MoveToHitEnemy(objectHit.transform, side);
        CheckDirection(objectHit.transform.position);
    }

    IEnumerator CheckForEnemiesClose(Vector3 targetPosition)
    {
        yield return new WaitForEndOfFrame();

        RaycastHit2D leftHit = Physics2D.Raycast(targetPosition, -transform.right, rangeDistance);
        RaycastHit2D rightHit = Physics2D.Raycast(targetPosition, transform.right, rangeDistance);

        if (leftHit.collider != null || rightHit.collider != null)
        {
            if (leftHit.collider != null && leftHit.collider.gameObject.CompareTag("Enemy"))
            {
                EventHandler.Event_SlowTime?.Invoke();
            }
            else if (rightHit.collider != null && rightHit.collider.gameObject.CompareTag("Enemy"))
            {
                EventHandler.Event_SlowTime?.Invoke();
            }
            else
            {
                EventHandler.Event_StopSlowTime?.Invoke();
            }
        }
        else
        {
            EventHandler.Event_StopSlowTime?.Invoke();
        }
    }

    void MoveToHitEnemy(Transform enemyPosition, string side)
    {
        var targetPosition = enemyPosition.position;

        if (side.Equals("left"))
        {
            targetPosition += new Vector3(moveToEnemyDistance, 0f);
            EventHandler.Event_MoveToEnemy?.Invoke(targetPosition);
        }
        else if (side.Equals("right"))
        {
            targetPosition += new Vector3(-moveToEnemyDistance, 0f);
            EventHandler.Event_MoveToEnemy?.Invoke(targetPosition);
        }

        StartCoroutine(CheckForEnemiesClose(targetPosition));
    }

    void CheckDirection(Vector3 target)
    {
        if (transform.position.x > target.x && _isFacingRight)
            Flip();
        else if (transform.position.x < target.x && !_isFacingRight)
            Flip();
    }

    void Flip()
    {
        _isFacingRight = !_isFacingRight;
        _spriteRenderer.flipX = !_isFacingRight;
    }
}
