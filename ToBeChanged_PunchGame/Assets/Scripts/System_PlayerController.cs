using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class System_PlayerController : MonoBehaviour
{
    public void AttackLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
            System_EventHandler.Instance.Event_AttackLeft?.Invoke();
    }

    public void AttackRight(InputAction.CallbackContext context)
    {
        if (context.performed)
            System_EventHandler.Instance.Event_AttackRight?.Invoke();
    }
}
