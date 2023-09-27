using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class System_UIManager : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Initialization")]
    [Space]
    [SerializeField]
    GameObject _gameOverPanel;

    [SerializeField]
    GameObject _pausePanel;

    [SerializeField]
    TextMeshProUGUI _healthText;

    [SerializeField]
    TextMeshProUGUI _defeatedText;

    [Header("Stun UI")]
    [Space]
    [SerializeField]
    GameObject _stunBarObject;

    [SerializeField]
    Slider _stunBarSlider;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        //Stun
        EventHandler.Event_TriggerStun += ActivateStunBar;
        EventHandler.Event_PlayerStunTimeChange += UpdateStunBarSlider;
        EventHandler.Event_PlayerStunFinished += DeactivateStunBar;

        //Health
        EventHandler.Event_PlayerHealthValueChange += UpdateHealthDisplay;

        EventHandler.Event_EnemyDefeatedValueChange += UpdateDefeatedDisplay;
        EventHandler.Event_PlayerDied += ActivateGameOver;

        EventHandler.Event_Pause += PausePanel;
    }

    private void OnDisable()
    {
        EventHandler.Event_TriggerStun -= ActivateStunBar;
        EventHandler.Event_PlayerStunTimeChange -= UpdateStunBarSlider;
        EventHandler.Event_PlayerStunFinished -= DeactivateStunBar;

        EventHandler.Event_PlayerHealthValueChange -= UpdateHealthDisplay;

        EventHandler.Event_EnemyDefeatedValueChange -= UpdateDefeatedDisplay;
        EventHandler.Event_PlayerDied -= ActivateGameOver;

        EventHandler.Event_Pause -= PausePanel;
    }

    void PausePanel(bool value)
    {
        _pausePanel.SetActive(value);
    }

    void ActivateStunBar()
    {
        if (!_stunBarObject.activeSelf)
            _stunBarObject.SetActive(true);
    }

    void DeactivateStunBar()
    {
        if (_stunBarObject.activeSelf)
            _stunBarObject.SetActive(false);
    }

    void UpdateStunBarSlider(float value)
    {
        _stunBarSlider.value = value;
    }

    void ActivateGameOver()
    {
        if (!_gameOverPanel.activeSelf)
            _gameOverPanel.SetActive(true);
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
