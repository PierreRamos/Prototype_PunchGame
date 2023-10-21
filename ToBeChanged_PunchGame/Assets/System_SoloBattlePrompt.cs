using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class System_SoloBattlePrompt : MonoBehaviour
{
    System_GlobalValues GlobalValues;
    System_EventHandler EventHandler;

    [Header("Init")]
    [SerializeField]
    private GameObject _soloBattlePromptPanel;

    [SerializeField]
    private Transform _promptListObject;

    //Base Arrow
    [SerializeField]
    Sprite _upArrow;

    [SerializeField]
    Sprite _downArrow;

    [SerializeField]
    Sprite _rightArrow;

    [SerializeField]
    Sprite _leftArrow;

    [Header("Solo Battle Prompt Settings")]
    [SerializeField]
    float _scrollMoveValue;

    private GameObject _currentEnemy;
    private Dictionary<MoveSet, Sprite> _arrowSprites = new Dictionary<MoveSet, Sprite>();
    private List<MoveSet> _movesToHit = new List<MoveSet>();
    private float _scrollMoveAmount;

    private void OnEnable()
    {
        GlobalValues = System_GlobalValues.Instance;
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_TriggeredSoloBattle += (enemy, listOfMoves) =>
        {
            _currentEnemy = enemy;

            foreach (var move in listOfMoves)
            {
                foreach (Transform prompt in _promptListObject)
                {
                    if (!prompt.gameObject.activeSelf)
                    {
                        _movesToHit.Add(move);
                        prompt.GetComponent<Image>().sprite = _arrowSprites[move];
                        prompt.gameObject.SetActive(true);
                        break;
                    }
                }
            }

            if (!_soloBattlePromptPanel.activeSelf)
                _soloBattlePromptPanel.SetActive(true);
        };

        EventHandler.Event_Hit += (move) =>
        {
            CheckInput(move);
        };
    }

    private void Awake()
    {
        _arrowSprites[MoveSet.Left] = _leftArrow;
        _arrowSprites[MoveSet.Right] = _rightArrow;
        _arrowSprites[MoveSet.Up] = _upArrow;
        _arrowSprites[MoveSet.Down] = _downArrow;
    }

    private void CheckInput(MoveSet move)
    {
        if (_movesToHit[0] == move)
        {
            MovePrompt();
            _movesToHit.RemoveAt(0);
            EventHandler.Event_CorrectInput();
        }
        else
            EventHandler.Event_IncorrectInput();

        //End solo battle
        if (_movesToHit.Count == 0)
        {
            EvaluateSoloBattle(true);
        }
    }

    //Called at the end of solo battle event
    private void EvaluateSoloBattle(bool defeatedEnemy)
    {
        if (defeatedEnemy)
            EventHandler.Event_DefeatedEnemy?.Invoke(_currentEnemy);
        else
        {
            EventHandler.Event_PlayerHit?.Invoke(1);
            EventHandler.Event_EnemyHitPlayer?.Invoke(_currentEnemy);
        }

        _soloBattlePromptPanel.SetActive(false);
        EventHandler.Event_StopSlowTime?.Invoke();
        EventHandler.Event_StoppedSoloBattle?.Invoke();
        EventHandler.Event_NormalHealthUI?.Invoke();
        GlobalValues.SetGameState(GameState.Normal);

        //Disables all prompts
        foreach (Transform prompt in _promptListObject)
        {
            if (prompt.gameObject.activeSelf)
                prompt.gameObject.SetActive(false);
        }
    }

    private void MovePrompt()
    {
        _scrollMoveAmount += _promptListObject.transform.localPosition.x - _scrollMoveValue; // to fix

        _promptListObject
            .DOLocalMoveX(_promptListObject.transform.localPosition.x - _scrollMoveValue, 0.25f)
            .SetUpdate(true);
    }
}
