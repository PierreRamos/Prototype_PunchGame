using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_PlayerAttack : MonoBehaviour
{
    [SerializeField]
    float rangeDistance;

    void Start()
    {
        System_EventHandler.Instance.Event_AttackLeft += HitCheckLeft;
        System_EventHandler.Instance.Event_AttackRight += HitCheckRight;
    }

    private void OnDisable()
    {
        System_EventHandler.Instance.Event_AttackLeft += HitCheckLeft;
        System_EventHandler.Instance.Event_AttackRight += HitCheckRight;
    }

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, -transform.right * rangeDistance, Color.green);
        Debug.DrawRay(transform.position, transform.right * rangeDistance, Color.green);
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
            if (leftHit.collider.gameObject.CompareTag("Enemy"))
            {
                print($"Hit enemy: {leftHit.collider.gameObject}");
                System_EventHandler.Instance.Event_EnemyHit?.Invoke(leftHit.collider.gameObject);
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
            if (rightHit.collider.gameObject.CompareTag("Enemy"))
            {
                print($"Hit enemy: {rightHit.collider.gameObject}");
                System_EventHandler.Instance.Event_EnemyHit?.Invoke(rightHit.collider.gameObject);
            }
        }
    }
}
