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
    public float coordSystemScale;

    private Vector3 velocity;
    private Transform mesh;
    private N_bodyPlanet closestBody;




    private GameObject[] otherBodies;

    void Awake() { 
        this.velocity = initialVelocity;

        // Find closest body
        closestBody = null;
        float closestDistance = float.MaxValue;
        foreach (N_bodyPlanet body in FindObjectsByType<N_bodyPlanet>(FindObjectsSortMode.None))
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

    public void OnDrawGizmos()
    {
        drawCoordinateSystem(coordSystemScale);
    }

    // Draws orbital coordinate system for planet, considering that it's orbiting the closest body
    void drawCoordinateSystem(float scale)
    {
        //Draw velocity vector
        Handles.color = Color.cyan;
        Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(velocity), velocity.magnitude/(10*scale), EventType.Repaint);

        //Draw XYZ axis
        Vector3 r = closestBody.gameObject.transform.position - transform.position;
        Handles.color = Color.red;
        Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(r), scale, EventType.Repaint);
        Handles.color = Color.green;
        Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(velocity), scale, EventType.Repaint);
        Handles.color = Color.blue;
        Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(Vector3.Cross(r, velocity)), scale, EventType.Repaint);
    }
}
