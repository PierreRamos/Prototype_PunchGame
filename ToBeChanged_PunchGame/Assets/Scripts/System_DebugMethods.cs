using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class System_DebugMethods : MonoBehaviour
{
    public void SpawnEnemy(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            System_EventHandler.Instance.Event_SpawnEnemy?.Invoke();
        }
    }
}
