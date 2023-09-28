using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_DashMechanics : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Dash Settings")]
    [Space]
    [SerializeField]
    float _dashDistance;

    [SerializeField]
    float _attackMoveTime;

    [SerializeField]
    AnimationCurve _attackMoveAnimationCurve;

    Collider2D _collider;

    bool _isMovingToPosition;

    float _elapsedTime;

    Vector3 _initialPosition,
        _targetPosition;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_TriggerDash += TriggerDash;
    }

    private void OnDisable()
    {
        EventHandler.Event_TriggerDash -= TriggerDash;
        ResetValues();
    }

    private void Update()
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
                EnableCollider(true);
                transform.position = _targetPosition;
                _isMovingToPosition = false;
            }
        }
    }

    void TriggerDash(GameObject gameObject)
    {
        if (this.gameObject != gameObject)
            return;

        EnableCollider(false);
        var enemyController = GetComponent<System_EnemyController>();
        var player = enemyController.GetPlayerPosition();
        bool isFacingRight = enemyController.GetIsFacingRight();

        var initialDistance = Vector3.Distance(transform.position, player.transform.position);

        if (initialDistance > 0.75f)
            initialDistance = 0.75f;

        var xDistance = initialDistance + _dashDistance;

        if (isFacingRight)
        {
            MoveToPosition(new Vector2(xDistance + transform.position.x, transform.position.y));
        }
        else
        {
            MoveToPosition(new Vector2(-xDistance + transform.position.x, transform.position.y));
        }

        EventHandler.Event_EnemyFlip?.Invoke(gameObject);
    }

    void MoveToPosition(Vector3 target)
    {
        _initialPosition = transform.position;
        _elapsedTime = 0;
        _targetPosition = target;
        _isMovingToPosition = true;
    }

    void EnableCollider(bool value)
    {
        var collider = GetComponent<Collider2D>();
        collider.enabled = value;
    }

    void ResetValues()
    {
        _isMovingToPosition = false;
        _elapsedTime = 0;

        if (!_collider.enabled)
            _collider.enabled = true;
    }
}
