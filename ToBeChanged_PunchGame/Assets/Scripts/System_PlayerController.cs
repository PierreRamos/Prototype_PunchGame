using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class System_PlayerController : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;
    }

    public void AttackLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
            if (GlobalValues.GetGameState() == GameState.Normal)
                EventHandler.Event_AttackLeft?.Invoke();
            else if (GlobalValues.GetGameState() == GameState.SoloBattle)
                EventHandler.Event_Hit?.Invoke(MoveSet.Left);
    }

    public void AttackRight(InputAction.CallbackContext context)
    {
        if (context.performed)
            if (GlobalValues.GetGameState() == GameState.Normal)
                EventHandler.Event_AttackRight?.Invoke();
            else if (GlobalValues.GetGameState() == GameState.SoloBattle)
                EventHandler.Event_Hit?.Invoke(MoveSet.Right);
    }

    public void AttackUp(InputAction.CallbackContext context)
    {
        if (context.performed)
            if (GlobalValues.GetGameState() == GameState.SoloBattle)
                EventHandler.Event_Hit?.Invoke(MoveSet.Up);
    }

    public void AttackDown(InputAction.CallbackContext context)
    {
        if (context.performed)
            if (GlobalValues.GetGameState() == GameState.SoloBattle)
                EventHandler.Event_Hit?.Invoke(MoveSet.Down);
    }
}
