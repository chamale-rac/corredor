using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private GameObject targetObject;
    
    [Header("Follow Settings")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;
    [SerializeField] private bool followZ = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    private Vector3 originalDistance;
    private bool isInitialized = false;
    
    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("FollowObject: No target object assigned! Please assign a target in the inspector.");
            return;
        }
        
        // Calculate and store the original distance between this object and the target
        originalDistance = transform.position - targetObject.transform.position;
        isInitialized = true;
        
        if (showDebugInfo)
        {
            Debug.Log($"FollowObject initialized. Original distance: {originalDistance}");
        }
    }
    
    void Update()
    {
        if (!isInitialized || targetObject == null)
            return;
        
        // Get the target's current position
        Vector3 targetPosition = targetObject.transform.position;
        
        // Calculate the new position based on which axes should be followed
        Vector3 newPosition = transform.position;
        
        if (followX)
            newPosition.x = targetPosition.x + originalDistance.x;
        
        if (followY)
            newPosition.y = targetPosition.y + originalDistance.y;
        
        if (followZ)
            newPosition.z = targetPosition.z + originalDistance.z;
        
        // Apply the new position
        transform.position = newPosition;
    }
    
    // Public methods to change follow settings at runtime
    public void SetFollowX(bool follow)
    {
        followX = follow;
    }
    
    public void SetFollowY(bool follow)
    {
        followY = follow;
    }
    
    public void SetFollowZ(bool follow)
    {
        followZ = follow;
    }
    
    public void SetTarget(GameObject newTarget)
    {
        targetObject = newTarget;
        if (newTarget != null)
        {
            originalDistance = transform.position - targetObject.transform.position;
            isInitialized = true;
        }
    }
    
    // Method to recalculate the original distance (useful if you move the objects manually)
    public void RecalculateDistance()
    {
        if (targetObject != null)
        {
            originalDistance = transform.position - targetObject.transform.position;
            if (showDebugInfo)
            {
                Debug.Log($"FollowObject: Distance recalculated. New distance: {originalDistance}");
            }
        }
    }
    
    // Gizmos for debugging
    void OnDrawGizmosSelected()
    {
        if (showDebugInfo && targetObject != null && isInitialized)
        {
            // Draw a line from this object to the target
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetObject.transform.position);
            
            // Draw the original distance vector
            Gizmos.color = Color.red;
            Vector3 targetPos = targetObject.transform.position;
            Gizmos.DrawRay(targetPos, originalDistance);
            
            // Draw spheres at the positions
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetPos, 0.1f);
        }
    }
} 