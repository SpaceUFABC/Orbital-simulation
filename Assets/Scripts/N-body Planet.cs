using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "N-bodyPlanet", menuName = "Scriptable Objects/N-bodyPlanet")]
public class N_bodyPlanet : MonoBehaviour
{
    // Planet properties
    // public double scale; // Lets just ignore this for now ;)
    public double gravitationalConstant;
    public double mass;

    public Vector3 initialVelocity;

    private Vector3 position;
    private Vector3 velocity;

    private GameObject[] otherBodies;

    void Start() { 
        // Instantiate list of other planet objects
        List<GameObject> otherBodiesList = new List<GameObject>();

        N_bodyPlanet[] otherScripts = UnityEngine.Object.FindObjectsByType<N_bodyPlanet>(FindObjectsSortMode.None);

        // Add all the other planet objects to the otherBodies array
        foreach (N_bodyPlanet script in otherScripts) {
            otherBodiesList.Add(script.gameObject);
        }
        otherBodies = otherBodiesList.ToArray(); 
    }


    public void updateVelocity(float timeStep) {
        Vector3 acceleration = new Vector3(0, 0, 0);

        // Loop through ll other bodies and calculate the acceleration due to each
        foreach (GameObject body in otherBodies) {
            if (body == this.gameObject) {
                continue;
            }
            Vector3 otherPosition = body.transform.position;
            Vector3 direction = (otherPosition - position).normalized; // Direction vector from this planet to the other planet
            double distance = Vector3.Distance(otherPosition, position); // Distance between the two planets
            double scalarAcceleration = gravitationalConstant * body.GetComponent<N_bodyPlanet>().mass / (distance*distance);
            // Calculates effect of other body on this body's acceleration
            Vector3 bodyAcceleration = direction * (float)scalarAcceleration;

            acceleration += bodyAcceleration;

            Debug.Log("Acceleration: " + acceleration + ", F: " + scalarAcceleration + ", D: " + distance + ", Direction: " + direction);
        }

        // Update velocity
        velocity += acceleration * timeStep;
    }

    public void updatePosition(float timeStep) {
        transform.position += velocity * timeStep;
    }
}
