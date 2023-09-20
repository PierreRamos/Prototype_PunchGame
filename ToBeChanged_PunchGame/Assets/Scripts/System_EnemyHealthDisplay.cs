using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class System_EnemyHealthDisplay : MonoBehaviour
{
    [SerializeField]
    Transform _hitPanel;

    [SerializeField]
    GameObject _hitOrb;

    System_EventHandler EventHandler;

    private void Start()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_EnemyHealthValueChange += UpdateHealthDisplay;

        UpdateHealthDisplay(
            gameObject,
            gameObject.GetComponent<System_EnemyHealth>().GetEnemyHealth()
        );
    }

    private void OnDisable()
    {
        EventHandler.Event_EnemyHealthValueChange -= UpdateHealthDisplay;
    }

    void UpdateHealthDisplay(GameObject gameObject, int health)
    {
        if (this.gameObject == gameObject)
        {
            ClearHealthDisplay();

            for (int i = 0; i < health; i++)
            {
                Instantiate(_hitOrb, _hitPanel);
            }
        }
    }

    void ClearHealthDisplay()
    {
        foreach (Transform child in _hitPanel)
        {
            Destroy(child.gameObject);
        }
    }
}
