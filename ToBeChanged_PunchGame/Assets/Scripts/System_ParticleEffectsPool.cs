using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class System_ParticleEffectsPool : MonoBehaviour
{
    System_EventHandler EventHandler;

    [Header("Initialization")]
    [Space]
    [SerializeField]
    GameObject _hitEffectPrefab;

    [SerializeField]
    Transform _effectPoolParent;

    [Header("Particle Pool Settings")]
    [Space]
    [SerializeField]
    int _poolSize;

    List<GameObject> _hitParticlePool = new List<GameObject>();

    private void Start()
    {
        EventHandler = System_EventHandler.Instance;

        EventHandler.Event_HitEffect += ActivateHitParticle;

        // Create the pool of particle system instances
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject particleInstance = Instantiate(_hitEffectPrefab, _effectPoolParent);
            particleInstance.SetActive(false);
            _hitParticlePool.Add(particleInstance);
        }
    }

    private void OnDisable()
    {
        EventHandler.Event_HitEffect -= ActivateHitParticle;
    }

    public void ActivateHitParticle(Vector3 position)
    {
        // Find an inactive particle system in the pool and activate it
        foreach (GameObject particleInstance in _hitParticlePool)
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
