using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class System_HoldMechanics : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;

        EventHandler.Event_TriggerHoldBattle += CheckHit;
    }

    private void OnDisable()
    {
        EventHandler.Event_TriggerHoldBattle -= CheckHit;
    }

    void CheckHit(GameObject gameObject)
    {
        if (this.gameObject != gameObject)
            return;

        if (GlobalValues.GetGameState() == GameState.Normal)
        {
            GlobalValues.SetGameState(GameState.HoldBattle);
            EventHandler.Event_TriggeredHoldBattle?.Invoke(gameObject);
        }
    }
}
