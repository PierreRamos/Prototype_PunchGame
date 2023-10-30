using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class System_UIManager : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    [Header("Initialization")]
    [Space]
    [SerializeField]
    GameObject _gameOverPanel;

    [SerializeField]
    GameObject _pausePanel;

    [SerializeField]
    TextMeshProUGUI _defeatedText;

    [Header("Stun UI")]
    [Space]
    [SerializeField]
    GameObject _stunBarObject;

    [SerializeField]
    Slider _stunBarSlider;

    [Header("Health UI")]
    [Space]
    [SerializeField]
    GameObject _baseHealthPanel;

    [SerializeField]
    GameObject _soloHealthPanel;

    TextMeshProUGUI _healthText;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        //Stun
        EventHandler.Event_TriggerStun += ActivateStunBar;
        EventHandler.Event_PlayerStunTimeChange += UpdateStunBarSlider;
        EventHandler.Event_PlayerStunFinished += DeactivateStunBar;

        //Health
        // EventHandler.Event_PlayerHealthValueChange += UpdateHealthDisplay;
        // EventHandler.Event_FocusHealthUI += FocusHealthUI;
        // EventHandler.Event_NormalHealthUI += NormalHealthUI;

        EventHandler.Event_EnemyDefeatedValueChange += UpdateDefeatedDisplay;
        EventHandler.Event_PlayerDied += ActivateGameOver;

        EventHandler.Event_Pause += PausePanel;
    }

    private void OnDisable()
    {
        EventHandler.Event_TriggerStun -= ActivateStunBar;
        EventHandler.Event_PlayerStunTimeChange -= UpdateStunBarSlider;
        EventHandler.Event_PlayerStunFinished -= DeactivateStunBar;

        // EventHandler.Event_PlayerHealthValueChange -= UpdateHealthDisplay;
        // EventHandler.Event_FocusHealthUI -= FocusHealthUI;
        // EventHandler.Event_NormalHealthUI -= NormalHealthUI;

        EventHandler.Event_EnemyDefeatedValueChange -= UpdateDefeatedDisplay;
        EventHandler.Event_PlayerDied -= ActivateGameOver;

        EventHandler.Event_Pause -= PausePanel;
    }

    private void Start()
    {
        _healthText = _baseHealthPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    // void NormalHealthUI()
    // {
    //     var healthValue = GlobalValues.GetPlayerHealth();

    //     _healthText = _baseHealthPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    //     _healthText.text = healthValue.ToString();

    //     _baseHealthPanel.SetActive(true);
    //     _soloHealthPanel.SetActive(false);
    // }

    // void FocusHealthUI()
    // {
    //     var healthValue = GlobalValues.GetPlayerHealth();

    //     _healthText = _soloHealthPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    //     _healthText.text = healthValue.ToString();

    //     _soloHealthPanel.SetActive(true);
    //     _baseHealthPanel.SetActive(false);
    // }

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

    void UpdateDefeatedDisplay(int value)
    {
        _defeatedText.text = $"Defeated: {value}";
    }
}
