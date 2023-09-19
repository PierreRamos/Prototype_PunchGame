using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class System_EnemyController : MonoBehaviour
{
    [SerializeField]
    float _movementSpeed;

    Transform _playerTransform;

    void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            _playerTransform.position,
            _movementSpeed
        );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
