using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionOnCollision : MonoBehaviour
{
    public GameObject TriggerObject;
    public string targetScene;           // Specify the target scene name in the Inspector
    //public GameObject loadingScreen;     // (Optional) Assign a loading screen UI Canvas if desired
    private bool isSceneLoading = false; // To prevent multiple scene loads

    void Start() {
        if (TriggerObject == null)
        {
            Debug.LogWarning("Trigger box not assigned!"); // Warn if no collider is assigned
        }
    }

    void Update() {
        Debug.Log("Update Scene");
        if (isSceneLoading){
            Debug.LogWarning($"Attempting to load scene: {targetScene}");  // Log the scene that is being requested
            SceneManager.LoadScene(targetScene);
            Debug.LogWarning($"Scene {targetScene} loaded successfully.");  // Log after the scene has loaded (note that this will run before the new scene is fully displayed)
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.LogWarning("Trigger Activated");
        if (!isSceneLoading)
        {
            isSceneLoading = true; // Prevent multiple triggers
        }
    }
}
