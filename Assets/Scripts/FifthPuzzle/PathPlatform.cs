using UnityEngine;

public class PathPlatform : MonoBehaviour
{
    private int row;
    private int col;
    private PathPuzzleManager puzzleManager;
    
    public void Initialize(int row, int col, PathPuzzleManager manager)
    {
        this.row = row;
        this.col = col;
        this.puzzleManager = manager;
        
        // Add trigger collider if not present
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider>();
        }
        collider.isTrigger = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            puzzleManager.OnPlatformStepped(row, col);
        }
    }
    
    // Gizmos for visual debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
} 