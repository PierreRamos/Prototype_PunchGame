using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class System_UIManager : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Initialization")]
    [Space]
    [SerializeField]
    TextMeshProUGUI _healthText;

    [SerializeField]
    TextMeshProUGUI _defeatedText;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_PlayerHealthValueChange += UpdateHealthDisplay;
        EventHandler.Event_EnemyDefeatedValueChange += UpdateDefeatedDisplay;
    }

    private void OnDisable()
    {
        EventHandler.Event_PlayerHealthValueChange -= UpdateHealthDisplay;
        EventHandler.Event_EnemyDefeatedValueChange -= UpdateDefeatedDisplay;
    }

    void UpdateHealthDisplay(int value)
    {
        _healthText.text = value.ToString();
    }

    void UpdateDefeatedDisplay(int value)
    {
        _defeatedText.text = $"Defeated: {value}";
    }
}
