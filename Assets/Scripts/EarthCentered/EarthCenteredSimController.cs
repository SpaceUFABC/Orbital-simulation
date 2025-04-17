using UnityEngine;
using static OrbitTracer;

public class EarthCenteredSimController : MonoBehaviour
{
    Satellite[] bodies;
    public float orbitTimeStep = 1f;
    public float gravitationalConstant = 10f;
    public int orbitMaxSteps = 1000; // Maximum number of steps for the orbit tracer
    public Earth earth;


    void Awake()
    {
        // Find all planet objects
        bodies = FindObjectsByType<Satellite>(FindObjectsSortMode.None);
        earth = FindFirstObjectByType<Earth>();
    }


    void FixedUpdate()
    {
        // Calculate the acceleration of each body and update each body's velocity
        foreach (Satellite body in bodies)
        {
            Vector3 acceleration = getAcceleration(body);
            Vector3 velocity = acceleration * Time.fixedDeltaTime;
            body.updateVelocity(velocity);
        }

        foreach (Satellite body in bodies)
        {
            body.updatePosition(Time.fixedDeltaTime);
        }

        // Draw the orbits of each body
        drawOrbits();
    }

    Vector3 getAcceleration(Satellite body)
    {
        Vector3 acceleration = new Vector3(0, 0, 0);

        Vector3 direction = earth.gameObject.transform.position - body.transform.position;
        float distance = direction.magnitude;
        float scalarAcceleration = (float)(gravitationalConstant * earth.mass / Mathf.Pow(distance, 2));
        acceleration = direction.normalized * scalarAcceleration;

        return acceleration;
    }

    public void drawOrbits()
    {
        foreach (Satellite body in bodies)
        {
            OrbitTracer orbitTracer = new OrbitTracer(body, earth, gravitationalConstant, orbitTimeStep, this.orbitMaxSteps);
            orbitTracer.DrawOrbit();
        }
    }
}
