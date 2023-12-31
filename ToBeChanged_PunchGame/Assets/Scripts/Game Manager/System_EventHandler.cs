using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EventHandler : MonoBehaviour
{
    public static System_EventHandler Instance;

    //Game events
    public Action<Vector3> Event_SpawnEnemy;
    public Action<string> Event_PlaySound;

    //Enemy events
    public Action<GameObject> Event_RemoveEnemy; //Override for removing enemies without defeating
    public Action<GameObject> Event_EnemyHitPlayer;
    public Action<GameObject> Event_DefeatedEnemy;
    public Action<GameObject> Event_EnemyDeathAnimation;

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
    public Action<GameObject> Event_EnemyTaggedForHit; //Called from the moment of player input
    public Action<GameObject> Event_EnemyHitConfirm; //Decreased an orb from enemy
    public Action<GameObject> Event_EnemyHitAnimation; //Called when animation finishes
    public Action<Vector3> Event_MoveToEnemy;

    //Potion events
    public Action<int> Event_HealPlayer;

    //Solo Battle events
    public Action<MoveSet> Event_Hit;
    public Action<GameObject> Event_TriggerSoloBattle;
    public Action<GameObject, List<MoveSet>> Event_TriggeredSoloBattle;
    public Action<bool> Event_SoloBattleTimerFinished;
    public Action Event_SoloBattleWrongInput;
    public Action Event_StoppedSoloBattle;
    public Action<int> Event_CorrectInput;
    public Action Event_IncorrectInput;

    //Hold Battle events
    public Action<GameObject> Event_TriggerHoldBattle;
    public Action<GameObject> Event_TriggeredHoldBattle;
    public Action Event_StoppedHoldInput;
    public Action Event_StoppedHoldBattle;

    //Value change events
    public Action<float> Event_PlayerStunTimeChange;
    public Action<int> Event_PlayerHealthValueChange;
    public Action<int> Event_EnemyDefeatedValueChange;
    public Action<int> Event_DifficultyValueChange;

    public Action<GameObject, List<HitType>> Event_EnemyHitListChange;

    //Update on enabled object event
    public Action<int> Event_SetSoloBattleTimer;

    //Effect events
    public Action<Vector3> Event_ExclamationEffect;
    public Action<Vector3> Event_PlayerHealEffect;

    //Hit indicator events
    public Action<int> Event_PlayerHit;
    public Action<bool> Event_HasEnemyLeft;
    public Action<bool> Event_HasEnemyRight;

    //New events
    public Action Event_NormalHealthUI;
    public Action Event_FocusHealthUI;

    //Enemy hit manager events
    public Action<GameObject> Event_TriggerDash;
    public Action<GameObject> Event_EnemyFlip;
    public Action Event_HitStopFinished;

    //Special meter events
    public Action Event_ActivateSpecialInput;
    public Action<bool, float> Event_SpecialActive;
    public Action<float> Event_SpecialMeterValueChange;
    public Action<float> Event_SetSpecialMeterMaxValue;
    public Action Event_MaxedSpecialMeter;
    public Action Event_SpecialCutsceneFinished;

    //Player hit range
    public Action Event_PlayerAttackRangeChange;

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
}
