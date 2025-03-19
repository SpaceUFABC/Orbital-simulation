using UnityEngine;


public class NBodySupervisor : MonoBehaviour
{
    N_bodyPlanet[] bodies;
    public float timeStep = 0.01f;

    public double gravitationalConstant = 10;

    void Awake()
    {
        // Find all planet objects
        bodies = FindObjectsByType<N_bodyPlanet>(FindObjectsSortMode.None);
    }


    void FixedUpdate()
    {
        foreach (N_bodyPlanet body in bodies)
        {
            Vector3 acceleration = getAcceleration(body);
            Vector3 velocity = acceleration * timeStep;
            body.updateVelocity(velocity);
        }

        foreach (N_bodyPlanet body in bodies)
        {
            body.updatePosition(timeStep);
        }
    }

    Vector3 getAcceleration(N_bodyPlanet ignoreBody)
    {
        Vector3 acceleration = new Vector3(0, 0, 0);

        // Loop through ll other bodies and calculate the acceleration due to each
        foreach (N_bodyPlanet body in bodies)
        {
            if (body == ignoreBody)
            {
                continue;
            }


            Vector3 otherPosition = body.transform.position;
            Vector3 direction = (otherPosition - ignoreBody.transform.position).normalized; // Direction vector from this planet to the other planet
            double distance = Vector3.Distance(otherPosition, ignoreBody.transform.position); // Distance between the two planets
            double scalarAcceleration = gravitationalConstant * body.mass / (distance * distance);
            // Calculates effect of other body on this body's acceleration
            Vector3 bodyAcceleration = direction * (float)scalarAcceleration;

            acceleration += bodyAcceleration;

            Debug.Log("Acceleration: " + acceleration + ", F: " + scalarAcceleration + ", D: " + distance + ", Direction: " + direction);
        }

        return acceleration;
    }
}
