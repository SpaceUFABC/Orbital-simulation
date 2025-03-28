using UnityEditor;
using UnityEngine;

public class TwoBodyObj : MonoBehaviour
{
    // Planet properties
    public double mass;
    public Vector3 initialVelocity;
    public float radius;
    public TwoBodyObj otherBody;
    public Vector3 velocity;
    public LineRenderer orbitLine;
    private Transform mesh;

    void Awake() { 
        orbitLine = GetComponent<LineRenderer>();

        // Apply initial velocity
        this.velocity = initialVelocity;

        updateClosestBody();
    }


    public void updateVelocity(Vector3 velocity) {
        this.velocity += velocity;
    }

    public void updatePosition(float timeStep) {
        transform.position += velocity * timeStep;
    }

    public void updateClosestBody() {
         // Find closest body
        TwoBodyObj closestBody = null;
        float closestDistance = float.MaxValue;
        foreach (TwoBodyObj body in FindObjectsByType<TwoBodyObj>(FindObjectsSortMode.None))
        {
            if (body == this)
            {
                continue;
            }

            float distance = Vector3.Distance(body.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBody = body;
            }
        }

        // Set other body to the closest body (this is the other body in the two-body system)
        otherBody = closestBody;
    }
}
