using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class System_SoloBattle : MonoBehaviour
{
    System_EventHandler EventHandler;

    System_GlobalValues GlobalValues;

    [Header("Initialization")]
    [Space]
    [SerializeField]
    GameObject _playerObject;

    [SerializeField]
    GameObject _hitIndicator;

    [SerializeField]
    GameObject _soloBattlePanel;

    [SerializeField]
    GameObject _soloBattlePrompt;

    [Space]
    [SerializeField]
    List<GameObject> _listOfGhostPrompts = new List<GameObject>();

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
    Image _promptImage;
    bool _isWaitingForAction,
        _correctInputRunning,
        _wrongInputRunning;

    Dictionary<MoveSet, Sprite> baseSprites = new Dictionary<MoveSet, Sprite>();
    Dictionary<MoveSet, Sprite> correctSprites = new Dictionary<MoveSet, Sprite>();
    Dictionary<MoveSet, Sprite> wrongSprites = new Dictionary<MoveSet, Sprite>();
    Dictionary<MoveSet, Sprite> ghostSprites = new Dictionary<MoveSet, Sprite>();

    void Awake()
    {
        baseSprites[MoveSet.Left] = _baseLeftSprite;
        baseSprites[MoveSet.Right] = _baseRightSprite;
        baseSprites[MoveSet.Up] = _baseUpSprite;
        baseSprites[MoveSet.Down] = _baseDownSprite;

        ghostSprites[MoveSet.Left] = _ghostLeftSprite;
        ghostSprites[MoveSet.Right] = _ghostRightSprite;
        ghostSprites[MoveSet.Up] = _ghostUpSprite;
        ghostSprites[MoveSet.Down] = _ghostDownSprite;

        correctSprites[MoveSet.Left] = _correctLeftSprite;
        correctSprites[MoveSet.Right] = _correctRightSprite;
        correctSprites[MoveSet.Up] = _correctUpSprite;
        correctSprites[MoveSet.Down] = _correctDownSprite;

        wrongSprites[MoveSet.Left] = _wrongLeftSprite;
        wrongSprites[MoveSet.Right] = _wrongRightSprite;
        wrongSprites[MoveSet.Up] = _wrongUpSprite;
        wrongSprites[MoveSet.Down] = _wrongDownSprite;
    }

    void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_TriggeredSoloBattle += ActivateSoloBattle;
        EventHandler.Event_SoloBattleTimerFinished += DeactivateSoloBattle;
        EventHandler.Event_Hit += CheckMove;
    }

    void OnDisable()
    {
        EventHandler.Event_TriggeredSoloBattle -= ActivateSoloBattle;
        EventHandler.Event_SoloBattleTimerFinished -= DeactivateSoloBattle;
        EventHandler.Event_Hit -= CheckMove;
    }

    void Start()
    {
        _promptImage = _soloBattlePrompt.GetComponent<Image>();
    }

    void ActivateSoloBattle(GameObject enemy, List<MoveSet> listOfMoves)
    {
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
        EventHandler.Event_DeactivatedSoloBattle?.Invoke(_playerObject.transform.position);

        _currentEnemy.SetActive(false);

        if (defeatedEnemy)
        {
            EventHandler.Event_DefeatedEnemy?.Invoke(_currentEnemy);
        }
        else
            EventHandler.Event_PlayerHit?.Invoke(1);
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

        UpdateCorrectSprite(move);
        EventHandler.Event_EnemyHit?.Invoke(_currentEnemy);

        yield return new WaitForSecondsRealtime(_promptDelayDuration);

        _isWaitingForAction = false;
        _correctInputRunning = false;
    }

    IEnumerator WrongInput(MoveSet move)
    {
        _wrongInputRunning = true;

        UpdateWrongSprite(move);
        EventHandler.Event_SoloBattleWrongInput?.Invoke();

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
        SetSprite(move, baseSprites);
    }

    void UpdateCorrectSprite(MoveSet move)
    {
        SetSprite(move, correctSprites);
    }

    void UpdateWrongSprite(MoveSet move)
    {
        SetSprite(move, wrongSprites);
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
}
