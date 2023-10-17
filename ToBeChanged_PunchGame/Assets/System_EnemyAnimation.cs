using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EnemyAnimation : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    [Header("Enemy Animation Settings")]
    [SerializeField]
    private float _hitDuration;

    private Animator _enemyAnimator;
    private int _currentState;
    private float _lockedTill;
    private bool _hit;
    private bool _idle;
    private bool _triggeredHit;

    private static readonly int Died = Animator.StringToHash("Player_Death");
    private static readonly int Idle = Animator.StringToHash("Enemy_Idle");
    private static readonly int Walk = Animator.StringToHash("Enemy_Walk");
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
                _triggeredHit = true;
            }
        };

        EventHandler.Event_TriggerSoloBattle += (enemy) =>
        {
            if (enemy == gameObject)
                _idle = true;
        };
        EventHandler.Event_TriggeredHoldBattle += (enemy) =>
        {
            if (enemy == gameObject)
                _idle = true;
        };
    }

    private void OnDisable()
    {
        _idle = false;
        _hit = false;
    }

    private void Update()
    {
        var state = GetState();

        _hit = false;

        if (state == _currentState && state != Hit)
            return;
        if (state == Hit && _triggeredHit == false)
            return;

        _triggeredHit = false;

        _enemyAnimator.CrossFadeInFixedTime(state, 0, 0);

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

        if (_idle)
            return Idle;

        return Walk;

        int LockState(int state, float time)
        {
            _lockedTill = Time.time + time;
            return state;
        }
    }
}
