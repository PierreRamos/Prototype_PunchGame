using System.Collections;
using System.Collections.Generic;
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

    private Dictionary<MoveSet, Sprite> _arrowSprites = new Dictionary<MoveSet, Sprite>();

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_TriggeredSoloBattle += (enemy, listOfMoves) =>
        {
            foreach (var move in listOfMoves)
            {
                foreach (Transform prompt in _promptListObject)
                {
                    if (!prompt.gameObject.activeSelf)
                    {
                        prompt.GetComponent<Image>().sprite = _arrowSprites[move];
                        prompt.gameObject.SetActive(true);
                        break;
                    }
                }
            }
        };
    }

    private void Awake()
    {
        _arrowSprites[MoveSet.Left] = _leftArrow;
        _arrowSprites[MoveSet.Right] = _rightArrow;
        _arrowSprites[MoveSet.Up] = _upArrow;
        _arrowSprites[MoveSet.Down] = _downArrow;
    }
}
