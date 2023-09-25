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
    Sprite _baseUpSprite;

    [SerializeField]
    Sprite _baseDownSprite;

    [SerializeField]
    Sprite _baseRightSprite;

    [SerializeField]
    Sprite _baseLeftSprite;

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

    Image _promptImage;

    bool _isWaitingForAction,
        _correctInputRunning,
        _wrongInputRunning;

    GameObject _currentEnemy;

    MoveSet _currentMoveToHit;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_TriggeredSoloBattle += ActivateSoloBattle;
        EventHandler.Event_SoloBattleTimerFinished += DeactivateSoloBattle;
        EventHandler.Event_Hit += CheckMove;
    }

    private void OnDisable()
    {
        EventHandler.Event_TriggeredSoloBattle -= ActivateSoloBattle;
        EventHandler.Event_SoloBattleTimerFinished -= DeactivateSoloBattle;
        EventHandler.Event_Hit -= CheckMove;
    }

    void Start()
    {
        _promptImage = _soloBattlePrompt.GetComponent<Image>();
    }

    void ActivateSoloBattle(GameObject gameObject, List<MoveSet> listOfMoves)
    {
        _hitIndicator.SetActive(false);
        _soloBattlePanel.SetActive(true);

        _currentEnemy = gameObject;

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

    void UpdateBaseSprite(MoveSet move)
    {
        switch (move)
        {
            case MoveSet.Left:
                _promptImage.sprite = _baseLeftSprite;
                break;
            case MoveSet.Right:
                _promptImage.sprite = _baseRightSprite;
                break;
            case MoveSet.Up:
                _promptImage.sprite = _baseUpSprite;
                break;
            case MoveSet.Down:
                _promptImage.sprite = _baseDownSprite;
                break;
        }
    }

    void UpdateCorrectSprite(MoveSet move)
    {
        switch (move)
        {
            case MoveSet.Left:
                _promptImage.sprite = _correctLeftSprite;
                break;
            case MoveSet.Right:
                _promptImage.sprite = _correctRightSprite;
                break;
            case MoveSet.Up:
                _promptImage.sprite = _correctUpSprite;
                break;
            case MoveSet.Down:
                _promptImage.sprite = _correctDownSprite;
                break;
        }
    }

    void UpdateWrongSprite(MoveSet move)
    {
        switch (move)
        {
            case MoveSet.Left:
                _promptImage.sprite = _wrongLeftSprite;
                break;
            case MoveSet.Right:
                _promptImage.sprite = _wrongRightSprite;
                break;
            case MoveSet.Up:
                _promptImage.sprite = _wrongUpSprite;
                break;
            case MoveSet.Down:
                _promptImage.sprite = _wrongDownSprite;
                break;
        }
    }
}
