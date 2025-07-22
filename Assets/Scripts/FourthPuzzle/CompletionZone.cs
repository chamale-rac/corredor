using UnityEngine;

public class CompletionZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Fourth puzzle completed: El Camino del Conocimiento");
            PuzzleManager.CompletePuzzle(4);
            
            // Optional: Disable this zone to prevent multiple completions
            gameObject.SetActive(false);
        }
    }
} 