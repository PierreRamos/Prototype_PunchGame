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

    public void ConfirmHit()
    {
        EventHandler.Event_EnemyHitAnimation?.Invoke(_currentEnemy);
    }

    private int GetState()
    {
        if (_died)
            return Died;

        if (_hit)
            return LockStateUnscaled(Hit, _hitDuration);

        if (_stunned)
            return Missed;

        if (_attacking)
        {
            _attackVariant = Random.Range(0, 2);

            if (_usedRight)
            {
                _usedRight = !_usedRight;
                return LockStateUnscaled(_leftAttacks[_attackVariant], _punchDuration);
            }
            else
            {
                _usedRight = !_usedRight;
                return LockStateUnscaled(_rightAttacks[_attackVariant], _punchDuration);
            }
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
