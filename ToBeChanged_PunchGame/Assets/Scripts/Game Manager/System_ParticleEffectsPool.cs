using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_ParticleEffectsPool : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Initialization")]
    [SerializeField]
    Transform _effectPoolParent;

    [SerializeField]
    Transform _playerEffectPoolParent;

    [Space]
    [SerializeField]
    GameObject _hitEffectPrefab;

    [SerializeField]
    GameObject _healEffectPrefab;

    [SerializeField]
    GameObject _defeatEffectPrefab;

    [SerializeField]
    GameObject _exclamationEffectPrefab;

    [Header("Particle Pool Settings")]
    [SerializeField]
    int _hitEffectPoolSize;

    [SerializeField]
    int _healEffectPoolSize;

    [SerializeField]
    int _defeatEffectPoolSize;

    [SerializeField]
    int _exclamationEffectPoolSize;

    List<GameObject> _hitParticlePool = new List<GameObject>();
    List<GameObject> _healParticlePool = new List<GameObject>();
    List<GameObject> _defeatParticlePool = new List<GameObject>();
    List<GameObject> _exclamationParticlePool = new List<GameObject>();

    private void OnEnable()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_EnemyHit += ActivateHitParticle;
        EventHandler.Event_DefeatedEnemy += ActivateDefeatParticle;
        EventHandler.Event_ExclamationEffect += ActivateExclamationParticle;
        EventHandler.Event_PlayerHealEffect += ActivateHealParticle;
    }

    private void OnDisable()
    {
        EventHandler.Event_EnemyHit -= ActivateHitParticle;
        EventHandler.Event_DefeatedEnemy -= ActivateDefeatParticle;
        EventHandler.Event_ExclamationEffect -= ActivateExclamationParticle;
        EventHandler.Event_PlayerHealEffect -= ActivateHealParticle;
    }

    private void Start()
    {
        // Create the pool of particle system instances
        for (int i = 0; i < _hitEffectPoolSize; i++)
        {
            PrefabInstantiation(_hitEffectPrefab, _hitParticlePool, _effectPoolParent);
        }
        for (int i = 0; i < _healEffectPoolSize; i++)
        {
            PrefabInstantiation(_healEffectPrefab, _healParticlePool, _playerEffectPoolParent);
        }
        for (int i = 0; i < _defeatEffectPoolSize; i++)
        {
            PrefabInstantiation(_defeatEffectPrefab, _defeatParticlePool, _effectPoolParent);
        }
        for (int i = 0; i < _exclamationEffectPoolSize; i++)
        {
            PrefabInstantiation(
                _exclamationEffectPrefab,
                _exclamationParticlePool,
                _effectPoolParent
            );
        }
    }

    private void PrefabInstantiation(
        GameObject gameObject,
        List<GameObject> poolList,
        Transform effectParent
    )
    {
        GameObject particleInstance = Instantiate(gameObject, effectParent);
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

    public void ActivateHealParticle(Vector3 playerPosition)
    {
        // Find an inactive particle system in the pool and activate it
        foreach (GameObject particleInstance in _healParticlePool)
        {
            if (!particleInstance.activeInHierarchy)
            {
                particleInstance.transform.position = playerPosition;
                particleInstance.SetActive(true);
                return;
            }
        }
    }

    public void ActivateDefeatParticle(GameObject enemy)
    {
        // Find an inactive particle system in the pool and activate it
        foreach (GameObject particleInstance in _defeatParticlePool)
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
