using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// ------------------------------
/// Creating instance of particles
/// ------------------------------
public class FireWorks : MonoBehaviour
{
    /// ------------------------------
    /// Singleton
    /// ------------------------------
    public static FireWorks Instance;

    public ParticleSystem effectA;
    public ParticleSystem effectB;
    public float zaxis1;
    public float zaxis2;
    public float xaxis1;
    public float xaxis2;
    public float yaxis1;
    public float yaxis2;

    void Awake()
    {
        /// ---------------------
        // Register the singleton
        /// ---------------------
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of FireWorks script!");
        }

        Instance = this;
    }

    void Update()
    {
        /// -----------------------------------------
        /// Instanciate into a box of 5 x 5 x 5 (xyz)
        /// -----------------------------------------
        FireWorks.Instance.Explosion(new Vector3(Random.Range(xaxis1, xaxis2), Random.Range(yaxis1, yaxis2), Random.Range(zaxis1, zaxis2)));
    }

    /// -----------------------------------------
    /// Create an explosion at the given location
    /// -----------------------------------------
    public void Explosion(Vector3 position)
    {
        instantiate(effectA, position);
        instantiate(effectB, position);
    }

    /// -----------------------------------------
    /// Instantiate a Particle system from prefab
    /// -----------------------------------------
    private ParticleSystem instantiate(ParticleSystem prefab, Vector3 position)
    {
        ParticleSystem newParticleSystem = Instantiate(prefab, position, Quaternion.identity) as ParticleSystem;

        /// -----------------------------
        // Make sure it will be destroyed
        /// -----------------------------
        Destroy(
            newParticleSystem.gameObject,
            newParticleSystem.startLifetime
        );

        return newParticleSystem;
    }



}