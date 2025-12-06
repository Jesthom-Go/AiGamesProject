using UnityEngine;
using UnityEngine.SceneManagement;


public class Diamond : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerNear = false;

    private void Update()
    {
        if (playerNear && Input.GetKeyDown(interactKey))
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        Debug.Log(" YOU WIN!");
        
        SceneManager.LoadScene("Launcher");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNear = false;
    }
}
