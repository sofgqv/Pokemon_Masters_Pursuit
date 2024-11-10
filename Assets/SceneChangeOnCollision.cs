using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionOnCollision : MonoBehaviour
{
    public GameObject TriggerObject;
    public string targetScene;           // Specify the target scene name in the Inspector
    public GameObject loadingScreen;     // Assign a loading screen UI Canvas in the Inspector
    public GameObject miniMap;
    private bool isSceneLoading = false; // To prevent multiple scene loads

    void Start() {
        if (TriggerObject == null) {
            Debug.LogWarning("Trigger box not assigned!"); // Warn if no collider is assigned
        }
        
        if (loadingScreen != null) {
            loadingScreen.SetActive(false); // Hide the loading screen initially
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!isSceneLoading && other.gameObject == TriggerObject) { // Check that the collider matches
            Debug.LogWarning("Trigger Activated");
            isSceneLoading = true;      // Prevent multiple triggers
            StartCoroutine(LoadSceneAsync());  // Start the asynchronous loading coroutine
        }
        miniMap.SetActive(false);
    }

    private IEnumerator LoadSceneAsync() {
        // Display the loading screen and wait briefly for it to render
        if (loadingScreen != null) {
            loadingScreen.SetActive(true);
            yield return new WaitForSeconds(0.1f);  // Ensure UI updates before loading
        }

         yield return new WaitForSeconds(1f);

        // Start loading the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        asyncLoad.allowSceneActivation = false;

        // Debug to track loading progress
        while (!asyncLoad.isDone) {
            Debug.Log($"Loading progress: {asyncLoad.progress * 100}%");

            // When loading is almost done, activate the new scene
            if (asyncLoad.progress >= 0.9f) {
                Debug.Log("Scene loading complete. Activating scene...");
                asyncLoad.allowSceneActivation = true;
            }
            yield return null; // Continue to next frame
        }

        // Hide the loading screen after the scene has loaded
        if (loadingScreen != null) {
            loadingScreen.SetActive(false);
        }
    }
}
