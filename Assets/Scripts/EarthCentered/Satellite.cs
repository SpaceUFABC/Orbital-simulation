using UnityEngine;

public class Satellite : MonoBehaviour
{
    // Planet properties
    public double mass;
    public Vector3 initialVelocity;
    public Vector3 velocity;
    public LineRenderer orbitLine;
    private Transform mesh;

    void Awake() { 
        orbitLine = GetComponent<LineRenderer>();

        // Apply initial velocity
        this.velocity = initialVelocity;
    }


    public void updateVelocity(Vector3 velocity) {
        this.velocity += velocity;
    }

    public void updatePosition(float timeStep) {
        transform.position += velocity * timeStep;
    }
}
