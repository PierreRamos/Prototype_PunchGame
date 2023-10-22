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
    float _scrollOffset;

    [SerializeField]
    float _scrollDuration;

    private GameObject _currentEnemy;
    private Dictionary<MoveSet, Sprite> _arrowSprites = new Dictionary<MoveSet, Sprite>();
    private List<MoveSet> _movesToHit = new List<MoveSet>();
    private bool _initialScroll;
    private float _scrollMove;

    private void OnEnable()
    {
        GlobalValues = System_GlobalValues.Instance;
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_TriggeredSoloBattle += (enemy, listOfMoves) =>
        {
            if (!_soloBattlePromptPanel.activeSelf)
                _soloBattlePromptPanel.SetActive(true);

            var initialLocalPosition = _promptListObject.transform.localPosition;
            initialLocalPosition.x = -96.63f;
            _promptListObject.transform.localPosition = initialLocalPosition;

            _currentEnemy = enemy;
            _initialScroll = true;

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

            EventHandler.Event_SetSoloBattleTimer?.Invoke(_movesToHit.Count);
        };
        EventHandler.Event_Hit += (move) =>
        {
            CheckInput(move);
        };
        EventHandler.Event_SoloBattleTimerFinished += (defeatedEnemy) =>
        {
            EvaluateSoloBattle(defeatedEnemy);
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
            EventHandler.Event_CorrectInput?.Invoke(_movesToHit.Count);
            EventHandler.Event_EnemyHitConfirm?.Invoke(_currentEnemy);
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

        //Reset Values
        foreach (Transform prompt in _promptListObject)
        {
            if (prompt.gameObject.activeSelf)
                prompt.gameObject.SetActive(false);
        }

        _movesToHit.Clear();
    }

    private void MovePrompt()
    {
        if (_initialScroll)
        {
            _scrollMove = _promptListObject.transform.localPosition.x - _scrollOffset;
            _initialScroll = false;
        }
        else
            _scrollMove = _scrollMove - _scrollOffset;

        _promptListObject.DOLocalMoveX(_scrollMove, _scrollDuration).SetUpdate(true);
    }
}
