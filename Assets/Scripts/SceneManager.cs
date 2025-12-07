using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader2D : MonoBehaviour
{
    [Header("Leave empty to load next scene by Build Index")]
    public string sceneToLoad = "";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
            return;
        }

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("No next scene found in Build Settings.");
            return;
        }

        SceneManager.LoadScene(nextIndex);
    }
}
