using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_ParticleEffectsPool : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Initialization")]
    [Space]
    [SerializeField]
    Transform _effectPoolParent;

    [SerializeField]
    GameObject _hitEffectPrefab;

    [SerializeField]
    GameObject _exclamationEffectPrefab;

    [Header("Particle Pool Settings")]
    [Space]
    [SerializeField]
    int _hitEffectPoolSize;

    [SerializeField]
    int _exclamationEffectPoolSize;

    List<GameObject> _hitParticlePool = new List<GameObject>();
    List<GameObject> _exclamationParticlePool = new List<GameObject>();

    private void Start()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_EnemyHit += ActivateHitParticle;
        EventHandler.Event_ExclamationEffect += ActivateExclamationParticle;

        // Create the pool of particle system instances
        for (int i = 0; i < _hitEffectPoolSize; i++)
        {
            PrefabInstantiation(_hitEffectPrefab, _hitParticlePool);
        }
        for (int i = 0; i < _exclamationEffectPoolSize; i++)
        {
            PrefabInstantiation(_exclamationEffectPrefab, _exclamationParticlePool);
        }
    }

    private void OnDisable()
    {
        EventHandler.Event_EnemyHit -= ActivateHitParticle;
        EventHandler.Event_ExclamationEffect += ActivateExclamationParticle;
    }

    private void PrefabInstantiation(GameObject gameObject, List<GameObject> poolList)
    {
        GameObject particleInstance = Instantiate(gameObject, _effectPoolParent);
        particleInstance.SetActive(false);
        poolList.Add(particleInstance);
    }

    public void ActivateHitParticle(GameObject enemy)
    {
        // Find an inactive particle system in the pool and activate it
        foreach (GameObject particleInstance in _hitParticlePool)
        {
            if (!particleInstance.activeInHierarchy)
            {
                particleInstance.transform.position = enemy.transform.position;
                particleInstance.SetActive(true);
                return;
            }
        }
    }

    public void ActivateExclamationParticle(Vector3 position)
    {
        // Find an inactive particle system in the pool and activate it
        foreach (GameObject particleInstance in _exclamationParticlePool)
        {
            if (!particleInstance.activeInHierarchy)
            {
                particleInstance.transform.position = position;
                particleInstance.SetActive(true);
                return;
            }
        }
    }
}
