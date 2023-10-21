using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class System_SoloBattlePrompt : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Init")]
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

    private Dictionary<MoveSet, Sprite> _arrowSprites = new Dictionary<MoveSet, Sprite>();
    private List<MoveSet> _movesToHit = new List<MoveSet>();

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_TriggeredSoloBattle += (enemy, listOfMoves) =>
        {
            print(_promptListObject.transform.localPosition.x);

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
        };

        EventHandler.Event_Hit += (move) =>
        {
            MovePrompt();
        };
    }

    private void Awake()
    {
        _arrowSprites[MoveSet.Left] = _leftArrow;
        _arrowSprites[MoveSet.Right] = _rightArrow;
        _arrowSprites[MoveSet.Up] = _upArrow;
        _arrowSprites[MoveSet.Down] = _downArrow;
    }

    // private void CheckInput(MoveSet move)
    // {

    // }

    private void MovePrompt()
    {
        _promptListObject
            .DOLocalMoveX(_promptListObject.transform.localPosition.x - _scrollMoveValue, 0.25f)
            .SetUpdate(true);
    }
}
