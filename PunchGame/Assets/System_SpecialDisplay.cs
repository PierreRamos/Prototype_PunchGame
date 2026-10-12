using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class System_SpecialDisplay : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    [Header("Dependencies")]
    [SerializeField]
    private Slider _specialMeterSlider;

    [SerializeField]
    private Animator _specialMeterAnimator;

    [Space]
    [SerializeField]
    private List<Animator> _specialIndicatorAnimators;

    [Space]
    [SerializeField]
    private GameObject _specialCutscenePanel;

    [SerializeField]
    private RectTransform _specialCutsceneContainer;

    [Header("Special Display Settings")]
    [SerializeField]
    private float _specialCutsceneDelayDuration;

    private void Awake()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_SetSpecialMeterMaxValue += (maxSpecialValue) =>
        {
            _specialMeterSlider.maxValue = maxSpecialValue;
        };
        EventHandler.Event_SpecialMeterValueChange += (specialValue) =>
        {
            _specialMeterSlider.value = specialValue;
        };
        EventHandler.Event_MaxedSpecialMeter += () =>
        {
            _specialMeterAnimator.SetTrigger("Maxed");

            foreach (Animator animator in _specialIndicatorAnimators)
            {
                animator.SetTrigger("Maxed");
            }
        };
        EventHandler.Event_SpecialActive += (specialActive, specialDuration) =>
        {
            if (specialActive)
            {
                GlobalValues.SetGameState(GameState.Paused);

                if (!_specialCutscenePanel.activeSelf)
                    _specialCutscenePanel.SetActive(true);

                _specialCutsceneContainer.sizeDelta = new Vector2(
                    _specialCutsceneContainer.sizeDelta.x,
                    0f
                );

                _specialCutsceneContainer
                    .DOSizeDelta(new Vector2(_specialCutsceneContainer.sizeDelta.x, 200f), 0.15f)
                    .SetUpdate(true)
                    .OnComplete(
                        () =>
                        {
                            StartCoroutine(CutsceneTimer(specialActive, specialDuration));
                        }
                    );
            }
            else
            {
                string animation = specialActive ? "Active" : "Normal";
                _specialMeterAnimator.SetTrigger(animation);

                foreach (Animator animator in _specialIndicatorAnimators)
                {
                    animator.SetTrigger("Normal");
                }
            }
        };
    }

    private IEnumerator CutsceneTimer(bool specialActive, float specialDuration)
    {
        yield return new WaitForSecondsRealtime(_specialCutsceneDelayDuration);

        _specialCutsceneContainer
            .DOSizeDelta(new Vector2(_specialCutsceneContainer.sizeDelta.x, 0f), 0.15f)
            .SetUpdate(true)
            .OnComplete(
                () =>
                {
                    GlobalValues.SetGameState(GameState.Normal);
                    _specialCutscenePanel.SetActive(false);

                    EventHandler.Event_SpecialCutsceneFinished?.Invoke();

                    if (specialActive)
                        _specialMeterSlider.DOValue(0, specialDuration).SetEase(Ease.Linear);

                    string animation = specialActive ? "Active" : "Normal";
                    _specialMeterAnimator.SetTrigger(animation);

                    foreach (Animator animator in _specialIndicatorAnimators)
                    {
                        animator.SetTrigger("Active");
                    }
                }
            );
    }
}
