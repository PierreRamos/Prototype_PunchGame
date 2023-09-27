using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EliteMechanics : MonoBehaviour
{
    System_EventHandler EventHandler;

    System_GlobalValues GlobalValues;

    [SerializeField]
    int _minNumberOfMoves;

    [SerializeField]
    int _maxNumberOfMoves;

    [SerializeField]
    List<MoveSet> _listOfMoves = new List<MoveSet>();

    void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_EnemyHit += CheckIfHit;

        GenerateMoveSet();
    }

    void OnDisable()
    {
        EventHandler.Event_EnemyHit -= CheckIfHit;

        RemoveMoveSet();
    }

    public int GetMoveCount()
    {
        return _listOfMoves.Count;
    }

    void CheckIfHit(GameObject gameObject)
    {
        if (this.gameObject == gameObject && GlobalValues.GetGameState() == GameState.Normal)
        {
            StartCoroutine(TriggerSoloBattle());
        }
    }

    IEnumerator TriggerSoloBattle()
    {
        yield return new WaitForEndOfFrame();

        GlobalValues.SetGameState(GameState.SoloBattle);
        EventHandler.Event_TriggeredSoloBattle?.Invoke(gameObject, _listOfMoves);
    }

    void GenerateMoveSet()
    {
        var numberOfMoves = Random.Range(_minNumberOfMoves, _maxNumberOfMoves + 1);

        for (int i = 0; i < numberOfMoves; i++)
        {
            var random = Random.Range(0, 4);

            MoveSet move = (MoveSet)random;
            _listOfMoves.Add(move);
        }
    }

    void RemoveMoveSet()
    {
        _listOfMoves.Clear();
    }
}
