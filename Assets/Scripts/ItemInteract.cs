using UnityEngine;
using TMPro;

public class ItemInteraction : MonoBehaviour
{
    [Header("Trigger that becomes available after pickup")]
    public GameObject level3Trigger;

    [Header("UI Prompt (Press E)")]
    public TMP_Text interactPrompt;

    [Header("UI Message shown after picking up")]
    public TMP_Text pickupMessage;
    public string messageText = "You found the required item!";

    private bool playerIsNear = false;
    private bool pickedUp = false;

    private void Start()
    {
        if (interactPrompt != null)
            interactPrompt.gameObject.SetActive(false);

        if (pickupMessage != null)
            pickupMessage.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (playerIsNear && !pickedUp)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PickupItem();
            }
        }
    }

    private void PickupItem()
    {
        pickedUp = true;

        if (interactPrompt != null)
            interactPrompt.gameObject.SetActive(false);

        if (pickupMessage != null)
        {
            pickupMessage.text = messageText;
            pickupMessage.gameObject.SetActive(true);
        }

        if (level3Trigger != null)
            level3Trigger.SetActive(true);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;

            if (!pickedUp && interactPrompt != null)
            {
                interactPrompt.text = "Press E to interact";
                interactPrompt.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;

            if (interactPrompt != null)
                interactPrompt.gameObject.SetActive(false);
        }
    }
}
