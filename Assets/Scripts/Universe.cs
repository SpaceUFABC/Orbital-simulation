using UnityEngine;

public static class Universe
{
    public const double gravitationalConstant = 6.67430e-11;
    public const float timeStep = 0.01f;
    public const float scale = 1000;// Scale is realScale / SimulationScale, so that we can use the real gravitational constant
}
