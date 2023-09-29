using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EnemyHitManager : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    [SerializeField]
    int _enemyHitCount;

    [SerializeField]
    int _easyEnemyHealth;

    [SerializeField]
    int _mediumEnemyHealth;

    [SerializeField]
    int _hardEnemyHealth;

    [SerializeField]
    List<HitType> _listOfHits = new List<HitType>();

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_EnemyHit += HitCheck;

        GenerateHits();

        EventHandler.Event_EnemyHitListChange?.Invoke(gameObject, _listOfHits);
    }

    void OnDisable()
    {
        EventHandler.Event_EnemyHit -= HitCheck;
    }

    public int GetEnemyHealth()
    {
        return _enemyHitCount;
    }

    //Generates set of hit orb types depending on enemy type
    void GenerateHits()
    {
        _listOfHits.Clear();

        //Sets enemy health value according to enemy type
        var enemyType = GetComponent<System_EnemyType>().GetEnemyType();

        if (enemyType == EnemyType.Easy)
            for (int i = 0; i < _easyEnemyHealth; i++)
            {
                _listOfHits.Add(HitType.Normal);
            }
        else if (enemyType == EnemyType.Medium)
            for (int i = 0; i < _mediumEnemyHealth; i++)
            {
                _listOfHits.Add(HitType.Normal);
            }
        else if (enemyType == EnemyType.Hard)
            for (int i = 0; i < _hardEnemyHealth; i++)
            {
                _listOfHits.Add(HitType.Normal);
            }
        //Change this
        else if (enemyType == EnemyType.Elite)
        {
            for (int i = 0; i < 1; i++)
            {
                _listOfHits.Add(HitType.Solo);
            }
            EventHandler.Event_GenerateElite?.Invoke(gameObject);
        }
        else if (enemyType == EnemyType.Dash)
        {
            for (int i = 0; i < _hardEnemyHealth; i++)
            {
                if (i + 1 == _hardEnemyHealth)
                {
                    _listOfHits.Add(HitType.Normal);
                    return;
                }
                RandomizeHits(enemyType);
            }
        }
        else if (enemyType == EnemyType.Hold)
        {
            for (int i = 0; i < 1; i++)
            {
                _listOfHits.Add(HitType.Hold);
            }
        }
    }

    void RandomizeHits(EnemyType enemyType)
    {
        if (enemyType == EnemyType.Dash)
        {
            var random = Random.Range(0, 2);
            if (random == 0)
                _listOfHits.Add(HitType.Normal);
            else
                _listOfHits.Add(HitType.Dash);
        }
    }

    //Checks if this is the enemy hit and evaluates depending on what type of orb is in the list
    void HitCheck(GameObject gameObject)
    {
        if (this.gameObject != gameObject)
            return;

        if (GlobalValues.GetGameState() == GameState.Normal)
        {
            var currentHit = _listOfHits[0];
            _listOfHits.RemoveAt(0);
            EventHandler.Event_EnemyHitListChange?.Invoke(gameObject, _listOfHits);

            if (currentHit == HitType.Solo)
            {
                EventHandler.Event_TriggerSoloBattle?.Invoke(gameObject);
                return; //To avoid deactivating enemy since _listOfHits.Count is now 0
            }
            else if (currentHit == HitType.Dash)
            {
                EventHandler.Event_TriggerDash?.Invoke(gameObject);
            }
            else if (currentHit == HitType.Hold)
            {
                EventHandler.Event_TriggerHoldBattle?.Invoke(gameObject);
                return; //To avoid deactivating enemy since _listOfHits.Count is now 0
            }

            if (_listOfHits.Count <= 0)
            {
                EventHandler.Event_DefeatedEnemy?.Invoke(gameObject);
                return;
            }
        }
    }
}
