using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EnemySpawnLocation : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    Transform _leftEnemySpawnPoint;

    [SerializeField]
    Transform _rightEnemySpawnPoint;

    [Header("Enemy Spawn Point Settings")]
    [SerializeField]
    float _spawnPointOffset;

    Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        float leftCameraEdge = _camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        float rightCameraEdge = _camera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x;

        Vector3 leftSpawnPosition = _leftEnemySpawnPoint.position;
        leftSpawnPosition.x = leftCameraEdge - _spawnPointOffset;
        _leftEnemySpawnPoint.position = leftSpawnPosition;

        Vector3 rightSpawnPosition = _rightEnemySpawnPoint.position;
        rightSpawnPosition.x = rightCameraEdge + _spawnPointOffset;
        _rightEnemySpawnPoint.position = rightSpawnPosition;
    }
}
