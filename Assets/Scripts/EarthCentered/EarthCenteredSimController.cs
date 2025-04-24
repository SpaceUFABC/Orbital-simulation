using UnityEngine;
using static OrbitVisualization;
using System;

public class EarthCenteredSimController : MonoBehaviour
{
    Satellite[] bodies;
    public float orbitTimeStep = 1f;
    public float gravitationalConstant = 10f;
    public int orbitMaxSteps = 1000; // Maximum number of steps for the orbit tracer
    public Earth earth;
    private OrbitVisualization[] orbitVisualizations; // Array to store orbit visualizations for each body


    void Awake()
    {
        // Find all planet objects
        bodies = FindObjectsByType<Satellite>(FindObjectsSortMode.None);
        earth = FindFirstObjectByType<Earth>();

        orbitVisualizations = new OrbitVisualization[bodies.Length];

        foreach (Satellite body in bodies)
        {
            OrbitVisualization orbitVisualization = new OrbitVisualization(body, earth, gravitationalConstant, orbitTimeStep, this.orbitMaxSteps);
            orbitVisualizations[Array.IndexOf(bodies, body)] = orbitVisualization; // Store the orbit visualization for each body
        }
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

        // Update calculations of orbit visualizations for each body
        foreach (OrbitVisualization orbitVisualization in orbitVisualizations)
        {
            if (orbitVisualization != null)
            {
                orbitVisualization.UpdateValues();
            }
        } 
    }

    void Update()
    {
        // Update drawing of orbit visualizations for each body
        foreach (OrbitVisualization orbitVisualization in orbitVisualizations)
        {
            if (orbitVisualization != null)
            {
                orbitVisualization.UpdateDraw();
            }
        } 
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
}
