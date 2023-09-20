using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_PlayerMovement : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Player Movement Settings")]
    [Space]
    [SerializeField]
    float _attackMoveTime;

    [SerializeField]
    AnimationCurve _attackMoveAnimationCurve;

    Vector3 _initialPosition,
        _targetPosition;
    float _elapsedTime;

    bool _isMovingToPosition;

    void Start()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_MoveToEnemy += MovePlayerToPosition;
    }

    void OnDisable()
    {
        EventHandler.Event_MoveToEnemy -= MovePlayerToPosition;
    }

    void Update()
    {
        LerpToPosition();
    }

    void LerpToPosition()
    {
        if (_isMovingToPosition)
        {
            _elapsedTime += Time.unscaledDeltaTime;

            if (_elapsedTime < _attackMoveTime)
            {
                // Interpolate between initial position and target position
                float time = _attackMoveAnimationCurve.Evaluate(_elapsedTime / _attackMoveTime);
                transform.position = Vector3.Lerp(_initialPosition, _targetPosition, time);
            }
            else
            {
                // Ensure we reach the target exactly
                transform.position = _targetPosition;
                _isMovingToPosition = false;
            }
        }
    }

    void MovePlayerToPosition(Vector3 target)
    {
        _initialPosition = transform.position;
        _elapsedTime = 0;
        _targetPosition = target;
        _isMovingToPosition = true;
    }
}
