using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class System_PlayerController : MonoBehaviour
{
    System_EventHandler EventHandler;
    System_GlobalValues GlobalValues;
    bool _isPaused;

    GameState _lastGameState;

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
            {
                _isPaused = true;
                _lastGameState = GlobalValues.GetGameState();
                GlobalValues.SetGameState(GameState.Paused);
            }
            else
            {
                _isPaused = false;
                GlobalValues.SetGameState(_lastGameState);
            }

            EventHandler.Event_Pause?.Invoke(_isPaused);
        }
    }

    public void AttackLeft(InputAction.CallbackContext context)
    {
        if (
            GlobalValues.GetGameState() == GameState.GameOver
            || GlobalValues.GetGameState() == GameState.Paused
        )
            return;

        if (context.performed)
            if (GlobalValues.GetGameState() == GameState.Normal)
                EventHandler.Event_AttackLeft?.Invoke();
            else if (GlobalValues.GetGameState() == GameState.SoloBattle)
                EventHandler.Event_Hit?.Invoke(MoveSet.Left);

        if (context.canceled)
            if (GlobalValues.GetGameState() == GameState.HoldBattle)
                EventHandler.Event_StoppedHoldInput?.Invoke();
    }

    public void AttackRight(InputAction.CallbackContext context)
    {
        if (
            GlobalValues.GetGameState() == GameState.GameOver
            || GlobalValues.GetGameState() == GameState.Paused
        )
            return;

        if (context.performed)
            if (GlobalValues.GetGameState() == GameState.Normal)
                EventHandler.Event_AttackRight?.Invoke();
            else if (GlobalValues.GetGameState() == GameState.SoloBattle)
                EventHandler.Event_Hit?.Invoke(MoveSet.Right);

        //Test
        if (context.canceled)
            if (GlobalValues.GetGameState() == GameState.HoldBattle)
                EventHandler.Event_StoppedHoldInput?.Invoke();
    }

    public void AttackUp(InputAction.CallbackContext context)
    {
        if (
            GlobalValues.GetGameState() == GameState.GameOver
            || GlobalValues.GetGameState() == GameState.Paused
        )
            return;

        if (context.performed)
            if (GlobalValues.GetGameState() == GameState.SoloBattle)
                EventHandler.Event_Hit?.Invoke(MoveSet.Up);
    }

    public void AttackDown(InputAction.CallbackContext context)
    {
        if (
            GlobalValues.GetGameState() == GameState.GameOver
            || GlobalValues.GetGameState() == GameState.Paused
        )
            return;

        if (context.performed)
            if (GlobalValues.GetGameState() == GameState.SoloBattle)
                EventHandler.Event_Hit?.Invoke(MoveSet.Down);
    }

    public void ActivateSpecial(InputAction.CallbackContext context)
    {
        var gameState = GlobalValues.GetGameState();

        if (context.performed && gameState == GameState.Normal)
            EventHandler.Event_ActivateSpecialInput?.Invoke();
    }
}
