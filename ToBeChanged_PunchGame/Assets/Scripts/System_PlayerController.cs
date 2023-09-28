using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class System_PlayerController : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;
    bool _isPaused;

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;
        GlobalValues = System_GlobalValues.Instance;
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (GlobalValues.GetGameState() == GameState.GameOver)
            return;

        if (context.performed)
        {
            if (!_isPaused)
                _isPaused = true;
            else
                _isPaused = false;

            EventHandler.Event_Pause?.Invoke(_isPaused);
        }
    }

    public void AttackLeft(InputAction.CallbackContext context)
    {
        if (GlobalValues.GetGameState() == GameState.GameOver)
            return;

        if (context.performed)
            if (GlobalValues.GetGameState() == GameState.Normal)
                EventHandler.Event_AttackLeft?.Invoke();
            else if (GlobalValues.GetGameState() == GameState.SoloBattle)
                EventHandler.Event_Hit?.Invoke(MoveSet.Left);
    }

    public void AttackRight(InputAction.CallbackContext context)
    {
        if (GlobalValues.GetGameState() == GameState.GameOver)
            return;

        if (context.performed)
            if (GlobalValues.GetGameState() == GameState.Normal)
                EventHandler.Event_AttackRight?.Invoke();
            else if (GlobalValues.GetGameState() == GameState.SoloBattle)
                EventHandler.Event_Hit?.Invoke(MoveSet.Right);
    }

    public void AttackUp(InputAction.CallbackContext context)
    {
        if (GlobalValues.GetGameState() == GameState.GameOver)
            return;

        if (context.performed)
            if (GlobalValues.GetGameState() == GameState.SoloBattle)
                EventHandler.Event_Hit?.Invoke(MoveSet.Up);
    }

    public void AttackDown(InputAction.CallbackContext context)
    {
        if (GlobalValues.GetGameState() == GameState.GameOver)
            return;

        if (context.performed)
            if (GlobalValues.GetGameState() == GameState.SoloBattle)
                EventHandler.Event_Hit?.Invoke(MoveSet.Down);
    }
}
