using UnityEngine;


public class NBodySupervisor : MonoBehaviour
{
    N_bodyPlanet[] planets;
    public float timeStep = 0.01f;

    void Awake()
    {
        // Find all planet objects
        planets = FindObjectsByType<N_bodyPlanet>(FindObjectsSortMode.None);
    }


    void FixedUpdate()
    {
        foreach (N_bodyPlanet planet in planets)
        {
            planet.updateVelocity(timeStep);
        }

        foreach (N_bodyPlanet planet in planets)
        {
            planet.updatePosition(timeStep);
        }
    }
}
