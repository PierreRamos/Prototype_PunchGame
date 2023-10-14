using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_PlayerAnimation : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Init")]
    [SerializeField]
    private Animator _playerAnimator;

    [Header("Player Animation Settings")]
    [SerializeField]
    private float _punchDuration;

    [SerializeField]
    private float _hitDuration;

    private GameObject _currentEnemy;
    private int _currentState;
    private int _attackVariant;
    private float _lockedTill;
    private bool _attacking;
    private bool _battling; //Hold battle | Solo battle
    private bool _hit;
    private bool _stunned;
    private bool _died;
    private bool _hitStopped;
    private bool _usedRight;
    private List<int> _leftAttacks = new List<int>();
    private List<int> _rightAttacks = new List<int>();

    private static readonly int Died = Animator.StringToHash("Player_Death");
    private static readonly int Idle = Animator.StringToHash("Player_Idle");
    private static readonly int BattleIdle = Animator.StringToHash("Player_BattleIdle");
    private static readonly int Hit = Animator.StringToHash("Player_Hit");
    private static readonly int Missed = Animator.StringToHash("Player_Missed");
    private static readonly int Attack1 = Animator.StringToHash("Player_Attack1");
    private static readonly int Attack2 = Animator.StringToHash("Player_Attack2");
    private static readonly int Attack3 = Animator.StringToHash("Player_Attack3");
    private static readonly int Attack4 = Animator.StringToHash("Player_Attack4");

    private void Awake()
    {
        _leftAttacks.Add(Attack2);
        _leftAttacks.Add(Attack3);
        _rightAttacks.Add(Attack1);
        _rightAttacks.Add(Attack4);
    }

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        //Attacking
        EventHandler.Event_EnemyHitConfirm += (enemy) =>
        {
            _currentEnemy = enemy;
            _attacking = true;
        };

        //Battling
        EventHandler.Event_TriggeredHoldBattle += (dummy) =>
        {
            _battling = true;
        };
        EventHandler.Event_TriggeredSoloBattle += (dummy, dummy2) =>
        {
            _battling = true;
        };
        EventHandler.Event_StoppedHoldBattle += () =>
        {
            _battling = false;
        };
        EventHandler.Event_StoppedSoloBattle += () =>
        {
            _battling = false;
        };

        //Hit
        EventHandler.Event_PlayerHit += (dummy) =>
        {
            _hit = true;
        };

        //Missed
        EventHandler.Event_TriggerStun += () =>
        {
            _stunned = true;
        };
        EventHandler.Event_PlayerStunFinished += () =>
        {
            _stunned = false;
        };

        //Defeat
        EventHandler.Event_PlayerDied += () =>
        {
            _died = true;
        };

        //Hitstop
        EventHandler.Event_HitStopFinished += () =>
        {
            _hitStopped = false;
            AnimatorTimeScaling(AnimatorUpdateMode.UnscaledTime);
        };
    }

    private void Update()
    {
        var state = GetState();

        _attacking = false;
        _hit = false;

        if (state == Idle && _hitStopped == true)
            return;
        else if (state == _currentState)
            return;

        _playerAnimator.CrossFade(state, 0, 0);
        _currentState = state;
    }

    //Called through animation
    public void ConfirmHit()
    {
        _hitStopped = true;
        AnimatorTimeScaling(AnimatorUpdateMode.Normal);
        EventHandler.Event_EnemyHitAnimation?.Invoke(_currentEnemy);
    }

    public void AnimatorTimeScaling(AnimatorUpdateMode value)
    {
        _playerAnimator.updateMode = value;
    }

    private int GetState()
    {
        if (_died)
            return Died;

        if (_hit)
            return LockState(Hit, _hitDuration);

        if (_stunned)
            return Missed;

        if (_attacking)
        {
            int attackIndex = _currentEnemy.GetComponent<System_EnemyHitManager>().IsLastHit()
              ? 1
              : 0;
            int attackAnimation = _usedRight
                ? _leftAttacks[attackIndex]
                : _rightAttacks[attackIndex];

            _usedRight = !_usedRight;

            return LockState(attackAnimation, _punchDuration);
        }

        if (Time.time < _lockedTill)
            return _currentState;

        if (_battling)
            return BattleIdle;

        return Idle;

        int LockState(int state, float time)
        {
            _lockedTill = Time.time + time;
            return state;
        }
        // int LockStateScaled(int state, float time)
        // {
        //     _lockedTill = Time.time + time;
        //     return state;
        // }
    }
}
