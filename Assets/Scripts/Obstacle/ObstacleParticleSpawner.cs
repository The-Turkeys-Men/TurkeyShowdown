using System;
using Unity.Netcode;
using UnityEngine;

public class ObstacleParticleSpawner : MonoBehaviour
{
    public GameObject EnterParticlePrefab;
    public GameObject ExitParticlePrefab;
    [SerializeField] private string _nomSFX;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out BaseWeapon baseWeapon))
        {
            return;
        }
        Instantiate(EnterParticlePrefab, other.transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(_nomSFX,transform.position);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out BaseWeapon baseWeapon))
        {
            return;
        }
        Instantiate(ExitParticlePrefab, other.transform.position, Quaternion.identity);
    }
}
