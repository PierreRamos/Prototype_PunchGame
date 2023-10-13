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

    private int _currentState;
    private int _attackVariant;
    private int _previousAttackVariant;
    private float _lockedTill;
    private bool _attacking;
    private bool _battling; //Hold battle | Solo battle
    private bool _hit;
    private bool _stunned;
    private List<int> _listOfAttacks = new List<int>();

    private static readonly int Idle = Animator.StringToHash("Player_Idle");
    private static readonly int BattleIdle = Animator.StringToHash("Player_BattleIdle");
    private static readonly int Hit = Animator.StringToHash("Player_Hit");
    private static readonly int Missed = Animator.StringToHash("Player_Missed");
    private static readonly int Attack1 = Animator.StringToHash("Player_Punch1");
    private static readonly int Attack2 = Animator.StringToHash("Player_Punch2");
    private static readonly int Attack3 = Animator.StringToHash("Player_Attack3");
    private static readonly int Attack4 = Animator.StringToHash("Player_Attack4");

    private void Awake()
    {
        _listOfAttacks.Add(Attack1);
        _listOfAttacks.Add(Attack2);
        _listOfAttacks.Add(Attack3);
        _listOfAttacks.Add(Attack4);
    }

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        //Attacking
        EventHandler.Event_EnemyHitConfirm += (dummy) =>
        {
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
    }

    private void Update()
    {
        var state = GetState();

        _attacking = false;
        _hit = false;

        if (state == _currentState)
            return;
        _playerAnimator.CrossFade(state, 0, 0);
        _currentState = state;
    }

    int GetState()
    {
        if (_hit)
            return LockStateUnscaled(Hit, _hitDuration);

        if (_stunned)
            return Missed;

        if (_attacking)
        {
            while (_attackVariant == _previousAttackVariant)
            {
                _attackVariant = Random.Range(0, 4);
            }

            _previousAttackVariant = _attackVariant;
            return LockStateUnscaled(_listOfAttacks[_attackVariant], _punchDuration);
        }

        if (Time.unscaledTime < _lockedTill)
            return _currentState;

        if (_battling)
            return BattleIdle;

        return Idle;

        int LockStateUnscaled(int state, float time)
        {
            _lockedTill = Time.unscaledTime + time;
            return state;
        }
        // int LockStateScaled(int state, float time)
        // {
        //     _lockedTill = Time.time + time;
        //     return state;
        // }
    }
}
