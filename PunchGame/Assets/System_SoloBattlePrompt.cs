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
    Image _promptBackground;

    [Space]
    [SerializeField]
    private GameObject _soloBattlePromptPanel;

    [Space]
    [SerializeField]
    private Transform _promptListObject;

    [Space]
    [SerializeField]
    Sprite _normalBackground;

    [SerializeField]
    Sprite _correctBackground;

    [SerializeField]
    Sprite _incorrectBackground;

    [Space]
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
    float _backgroundChangeDuration;

    [SerializeField]
    float _scrollOffset;

    [SerializeField]
    float _scrollDuration;

    private Coroutine _changeBackgroundSprite;
    private GameObject _currentEnemy;
    private Dictionary<MoveSet, Sprite> _arrowSprites = new Dictionary<MoveSet, Sprite>();
    private List<MoveSet> _movesToHit = new List<MoveSet>();
    private bool _initialScroll;
    private bool _runningChangeBackgroundSprite;
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

            // print("initial invoke");
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
        bool isCorrect;

        if (_movesToHit[0] == move)
        {
            MovePrompt();
            _movesToHit.RemoveAt(0);
            EventHandler.Event_CorrectInput?.Invoke(_movesToHit.Count);
            EventHandler.Event_EnemyHitConfirm?.Invoke(_currentEnemy);
            isCorrect = true;
        }
        else
        {
            EventHandler.Event_IncorrectInput();
            isCorrect = false;
        }

        if (_runningChangeBackgroundSprite)
            StopCoroutine(_changeBackgroundSprite);

        _changeBackgroundSprite = StartCoroutine(ChangeBackgroundSprite(isCorrect));

        //End solo battle
        if (_movesToHit.Count == 0)
        {
            EvaluateSoloBattle(true);
        }

        //
        IEnumerator ChangeBackgroundSprite(bool isCorrect)
        {
            _runningChangeBackgroundSprite = true;
            if (isCorrect)
                _promptBackground.sprite = _correctBackground;
            else
                _promptBackground.sprite = _incorrectBackground;

            yield return new WaitForSecondsRealtime(_backgroundChangeDuration);

            _promptBackground.sprite = _normalBackground;
            _runningChangeBackgroundSprite = false;
        }
    }

    //Called at the end of solo battle event
    private void EvaluateSoloBattle(bool defeatedEnemy)
    {
        if (defeatedEnemy)
            // EventHandler.Event_DefeatedEnemy?.Invoke(_currentEnemy);
            EventHandler.Event_EnemyHitConfirm?.Invoke(_currentEnemy);
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
