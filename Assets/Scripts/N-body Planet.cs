using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "N-bodyPlanet", menuName = "Scriptable Objects/N-bodyPlanet")]
public class N_bodyPlanet : MonoBehaviour
{
    // Planet properties
    // public double scale; // Lets just ignore this for now ;)

    public double mass;
    public Vector3 initialVelocity;
    public float radius;

    private Vector3 velocity;
    private Transform mesh;



    private GameObject[] otherBodies;

    void Awake() { 
        this.velocity = initialVelocity;
    }


    public void updateVelocity(Vector3 velocity) {
        this.velocity += velocity;
    }

    public void updatePosition(float timeStep) {
        transform.position += velocity * timeStep;
    }

    public void OnValidate()
    {
        mesh = transform.GetChild(0);
        mesh.localScale = Vector3.one * radius;
    }
}
