using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EnemyHitManager : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    [SerializeField]
    int _maxEnemyHealth;

    [SerializeField]
    List<HitType> _listOfHits = new List<HitType>();

    bool _taggedForHit;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_EnemyTaggedForHit += HitCheck;

        GenerateHits();

        EventHandler.Event_EnemyHitListChange?.Invoke(gameObject, _listOfHits);
        EventHandler.Event_EnemyHitAnimation += ConfirmHitAfterAnimation;
    }

    private void OnDisable()
    {
        EventHandler.Event_EnemyTaggedForHit -= HitCheck;
        EventHandler.Event_EnemyHitAnimation -= ConfirmHitAfterAnimation;
    }

    public bool IsLastHit()
    {
        return _listOfHits.Count == 0;
    }

    //Generates set of hit orb types depending on enemy type
    private void GenerateHits()
    {
        _listOfHits.Clear();

        //Sets enemy health value according to enemy type
        var enemyType = GetComponent<System_EnemyType>().GetEnemyType();
        var enemyHealth = Random.Range(1, _maxEnemyHealth + 1);

        bool hasDash = false;

        //Hit types logic which enemy types can get

        //Normal enemy
        if (enemyType == EnemyType.Normal)
        {
            for (int i = 0; i < enemyHealth; i++)
            {
                _listOfHits.Add(HitType.Normal);
            }
        }
        //Elite enemy
        else if (enemyType == EnemyType.Elite)
        {
            for (int i = 0; i < enemyHealth; i++)
            {
                if (i + 1 == enemyHealth)
                {
                    _listOfHits.Add(HitType.Solo);
                    break;
                }
                _listOfHits.Add(HitType.Normal);
            }
        }
        //Dash enemy
        else if (enemyType == EnemyType.Dash)
        {
            if (enemyHealth < 2)
                enemyHealth++; //Adds health so dash enemies always have dash orbs

            for (int i = 0; i < enemyHealth; i++)
            {
                if (i + 1 == enemyHealth)
                {
                    _listOfHits.Add(HitType.Normal);
                    break;
                }
                RandomizeHits(i);
            }
        }
        //Hold enemy
        else if (enemyType == EnemyType.Hold)
        {
            for (int i = 0; i < enemyHealth; i++)
            {
                if (i + 1 == enemyHealth)
                {
                    _listOfHits.Add(HitType.Hold);
                    break;
                }
                _listOfHits.Add(HitType.Normal);
            }
        }

        //
        void RandomizeHits(int currentIndex)
        {
            if (enemyType == EnemyType.Dash)
            {
                if (enemyHealth - 2 == currentIndex && hasDash == false)
                {
                    _listOfHits.Add(HitType.Dash);
                    return;
                }

                var random = Random.Range(0, 2);
                if (random == 0)
                    _listOfHits.Add(HitType.Normal);
                else
                {
                    _listOfHits.Add(HitType.Dash);
                    hasDash = true;
                }
            }
        }
    }

    private void ConfirmHitAfterAnimation(GameObject enemy)
    {
        if (gameObject != enemy)
            return;

        if (_listOfHits.Count <= 0 && GlobalValues.GetGameState() == GameState.Normal)
        {
            EventHandler.Event_DefeatedEnemy?.Invoke(gameObject);
        }

        _taggedForHit = false;
    }

    //Checks if this is the enemy is tagged for hit and evaluates depending on what type of orb is in the list
    private void HitCheck(GameObject enemy)
    {
        if (gameObject != enemy)
        {
            if (_taggedForHit == true)
                if (_listOfHits.Count <= 0 && GlobalValues.GetGameState() == GameState.Normal)
                {
                    EventHandler.Event_DefeatedEnemy?.Invoke(gameObject);
                }
            return;
        }

        if (_listOfHits.Count <= 0)
            return;

        if (GlobalValues.GetGameState() == GameState.Normal)
        {
            var currentHit = _listOfHits[0];
            _listOfHits.RemoveAt(0);
            EventHandler.Event_EnemyHitListChange?.Invoke(enemy, _listOfHits);

            if (currentHit == HitType.Normal)
            {
                EventHandler.Event_EnemyHitConfirm?.Invoke(enemy);
            }
            else if (currentHit == HitType.Solo)
            {
                EventHandler.Event_TriggerSoloBattle?.Invoke(enemy);
                return; //To avoid deactivating enemy since _listOfHits.Count is now 0
            }
            else if (currentHit == HitType.Dash)
            {
                EventHandler.Event_TriggerDash?.Invoke(enemy);
                EventHandler.Event_EnemyHitConfirm?.Invoke(enemy);
            }
            else if (currentHit == HitType.Hold)
            {
                EventHandler.Event_TriggerHoldBattle?.Invoke(enemy);
                return; //To avoid deactivating enemy since _listOfHits.Count is now 0
            }
        }

        if (_listOfHits.Count <= 0)
        {
            var collider = GetComponent<Collider2D>();
            if (collider.enabled == true)
                collider.enabled = false;
        }

        _taggedForHit = true;
    }
}
