using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3Trigger : MonoBehaviour
{
    public string levelName = "Level3";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(levelName);
        }
    }
}
