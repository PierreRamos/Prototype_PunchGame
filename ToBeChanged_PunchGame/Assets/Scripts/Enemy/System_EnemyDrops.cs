using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class System_EnemyDrops : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    [Header("Initialization")]
    [Space]
    [SerializeField]
    Sprite _healthPotionSprite;

    [SerializeField]
    Transform _itemPanel;

    [Header("Enemy Drops Settings")]
    [Space]
    [Range(0, 100)]
    [SerializeField]
    float _healthPotionDropChance;

    [SerializeField]
    int _healthPotionValue;

    List<GameObject> _listOfItems = new List<GameObject>();

    bool _hasHealthPotion,
        _isDefeated;

    void Awake()
    {
        foreach (RectTransform child in _itemPanel)
        {
            _listOfItems.Add(child.gameObject);
        }
    }

    void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        GenerateItems();

        EventHandler.Event_DefeatedEnemy += (enemy) =>
        {
            if (enemy == gameObject && _hasHealthPotion && !_isDefeated)
            {
                print("called?");
                EventHandler.Event_HealPlayer?.Invoke(_healthPotionValue);
                ClearItemDisplay();
                _isDefeated = true;
            }
        };
    }

    void OnDisable()
    {
        ClearItemDisplay();
        ResetBool();
    }

    //Generates items carried by enemies on enable
    void GenerateItems()
    {
        var random = Random.Range(0, 101);

        //2% chance to generate a potion
        if (random < _healthPotionDropChance)
        {
            foreach (var item in _listOfItems)
            {
                if (!item.activeSelf)
                {
                    _hasHealthPotion = true;
                    item.GetComponent<Image>().sprite = _healthPotionSprite;
                    item.SetActive(true);
                    break;
                }
            }
        }
        //Other items code here
    }

    //Disables items in enemy item panel
    void ClearItemDisplay()
    {
        foreach (var item in _listOfItems)
        {
            if (item.activeSelf)
                item.SetActive(false);
        }
    }

    void ResetBool()
    {
        _hasHealthPotion = false;
        _isDefeated = false;
    }
}
