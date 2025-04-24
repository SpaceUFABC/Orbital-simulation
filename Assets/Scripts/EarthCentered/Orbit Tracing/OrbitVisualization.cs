using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class OrbitVisualization
{
    public Satellite satellite;
    public Earth earth;

    private Vector3[] drawOrbitLinePositions;
    private OrbitInfo drawOrbitInfo;

    private float gravitationalConstant;
    private float timeStep;
    private int maxSteps;
    private LineRenderer lineRenderer;

    // Create object of TwoBodyOrbitCalculation class for orbit calculation management
    private TwoBodyOrbitCalculation twoBodyOrbitCalculation;

    public OrbitVisualization(Satellite satellite, Earth earth, float gravitationalConstant, float timeStep, int maxSteps)
    {
        this.satellite = satellite;
        this.earth = earth;
        this.gravitationalConstant = gravitationalConstant;
        this.timeStep = timeStep;
        this.maxSteps = maxSteps;

        this.lineRenderer = satellite.GetComponent<LineRenderer>();

        twoBodyOrbitCalculation = new TwoBodyOrbitCalculation(satellite, earth, gravitationalConstant, timeStep, maxSteps);
    }
    
    
    public OrbitInfo CalculateOrbit()
    {
        MomentaryOrbitalElements[] momentaryOrbitalElementsArray = twoBodyOrbitCalculation.CalculateOrbit();

        // Calculate the orbital elements using the TwoBodyOrbitCalculation class
        OrbitalElements orbitalElements = twoBodyOrbitCalculation.CalculateGlobalOrbitalElements(momentaryOrbitalElementsArray);

        OrbitInfo orbitInfo = new OrbitInfo
        {
            orbitalElements = orbitalElements,
            momentaryOrbitalElementsArray = momentaryOrbitalElementsArray
        };

        return orbitInfo;
    }

    public void UpdateValues()
    {
        // Calculate the orbit and return complete orbit information
        OrbitInfo orbitInfo = CalculateOrbit();

        // Set the positions of the line renderer to the calculated positions
        Vector3[] positions = new Vector3[orbitInfo.momentaryOrbitalElementsArray.Length];
        for (int j = 0; j < orbitInfo.momentaryOrbitalElementsArray.Length; j++)
        {
            positions[j] = orbitInfo.momentaryOrbitalElementsArray[j].position;
        }

        drawOrbitLinePositions = positions; // Store the positions in buffer for draw call
        drawOrbitInfo = orbitInfo; // Store the complete orbit information for draw call
    }

    public void UpdateDraw()
    {
        DrawOrbitLines(drawOrbitLinePositions);
        DrawOrbitUI(drawOrbitInfo); // Call to draw the orbit UI with the calculated orbit information
    }

    public void DrawOrbitLines(Vector3[] positions)
    {
        if (positions == null || positions.Length == 0)
        {
            Debug.LogWarning("No positions to draw the orbit line.");
            return;
        }
        else
        {
            // Set the number of positions in the line renderer
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
        lineRenderer.enabled = true; // Enable the line renderer to visualize the orbit
        }
    }

    public void DrawOrbitUI(OrbitInfo orbitInfo)
    {
        // Draw the orbit UI using the drawOrbitInfo data
        // This can be used to display information about the orbit, such as apoapsis, periapsis, etc.
        // Implement your UI drawing logic here using the drawOrbitInfo data

        GameObject orbitUI = satellite.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.gameObject.name == "OrbitUI")?.gameObject;

        orbitUI.GetComponent<RectTransform>().transform.position = earth.gameObject.transform.position;


        GameObject apoapsisUI = orbitUI.transform.Find("ApoapsisUI").gameObject;
        apoapsisUI.GetComponent<RectTransform>().transform.position = orbitInfo.orbitalElements.positionOfApoapsis;
        GameObject apoapsisValuePanel = apoapsisUI.transform.Find("value").gameObject;
        apoapsisValuePanel.GetComponent<TextMeshPro>().text = orbitInfo.orbitalElements.apoapsis.ToString("F2") + " km";
        //GameObject periapsisUI = orbitUI.transform.Find("PeriapsisUI").gameObject;
    }
}



