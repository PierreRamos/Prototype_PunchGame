using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_PlayerHealthDisplay : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Dependencies")]
    [SerializeField]
    private Transform _healthContainer;

    [Header("Player Display Settings")]
    [SerializeField]
    private int _maxPlayerHealth;
    private List<GameObject> _healthObjectList = new List<GameObject>();

    private void Awake()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_PlayerHealthValueChange += UpdateHealthDisplay;

        foreach (Transform healthObject in _healthContainer)
        {
            _healthObjectList.Add(healthObject.gameObject);
        }

        // _healthObjectList.Reverse();
    }

    private void UpdateHealthDisplay(int healthValue)
    {
        print($"Current player health: {healthValue}");

        for (int i = 0; i < _maxPlayerHealth; i++)
        {
            if (_healthObjectList[i].activeSelf == false && i + 1 <= healthValue)
            {
                _healthObjectList[i].SetActive(true);
                print("Enabled");
            }
            else if (_healthObjectList[i].activeSelf == true && i + 1 > healthValue)
            {
                _healthObjectList[i].SetActive(false);
                print("Disabled");
            }
        }
    }
}
