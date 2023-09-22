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
    GameObject _hitIndicator;

    [SerializeField]
    GameObject _soloBattlePanel;

    [SerializeField]
    Sprite _upSprite;

    [SerializeField]
    Sprite _downSprite;

    [SerializeField]
    Sprite _rightSprite;

    [SerializeField]
    Sprite _leftSprite;

    Camera _mainCamera;

    Image _soloBattleImage;

    bool _isWaitingForAction;

    GameObject _currentEnemy;

    MoveSet _currentMoveToHit;

    Coroutine _performMoves;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_TriggeredSoloBattle += ActivateSoloBattle;
        EventHandler.Event_Hit += CheckMove;
    }

    private void OnDisable()
    {
        EventHandler.Event_TriggeredSoloBattle += ActivateSoloBattle;
        EventHandler.Event_Hit += CheckMove;
    }

    void Start()
    {
        _soloBattleImage = _soloBattlePanel.GetComponent<Image>();
    }

    void ActivateSoloBattle(GameObject gameObject, List<MoveSet> listOfMoves)
    {
        _hitIndicator.SetActive(false);
        _soloBattlePanel.SetActive(true);

        _currentEnemy = gameObject;

        // if (_performMoves == null)
        _performMoves = StartCoroutine(PerformMoves(listOfMoves));
    }

    IEnumerator PerformMoves(List<MoveSet> movesQueue)
    {
        var numberOfLoops = 0;
        while (movesQueue.Count > 0)
        {
            numberOfLoops++;

            _currentMoveToHit = movesQueue[0];
            movesQueue.RemoveAt(0);

            UpdateSprite(_currentMoveToHit);

            _isWaitingForAction = true;

            yield return new WaitUntil(() => !_isWaitingForAction);
        }

        DeactivateSoloBattle(true);
    }

    void UpdateSprite(MoveSet move)
    {
        switch (move)
        {
            case MoveSet.Left:
                _soloBattleImage.sprite = _leftSprite;
                break;
            case MoveSet.Right:
                _soloBattleImage.sprite = _rightSprite;
                break;
            case MoveSet.Up:
                _soloBattleImage.sprite = _upSprite;
                break;
            case MoveSet.Down:
                _soloBattleImage.sprite = _downSprite;
                break;
        }
    }

    void DeactivateSoloBattle(bool defeatedEnemy)
    {
        _hitIndicator.SetActive(true);
        _soloBattlePanel.SetActive(false);

        GlobalValues.SetGameState(GameState.Normal);

        EventHandler.Event_StopSlowTime?.Invoke();
        EventHandler.Event_DeactivatedSoloBattle?.Invoke();

        if (defeatedEnemy)
        {
            _currentEnemy.SetActive(false);
            EventHandler.Event_DefeatedEnemy?.Invoke(_currentEnemy);
        }
    }

    void CheckMove(MoveSet move)
    {
        if (move == _currentMoveToHit)
        {
            EventHandler.Event_EnemyHit?.Invoke(_currentEnemy);
            _isWaitingForAction = false;
        }
        else
            EventHandler.Event_PlayerHit?.Invoke(1);
    }
}
