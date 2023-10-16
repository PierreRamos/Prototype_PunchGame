using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EnemyAnimation : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    [Header("Enemy Animation Settings")]
    private float _hitDuration;

    private Animator _enemyAnimator;
    private int _currentState;
    private float _lockedTill;
    private bool _hit;

    private static readonly int Died = Animator.StringToHash("Player_Death");
    private static readonly int Idle = Animator.StringToHash("Enemy_Idle");
    private static readonly int Hit = Animator.StringToHash("Enemy_Hit");
    private static readonly int Attack = Animator.StringToHash("Player_Attack1");

    private void Awake()
    {
        _enemyAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_EnemyHitAnimation += (enemy) =>
        {
            if (enemy == gameObject)
            {
                _hit = true;
            }
        };
    }

    private void Update()
    {
        var state = GetState();

        if (state == _currentState)
            return;

        _hit = false;

        _enemyAnimator.CrossFade(state, 0, 0);
        _currentState = state;
    }

    public void AnimatorTimeScaling(AnimatorUpdateMode value)
    {
        _enemyAnimator.updateMode = value;
    }

    private int GetState()
    {
        if (_hit)
            return LockState(Hit, _hitDuration);

        if (Time.time < _lockedTill)
            return _currentState;

        return Idle;

        int LockState(int state, float time)
        {
            _lockedTill = Time.time + time;
            return state;
        }
    }
}
