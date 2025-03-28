using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class TwoBodySimController : MonoBehaviour
{
    TwoBodyObj[] bodies;
    public float timeStep = 0.01f;
    public float futureSimTimeStep = 1f;
    public double gravitationalConstant = 10;
    public int simulationSteps = 1000;

    void Awake()
    {
        // Find all planet objects
        bodies = FindObjectsByType<TwoBodyObj>(FindObjectsSortMode.None);
    }


    void FixedUpdate()
    {
        // Calculate the acceleration of each body and update each body's velocity
        foreach (TwoBodyObj body in bodies)
        {
            Vector3 acceleration = getAcceleration(body);
            Vector3 velocity = acceleration * timeStep;
            body.updateVelocity(velocity);
        }

        foreach (TwoBodyObj body in bodies)
        {
            body.updatePosition(timeStep);
        }

        // Simulate future steps
        Vector3[][] futurePositions = simulateFutureSteps();

        for (int i = 0; i < futurePositions.Length; i++)
        {
            bodies[i].orbitLine.positionCount = futurePositions[i].Length;
            for (int j = 0; j < futurePositions[i].Length; j++)
            {
                bodies[i].orbitLine.SetPosition(j, futurePositions[i][j]);
            }
        }
    }

    Vector3 getAcceleration(TwoBodyObj body)
    {
        Vector3 acceleration = new Vector3(0, 0, 0);

        body.updateClosestBody();
        TwoBodyObj otherBody = body.otherBody;

        if (otherBody != null)
        {
            Vector3 direction = otherBody.transform.position - body.transform.position;
            float distance = direction.magnitude;
            float scalarAcceleration = (float)(gravitationalConstant * otherBody.mass / Mathf.Pow(distance, 2));
            acceleration = direction.normalized * scalarAcceleration;
        }

        return acceleration;
    }

    Vector3 getVirtualAcceleration(TwoBodyObj body, Vector3 virtualPosition1, Vector3 virtualPosition2)
    {
        TwoBodyObj otherBody = body.otherBody;

        Vector3 acceleration = new Vector3(0, 0, 0);

        Vector3 direction = virtualPosition2 - virtualPosition1;
        float distance = direction.magnitude;
        float scalarAcceleration = (float)(gravitationalConstant * otherBody.mass / Mathf.Pow(distance, 2));
        acceleration = direction.normalized * scalarAcceleration;

        return acceleration;
    }

    Vector3[][] simulateFutureSteps()
    {
        // Initialize virtual positions and future positions arrays
        Vector3[][] futureVelocities = new Vector3[bodies.Length][];
        Vector3[][] futurePositions = new Vector3[bodies.Length][];
        
        for (int i = 0; i < bodies.Length; i++)
        {
            futureVelocities[i] = new Vector3[simulationSteps];
            futurePositions[i] = new Vector3[simulationSteps];

            futureVelocities[i][0] = bodies[i].velocity;
            futurePositions[i][0] = bodies[i].transform.position;
        }

        // Simulate future steps
        for (int step = 0; step < simulationSteps - 1; step++)
        {
            for (int i = 0; i < bodies.Length; i++)
            {
                TwoBodyObj body = bodies[i];
                Vector3 acceleration = getVirtualAcceleration(body, futurePositions[i][step], futurePositions[System.Array.IndexOf(bodies, body.otherBody)][step]);

                futureVelocities[i][step+1] = futureVelocities[i][step] + acceleration*futureSimTimeStep;
                futurePositions[i][step+1] = futurePositions[i][step] + futureVelocities[i][step+1]*futureSimTimeStep;
            }
        }

        return futurePositions;
    }
}
