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

        EventHandler.Event_EnemyHit += EnemyHitCheck;

        GenerateHits();

        EventHandler.Event_EnemyHitListChange?.Invoke(gameObject, _listOfHits);
    }

    void OnDisable()
    {
        EventHandler.Event_EnemyHit -= EnemyHitCheck;
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

        if (enemyType == EnemyType.easy)
            for (int i = 0; i < _easyEnemyHealth; i++)
            {
                _listOfHits.Add(HitType.normal);
            }
        else if (enemyType == EnemyType.medium)
            for (int i = 0; i < _mediumEnemyHealth; i++)
            {
                _listOfHits.Add(HitType.normal);
            }
        else if (enemyType == EnemyType.hard)
            for (int i = 0; i < _hardEnemyHealth; i++)
            {
                _listOfHits.Add(HitType.normal);
            }
        else if (enemyType == EnemyType.elite)
        {
            for (int i = 0; i < 1; i++)
            {
                _listOfHits.Add(HitType.solo);
            }
            EventHandler.Event_GenerateElite?.Invoke(gameObject);
        }
        //Change this
        else if (enemyType == EnemyType.dash)
        {
            for (int i = 0; i < _hardEnemyHealth; i++)
            {
                _listOfHits.Add(HitType.dash);
            }
        }
    }

    //Checks if this is the enemy hit and removes health
    void EnemyHitCheck(GameObject gameObject)
    {
        if (this.gameObject != gameObject)
            return;

        if (GlobalValues.GetGameState() == GameState.Normal)
        {
            var currentHit = _listOfHits[0];
            _listOfHits.RemoveAt(0);
            EventHandler.Event_EnemyHitListChange?.Invoke(gameObject, _listOfHits);

            if (currentHit == HitType.solo)
            {
                EventHandler.Event_TriggerSoloBattle?.Invoke(gameObject);
                EventHandler.Event_EnemyHitListChange?.Invoke(gameObject, _listOfHits);
                return;
            }
            else if (currentHit == HitType.dash)
            {
                EventHandler.Event_TriggerDash?.Invoke(gameObject);
            }

            if (_listOfHits.Count <= 0)
            {
                EventHandler.Event_DefeatedEnemy?.Invoke(gameObject);
                gameObject.SetActive(false);
                return;
            }
        }
    }
}
