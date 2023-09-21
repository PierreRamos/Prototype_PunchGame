using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_HitIndicator : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Initialization")]
    [Space]
    [SerializeField]
    SpriteRenderer _center;

    [SerializeField]
    SpriteRenderer _left;

    [SerializeField]
    SpriteRenderer _right;

    [Space]
    [SerializeField]
    Sprite _deactivatedSprite;

    [SerializeField]
    Sprite _leftActivatedSprite;

    [SerializeField]
    Sprite _rightActivatedSprite;

    void Start()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_HasEnemyLeft += ActivateLeft;
        EventHandler.Event_HasEnemyRight += ActivateRight;
    }

    void OnDisable()
    {
        EventHandler.Event_HasEnemyLeft += ActivateLeft;
        EventHandler.Event_HasEnemyRight += ActivateRight;
    }

    void ActivateLeft(bool value)
    {
        if (value == true)
            _left.sprite = _leftActivatedSprite;
        else
            _left.sprite = _deactivatedSprite;
    }

    void ActivateRight(bool value)
    {
        if (value == true)
            _right.sprite = _rightActivatedSprite;
        else
            _right.sprite = _deactivatedSprite;
    }
}
