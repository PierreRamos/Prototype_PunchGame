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
    private int _animationVariant;
    private float _lockedTill;
    private bool _attacking;
    private bool _battling; //Hold battle | Solo battle
    private bool _hit;

    private static readonly int Idle = Animator.StringToHash("Player_Idle");
    private static readonly int BattleIdle = Animator.StringToHash("Player_BattleIdle");
    private static readonly int Hit = Animator.StringToHash("Player_Hit");
    private static readonly int Punch1 = Animator.StringToHash("Player_Punch1");
    private static readonly int Punch2 = Animator.StringToHash("Player_Punch2");

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
            return LockState(Hit, _hitDuration);

        if (_attacking)
        {
            if (_animationVariant == 0)
            {
                _animationVariant++;
                return LockState(Punch1, _punchDuration);
            }
            else
            {
                _animationVariant = 0;
                return LockState(Punch2, _punchDuration);
            }
        }

        if (Time.unscaledTime < _lockedTill)
            return _currentState;

        if (_battling)
            return BattleIdle;

        return Idle;

        int LockState(int state, float time)
        {
            _lockedTill = Time.unscaledTime + time;
            return state;
        }
    }
}
