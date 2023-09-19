using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_EventHandler : MonoBehaviour
{
    public static System_EventHandler Instance;

    //Debug events
    public Action Event_SpawnEnemy;

    //Player controller
    public Action Event_AttackLeft;
    public Action Event_AttackRight;

    //Attack events
    public Action<GameObject> Event_EnemyHit;

    //Value change events
    public Action<GameObject, int> Event_EnemyHealthValueChange;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
}
