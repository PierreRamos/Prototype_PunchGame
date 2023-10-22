using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class System_SoloBattle : MonoBehaviour
{
    System_EventHandler EventHandler;

    System_GlobalValues GlobalValues;

    [Header("Initialization")]
    [SerializeField]
    GameObject _hitIndicator;

    [SerializeField]
    GameObject _soloBattlePanel;

    [SerializeField]
    GameObject _soloBattlePrompt;

    [SerializeField]
    GameObject _transitionArrow;

    [Space]
    [SerializeField]
    List<GameObject> _listOfGhostPrompts = new List<GameObject>();

    //Base Arrow
    [SerializeField]
    Sprite _upArrow;

    [SerializeField]
    Sprite _downArrow;

    [SerializeField]
    Sprite _rightArrow;

    [SerializeField]
    Sprite _leftArrow;

    [Space]
    [SerializeField]
    Sprite _baseUpSprite;

    [SerializeField]
    Sprite _baseDownSprite;

    [SerializeField]
    Sprite _baseRightSprite;

    [SerializeField]
    Sprite _baseLeftSprite;

    [Space]
    [SerializeField]
    Sprite _ghostUpSprite;

    [SerializeField]
    Sprite _ghostDownSprite;

    [SerializeField]
    Sprite _ghostRightSprite;

    [SerializeField]
    Sprite _ghostLeftSprite;

    [Space]
    [SerializeField]
    Sprite _correctUpSprite;

    [SerializeField]
    Sprite _correctDownSprite;

    [SerializeField]
    Sprite _correctRightSprite;

    [SerializeField]
    Sprite _correctLeftSprite;

    [Space]
    [SerializeField]
    Sprite _wrongUpSprite;

    [SerializeField]
    Sprite _wrongDownSprite;

    [SerializeField]
    Sprite _wrongRightSprite;

    [SerializeField]
    Sprite _wrongLeftSprite;

    [Header("Solo Battle Settings")]
    [Space]
    [SerializeField]
    float _promptDelayDuration;
    MoveSet _currentMoveToHit;
    GameObject _currentEnemy;
    Transform _ghost1,
        _ghost2,
        _ghost3;

    Vector3 _initialTransitionArrowPosition;
    Image _promptImage,
        _transitionImage;
    bool _isWaitingForAction,
        _correctInputRunning,
        _wrongInputRunning;

    Dictionary<MoveSet, Sprite> _arrowSprites = new Dictionary<MoveSet, Sprite>();
    Dictionary<MoveSet, Sprite> _baseSprites = new Dictionary<MoveSet, Sprite>();
    Dictionary<MoveSet, Sprite> _correctSprites = new Dictionary<MoveSet, Sprite>();
    Dictionary<MoveSet, Sprite> _wrongSprites = new Dictionary<MoveSet, Sprite>();
    Dictionary<MoveSet, Sprite> _ghostSprites = new Dictionary<MoveSet, Sprite>();

    void Awake()
    {
        _arrowSprites[MoveSet.Left] = _leftArrow;
        _arrowSprites[MoveSet.Right] = _rightArrow;
        _arrowSprites[MoveSet.Up] = _upArrow;
        _arrowSprites[MoveSet.Down] = _downArrow;

        _baseSprites[MoveSet.Left] = _baseLeftSprite;
        _baseSprites[MoveSet.Right] = _baseRightSprite;
        _baseSprites[MoveSet.Up] = _baseUpSprite;
        _baseSprites[MoveSet.Down] = _baseDownSprite;

        _ghostSprites[MoveSet.Left] = _ghostLeftSprite;
        _ghostSprites[MoveSet.Right] = _ghostRightSprite;
        _ghostSprites[MoveSet.Up] = _ghostUpSprite;
        _ghostSprites[MoveSet.Down] = _ghostDownSprite;

        _correctSprites[MoveSet.Left] = _correctLeftSprite;
        _correctSprites[MoveSet.Right] = _correctRightSprite;
        _correctSprites[MoveSet.Up] = _correctUpSprite;
        _correctSprites[MoveSet.Down] = _correctDownSprite;

        _wrongSprites[MoveSet.Left] = _wrongLeftSprite;
        _wrongSprites[MoveSet.Right] = _wrongRightSprite;
        _wrongSprites[MoveSet.Up] = _wrongUpSprite;
        _wrongSprites[MoveSet.Down] = _wrongDownSprite;
    }

    void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        // EventHandler.Event_TriggeredSoloBattle += ActivateSoloBattle;
        // EventHandler.Event_SoloBattleTimerFinished += DeactivateSoloBattle;
        EventHandler.Event_Hit += CheckMove;
    }

    void OnDisable()
    {
        // EventHandler.Event_TriggeredSoloBattle -= ActivateSoloBattle;
        // EventHandler.Event_SoloBattleTimerFinished -= DeactivateSoloBattle;
        EventHandler.Event_Hit -= CheckMove;

        StopAllCoroutines();
        ResetBool();
    }

    void Start()
    {
        _promptImage = _soloBattlePrompt.GetComponent<Image>();
        _transitionImage = _transitionArrow.GetComponent<Image>();

        _initialTransitionArrowPosition = _transitionArrow.transform.localPosition;
    }

    private void ActivateSoloBattle(GameObject enemy, List<MoveSet> listOfMoves)
    {
        GlobalValues.SetMovesToHitCount(listOfMoves.Count);
        _currentEnemy = enemy;

        _hitIndicator.SetActive(false);
        _soloBattlePanel.SetActive(true);

        //Passes enemy move count to solo battle timer
        var moveCount = enemy.GetComponent<System_EliteMechanics>().GetMoveCount();
        EventHandler.Event_UpdateSoloBattleTimer?.Invoke(moveCount);

        StartCoroutine(PerformMoves(listOfMoves));
    }

    IEnumerator PerformMoves(List<MoveSet> movesQueue)
    {
        var numberOfLoops = 0;
        while (movesQueue.Count > 0)
        {
            numberOfLoops++;

            _currentMoveToHit = movesQueue[0];
            movesQueue.RemoveAt(0);
            GlobalValues.SetMovesToHitCount(movesQueue.Count);

            UpdateGhostPrompts(movesQueue);

            //Wait here until last sprite animation played?
            UpdateBaseSprite(_currentMoveToHit);

            _isWaitingForAction = true;

            yield return new WaitUntil(() => !_isWaitingForAction);
        }

        DeactivateSoloBattle(true);
    }

    void DeactivateSoloBattle(bool defeatedEnemy)
    {
        _hitIndicator.SetActive(true);
        _soloBattlePanel.SetActive(false);

        GlobalValues.SetGameState(GameState.Normal);

        EventHandler.Event_StopSlowTime?.Invoke();
        EventHandler.Event_StoppedSoloBattle?.Invoke();
        EventHandler.Event_NormalHealthUI?.Invoke();

        if (defeatedEnemy)
        {
            EventHandler.Event_DefeatedEnemy?.Invoke(_currentEnemy);
        }
        else
        {
            EventHandler.Event_PlayerHit?.Invoke(1);
            EventHandler.Event_EnemyHitPlayer?.Invoke(_currentEnemy);
        }

        // _currentEnemy.SetActive(false);
    }

    void CheckMove(MoveSet move)
    {
        if (move == _currentMoveToHit && !_correctInputRunning)
        {
            StartCoroutine(CorrectInput(move));
        }
        else if (!_wrongInputRunning)
            StartCoroutine(WrongInput(_currentMoveToHit));
    }

    IEnumerator CorrectInput(MoveSet move)
    {
        _correctInputRunning = true;

        // TransitionArrowPrompt(move);

        UpdateCorrectSprite(move);
        EventHandler.Event_EnemyHitConfirm?.Invoke(_currentEnemy);

        if (GlobalValues.GetMovesToHitCount() > 0)
            // EventHandler.Event_CorrectInput?.Invoke();

            yield return new WaitForSecondsRealtime(_promptDelayDuration);

        _isWaitingForAction = false;
        _correctInputRunning = false;
    }

    private void TransitionArrowPrompt(MoveSet move)
    {
        _transitionArrow.transform.localPosition = _initialTransitionArrowPosition;

        _transitionImage.sprite = _arrowSprites[move];

        if (_transitionArrow.activeSelf == false)
            _transitionArrow.SetActive(true);

        _transitionArrow.transform.DOMoveX(_promptImage.transform.position.x, .1f).onComplete +=
            () =>
            {
                _transitionArrow.SetActive(false);
            };
    }

    IEnumerator WrongInput(MoveSet move)
    {
        _wrongInputRunning = true;

        UpdateWrongSprite(move);
        EventHandler.Event_SoloBattleWrongInput?.Invoke();
        EventHandler.Event_IncorrectInput?.Invoke();

        yield return new WaitForSecondsRealtime(_promptDelayDuration);
        UpdateBaseSprite(move);

        _wrongInputRunning = false;
    }

    void UpdateGhostPrompts(List<MoveSet> moves)
    {
        //Initially disables every prompt
        foreach (var prompt in _listOfGhostPrompts)
        {
            if (prompt.activeSelf)
                prompt.SetActive(false);
        }

        if (moves.Count > 0)
        {
            for (int i = 0; i < _listOfGhostPrompts.Count; i++)
            {
                //Breaks the for loop early if moves.Count is lesser than _listOfGhostPrompts.Count
                if (i + 1 > moves.Count)
                    break;

                var ghostImage = _listOfGhostPrompts[i].GetComponent<Image>();

                if (moves[i] == MoveSet.Left)
                    ghostImage.sprite = _ghostLeftSprite;
                else if (moves[i] == MoveSet.Right)
                    ghostImage.sprite = _ghostRightSprite;
                else if (moves[i] == MoveSet.Up)
                    ghostImage.sprite = _ghostUpSprite;
                else if (moves[i] == MoveSet.Down)
                    ghostImage.sprite = _ghostDownSprite;

                if (!_listOfGhostPrompts[i].activeSelf)
                    _listOfGhostPrompts[i].SetActive(true);
            }
        }
    }

    void UpdateBaseSprite(MoveSet move)
    {
        SetSprite(move, _baseSprites);
    }

    void UpdateCorrectSprite(MoveSet move)
    {
        SetSprite(move, _correctSprites);
    }

    void UpdateWrongSprite(MoveSet move)
    {
        SetSprite(move, _wrongSprites);
    }

    void SetSprite(MoveSet move, Dictionary<MoveSet, Sprite> spriteDictionary)
    {
        if (spriteDictionary.ContainsKey(move))
        {
            _promptImage.sprite = spriteDictionary[move];
        }
        else
        {
            // Handle the case where the move is not found in the dictionary
            Debug.LogWarning("Sprite not found for move: " + move);
        }
    }

    void ResetBool()
    {
        _isWaitingForAction = false;
        _correctInputRunning = false;
        _wrongInputRunning = false;
    }
}
