using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProximitySoundSphere : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] private bool isSingingSphere = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Color foundColor = Color.yellow;

    private bool isFound = false;

    private void Reset()
    {
        // Ensure the collider is set as trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFound) return;
        if (!other.CompareTag("Player")) return;

        if (isSingingSphere)
        {
            isFound = true;
            if (audioSource != null)
                audioSource.Play();

            var renderer = GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = foundColor;

            PuzzleManager.CompletePuzzle(1);
        }
    }
}