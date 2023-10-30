using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_HitIndicator : MonoBehaviour
{
    System_EventHandler EventHandler;

    System_GlobalValues GlobalValues;

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
    Sprite _leftDeactivatedSprite;

    [SerializeField]
    Sprite _rightDeactivatedSprite;

    [SerializeField]
    Sprite _leftActivatedSprite;

    [SerializeField]
    Sprite _rightActivatedSprite;

    void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_HasEnemyLeft += ActivateLeft;
        EventHandler.Event_HasEnemyRight += ActivateRight;
        EventHandler.Event_PlayerAttackRangeChange += UpdateHitIndicatorRange;
    }

    void OnDisable()
    {
        EventHandler.Event_HasEnemyLeft -= ActivateLeft;
        EventHandler.Event_HasEnemyRight -= ActivateRight;
        EventHandler.Event_PlayerAttackRangeChange -= UpdateHitIndicatorRange;
    }

    void UpdateHitIndicatorRange()
    {
        var playerAttackRange = GlobalValues.GetPlayerAttackRange();

        _left.size = new Vector2(playerAttackRange - 0.1f, _left.size.y);
        _right.size = new Vector2(playerAttackRange - 0.1f, _right.size.y);
    }

    void ActivateLeft(bool value)
    {
        if (value == true)
            _left.sprite = _leftActivatedSprite;
        else
            _left.sprite = _leftDeactivatedSprite;
    }

    void ActivateRight(bool value)
    {
        if (value == true)
            _right.sprite = _rightActivatedSprite;
        else
            _right.sprite = _rightDeactivatedSprite;
    }
}
