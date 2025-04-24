using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TwoBodyOrbitCalculation
{
    public Satellite satellite;
    public Earth earth;
    private float gravitationalConstant;
    private float timeStep;
    private LineRenderer lineRenderer;
    public int maxSteps;

    public TwoBodyOrbitCalculation(Satellite satellite, Earth earth, float gravitationalConstant, float timeStep, int maxSteps)
    {
        this.satellite = satellite;
        this.earth = earth;
        this.gravitationalConstant = gravitationalConstant;
        this.timeStep = timeStep;
        this.maxSteps = maxSteps;

        this.lineRenderer = satellite.GetComponent<LineRenderer>();
    }
    
    public MomentaryOrbitalElements[] CalculateOrbit()
    {
        // Definition of vector arrays to store positions, velocities, and momentary orbital elements
        // The size of the arrays is arbitrary and can be adjusted based on the maximum expected number of steps in the simulation
        Vector3[] positions = new Vector3[this.maxSteps];
        Vector3[] velocities = new Vector3[this.maxSteps];
        MomentaryOrbitalElements[] momentaryOrbitalElements = new MomentaryOrbitalElements[this.maxSteps];

        // Calculating values for initial step
        velocities[0] = satellite.velocity;
        positions[0] = satellite.transform.position;

        momentaryOrbitalElements[0].trueAnomaly = GetTrueAnomaly(positions[0], velocities[0]);
        momentaryOrbitalElements[0].radius = (positions[0] - earth.gameObject.transform.position).magnitude;
        momentaryOrbitalElements[0].tangentialVelocity = velocities[0].magnitude;
        momentaryOrbitalElements[0].position = positions[0];

        // Initialization of variables to be used in the loop
        int i = 0;
        float totalAnomalyChange = 0f;

        while (i + 1 < this.maxSteps)
        {
            // Calculate satellite acceleration and, using Euler's method, update the velocity and position
            Vector3 acceleration = GetAcceleration(positions[i]);
            velocities[i + 1] = velocities[i] + acceleration * timeStep;
            positions[i + 1] = positions[i] + velocities[i + 1] * timeStep;

            // Calculate the true anomaly
            float trueAnomaly = GetTrueAnomaly(positions[i + 1], velocities[i + 1]);

            float anomalyChange = Mathf.Abs(trueAnomaly - momentaryOrbitalElements[i].trueAnomaly);
            if (anomalyChange < -180f)
            {
                anomalyChange += 360f;
            }
            else if (anomalyChange > 180f)
            {
                anomalyChange -= 360f;
            }

            totalAnomalyChange += anomalyChange;

            // Check if the orbit is complete (total anomaly change exceeds 360 degrees)
            if (totalAnomalyChange >= 360f)
            {
                break;
            }

            // Update the momentary orbital elements
            momentaryOrbitalElements[i + 1].trueAnomaly = trueAnomaly;
            momentaryOrbitalElements[i + 1].radius = (positions[i + 1] - earth.gameObject.transform.position).magnitude;
            momentaryOrbitalElements[i + 1].tangentialVelocity = velocities[i + 1].magnitude;
            momentaryOrbitalElements[i + 1].position = positions[i + 1];

            i += 1;
        }

        Debug.Log(momentaryOrbitalElements.Length);

        return momentaryOrbitalElements;
    }

    public OrbitalElements CalculateGlobalOrbitalElements(MomentaryOrbitalElements[] momentaryOrbitalElements)
    {
        OrbitalElements orbitalElements = new OrbitalElements();

        // Find apoapsis and periapsis from the momentary orbital elements
        float[] radiuss = new float[momentaryOrbitalElements.Length];
        for (int j = 0; j < radiuss.Length; j++)
        {
            radiuss[j] = momentaryOrbitalElements[j].radius;
        }
        orbitalElements.apoapsis = radiuss.Max();
        orbitalElements.periapsis = radiuss.Min();
        orbitalElements.semiMajorAxis = (orbitalElements.apoapsis + orbitalElements.periapsis) / 2;
        orbitalElements.eccentricity = (orbitalElements.apoapsis - orbitalElements.periapsis) / (orbitalElements.apoapsis + orbitalElements.periapsis);
        orbitalElements.inclination = GetInclination(momentaryOrbitalElements.Last().position, momentaryOrbitalElements.Last().velocity);
        orbitalElements.longitudeOfAscendingNode = GetLongitudeOfAscendingNode(momentaryOrbitalElements.Last().position, momentaryOrbitalElements.Last().velocity);
        orbitalElements.argumentOfPeriapsis = GetArgumentOfPeriapsis(momentaryOrbitalElements.Last().position, momentaryOrbitalElements.Last().velocity);

        orbitalElements.positionOfApoapsis = momentaryOrbitalElements.FirstOrDefault(x => x.radius == orbitalElements.apoapsis).position;
        orbitalElements.positionOfPeriapsis = momentaryOrbitalElements.FirstOrDefault(x => x.radius == orbitalElements.periapsis).position;

        return orbitalElements;
    }

    private Vector3 GetAcceleration(Vector3 position)
    {
        Vector3 direction = earth.gameObject.transform.position - position;
        float distance = direction.magnitude;
        float scalarAcceleration = (float)(gravitationalConstant * earth.mass / Mathf.Pow(distance, 2));
        Vector3 acceleration = direction.normalized * scalarAcceleration;

        return acceleration;
    }

    private float GetTrueAnomaly(Vector3 position, Vector3 velocity)
    {
        // Position vector relative to the Earth's center
        Vector3 r = position - earth.gameObject.transform.position;
        Vector3 v = velocity;

        // Calculate the specific angular momentum vector
        Vector3 h = Vector3.Cross(r, v);

        // Calculate the eccentricity vector
        Vector3 e = (Vector3.Cross(v, h) / (float)(gravitationalConstant * earth.mass)) - r.normalized;

        // Normalize the eccentricity vector and position vector
        Vector3 eNormalized = e.normalized;
        Vector3 rNormalized = r.normalized;

        // Calculate the true anomaly using the dot product
        float trueAnomaly = Mathf.Acos(Vector3.Dot(eNormalized, rNormalized));

        // Adjust the true anomaly based on the direction of the cross product
        if (Vector3.Dot(Vector3.Cross(e, r), h) < 0)
        {
            trueAnomaly = 2 * Mathf.PI - trueAnomaly;
        }

        // Convert the true anomaly to degrees
        return trueAnomaly * Mathf.Rad2Deg;
    }

    private float GetInclination(Vector3 position, Vector3 velocity)
    {
        Vector3 r = position - earth.gameObject.transform.position;
        Vector3 v = velocity;

        // Calculate the specific angular momentum vector
        Vector3 h = Vector3.Cross(r, v);

        // Calculate the inclination
        float inclination = Mathf.Acos(h.z / h.magnitude) * Mathf.Rad2Deg; // Convert to degrees

        return inclination;
    }

    private float GetLongitudeOfAscendingNode(Vector3 position, Vector3 velocity)
    {
        Vector3 r = position - earth.gameObject.transform.position;
        Vector3 v = velocity;

        // Calculate the specific angular momentum vector
        Vector3 h = Vector3.Cross(r, v);

        // Calculate the node vector
        Vector3 n = new Vector3(-h.y, h.x, 0); // Node vector in the XY plane

        // Calculate the longitude of ascending node
        float longitudeOfAscendingNode = Mathf.Acos(n.x / n.magnitude) * Mathf.Rad2Deg; // Convert to degrees
        if (n.y < 0)
        {
            longitudeOfAscendingNode = 360 - longitudeOfAscendingNode; // Adjust for the correct quadrant
        }

        return longitudeOfAscendingNode;
    }

    private float GetArgumentOfPeriapsis(Vector3 position, Vector3 velocity)
    {
        Vector3 r = position - earth.gameObject.transform.position;
        Vector3 v = velocity;

        // Calculate the specific angular momentum vector
        Vector3 h = Vector3.Cross(r, v);

        // Calculate the eccentricity vector
        Vector3 e = Vector3.Cross(h, v) / (float)(gravitationalConstant * earth.mass) - r.normalized;

        // Calculate the node vector
        Vector3 n = new Vector3(-h.y, h.x, 0); // Node vector in the XY plane

        // Calculate the argument of periapsis
        float argumentOfPeriapsis = (Mathf.Acos(Vector3.Dot(n, e)) / (n.magnitude * e.magnitude)) * Mathf.Rad2Deg; // Convert to degrees
        if (e.y < 0)
        {
            argumentOfPeriapsis = 360 - argumentOfPeriapsis; // Adjust for the correct quadrant
        }

        return argumentOfPeriapsis;
    }
}



