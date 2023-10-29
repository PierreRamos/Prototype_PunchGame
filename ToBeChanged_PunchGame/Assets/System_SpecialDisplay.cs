using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class System_SpecialDisplay : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Dependencies")]
    [SerializeField]
    private Slider _specialMeterSlider;

    [SerializeField]
    private Animator _specialMeterAnimator;

    private void Awake()
    {
        EventHandler = System_EventHandler.Instance;

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
        };
    }
}
