using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class System_EnemyHitTypeDisplay : MonoBehaviour
{
    [SerializeField]
    Transform _hitPanel;

    [Space]
    [SerializeField]
    Sprite _normalOrbSprite;

    [SerializeField]
    Sprite _soloOrbSprite;

    [SerializeField]
    Sprite _dashOrbSprite;

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

        EventHandler.Event_EnemyHitListChange += UpdateHitTypeDisplay;
    }

    private void OnDisable()
    {
        EventHandler.Event_EnemyHitListChange -= UpdateHitTypeDisplay;
    }

    void UpdateHitTypeDisplay(GameObject gameObject, List<HitType> listOfHits)
    {
        if (this.gameObject != gameObject)
            return;

        ClearHitTypeDisplay();

        for (int i = 0; i < listOfHits.Count; i++)
        {
            if (!_listOfOrbs[i].activeSelf)
            {
                if (listOfHits[i] == HitType.normal)
                    _listOfOrbs[i].GetComponent<SpriteRenderer>().sprite = _normalOrbSprite;
                else if (listOfHits[i] == HitType.solo)
                    _listOfOrbs[i].GetComponent<SpriteRenderer>().sprite = _soloOrbSprite;
                else if (listOfHits[i] == HitType.dash)
                    _listOfOrbs[i].GetComponent<SpriteRenderer>().sprite = _dashOrbSprite;

                _listOfOrbs[i].SetActive(true);
            }
        }
    }

    void ClearHitTypeDisplay()
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
