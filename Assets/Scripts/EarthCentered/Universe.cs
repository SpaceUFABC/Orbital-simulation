using UnityEngine;

public class DataManager : MonoBehaviour
{
    // Singleton instance
    public static DataManager Instance { get; private set; }

    // Shared variables
    public float gravitationalConstant = 0.01f; // Gravitational constant in m^3 kg^-1 s^-2

    void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
}