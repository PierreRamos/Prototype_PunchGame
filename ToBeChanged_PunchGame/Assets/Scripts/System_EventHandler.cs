using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EventHandler : MonoBehaviour
{
    public static System_EventHandler Instance;

    //Game events
    public Action<Vector3> Event_SpawnEnemy;
    public Action<Vector3> Event_DeactivatedSoloBattle;
    public Action<GameObject> Event_DefeatedEnemy;

    //Player controller events
    public Action Event_AttackLeft;
    public Action Event_AttackRight;
    public Action<bool> Event_Pause;

    //Player Status events
    public Action Event_TriggerStun;
    public Action Event_PlayerDied;
    public Action Event_PlayerStunFinished;

    //Attack events
    public Action Event_SlowTime;
    public Action Event_StopSlowTime;
    public Action<GameObject> Event_EnemyHit;
    public Action<Vector3> Event_MoveToEnemy;
    public Action<GameObject> Event_TriggerSoloBattle;
    public Action<GameObject, List<MoveSet>> Event_TriggeredSoloBattle;

    //Potion events
    public Action<int> Event_HealPlayer;

    //Solo Battle events
    public Action<MoveSet> Event_Hit;
    public Action<bool> Event_SoloBattleTimerFinished;
    public Action Event_SoloBattleWrongInput;

    //Value change events

    public Action<float> Event_PlayerStunTimeChange;
    public Action<int> Event_PlayerHealthValueChange;
    public Action<int> Event_EnemyDefeatedValueChange;
    public Action<int> Event_DifficultyValueChange;
    public Action<GameObject, List<HitType>> Event_EnemyHitListChange;

    //Update on enabled object event
    public Action<int> Event_UpdateSoloBattleTimer;

    //Effect events
    public Action<Vector3> Event_ExclamationEffect;

    //Hit indicator events
    public Action<int> Event_PlayerHit;
    public Action<bool> Event_HasEnemyLeft;
    public Action<bool> Event_HasEnemyRight;

    //New events
    public Action Event_ChangeHealthUI;

    //Enemy hit manager events
    public Action<GameObject> Event_GenerateElite;

    public Action<GameObject> Event_TriggerDash;

    public Action<GameObject> Event_EnemyFlip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        Event_TriggeredSoloBattle += TriggerChangeHealthUI;
        Event_DeactivatedSoloBattle += TriggerChangeHealthUI;
    }

    private void OnDisable()
    {
        Event_TriggeredSoloBattle -= TriggerChangeHealthUI;
        Event_DeactivatedSoloBattle -= TriggerChangeHealthUI;
    }

    private void TriggerChangeHealthUI(Vector3 dummy)
    {
        Event_ChangeHealthUI?.Invoke();
    }

    void TriggerChangeHealthUI(GameObject dummy, List<MoveSet> dummy2)
    {
        Event_ChangeHealthUI?.Invoke();
    }
}
