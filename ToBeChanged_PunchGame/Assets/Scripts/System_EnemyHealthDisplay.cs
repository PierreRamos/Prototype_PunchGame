using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class System_EnemyHealthDisplay : MonoBehaviour
{
    [SerializeField]
    Transform _hitPanel;

    [Space]
    [SerializeField]
    Sprite _normalOrbSprite;

    [SerializeField]
    Sprite _soloOrbSprite;

    [SerializeField]
    Sprite _holdOrbSprite;
    List<GameObject> _listOfOrbs = new List<GameObject>();

    System_EventHandler EventHandler;

    private void Awake()
    {
        //Adds orbs to _listOfOrbs
        foreach (RectTransform child in _hitPanel)
        {
            _listOfOrbs.Add(child.gameObject);
        }
    }

    private void OnEnable()
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

    private void Start()
    {
        //Updates sprites if enemy is not normal type depending on enemy type
        if (IsEliteEnemy())
        {
            foreach (var orb in _listOfOrbs)
            {
                orb.GetComponent<SpriteRenderer>().sprite = _soloOrbSprite;
            }
        }
    }

    void UpdateHealthDisplay(GameObject gameObject, int health)
    {
        if (this.gameObject == gameObject)
        {
            ClearHealthDisplay();

            //If enemy is elite only put one health orb since elites don't have HP scripts
            if (IsEliteEnemy())
            {
                if (!_listOfOrbs[0].activeSelf)
                    _listOfOrbs[0].SetActive(true);
                return;
            }

            for (int i = 0; i < health; i++)
            {
                if (!_listOfOrbs[i].activeSelf)
                    _listOfOrbs[i].SetActive(true);
            }
        }
    }

    void ClearHealthDisplay()
    {
        foreach (var orb in _listOfOrbs)
        {
            if (orb.activeSelf)
                orb.SetActive(false);
        }
    }

    //Method Conditions
    bool IsEliteEnemy()
    {
        return gameObject.GetComponent<System_EnemyType>().GetEnemyType() == EnemyType.elite;
    }
}
