using UnityEngine;
using UnityEngine.SceneManagement;

public class FallDetector : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private bool reloadOnTrigger = true;
    [SerializeField] private string playerTag = "Player";
    
    [Header("Scene Reload Settings")]
    [SerializeField] private float reloadDelay = 0.5f;
    [SerializeField] private bool useCurrentScene = true;
    [SerializeField] private string targetSceneName = "";
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    [SerializeField] private bool showTriggerZone = true;
    
    private bool hasTriggered = false;
    private float reloadTimer = 0f;
    
    void Start()
    {
        // Validate scene settings
        if (!useCurrentScene && string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("FallDetector: Target scene name is empty! Please set a scene name or enable 'Use Current Scene'.");
            reloadOnTrigger = false;
        }
        
        // Ensure this object has a Collider set as trigger
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("FallDetector: No Collider found! Please add a Collider component and set it as trigger.");
            reloadOnTrigger = false;
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning("FallDetector: Collider is not set as trigger! Setting it as trigger automatically.");
            col.isTrigger = true;
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"FallDetector initialized. Player tag: {playerTag}, Reload delay: {reloadDelay}");
        }
    }
    
    void Update()
    {
        if (!reloadOnTrigger || !hasTriggered)
            return;
        
        // Count down the reload delay
        reloadTimer += Time.deltaTime;
        
        if (reloadTimer >= reloadDelay)
        {
            ReloadScene();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!reloadOnTrigger || hasTriggered)
            return;
        
        // Check if the entering object is the player
        if (other.CompareTag(playerTag))
        {
            hasTriggered = true;
            reloadTimer = 0f;
            
            if (showDebugInfo)
            {
                Debug.Log($"FallDetector: Player entered trigger zone! Player position: {other.transform.position}");
            }
        }
    }
    
    private void ReloadScene()
    {
        if (showDebugInfo)
        {
            Debug.Log("FallDetector: Reloading scene...");
        }
        
        if (useCurrentScene)
        {
            // Reload the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            // Load the specified scene
            SceneManager.LoadScene(targetSceneName);
        }
    }
    
    // Public methods to modify settings at runtime
    public void SetPlayerTag(string newTag)
    {
        playerTag = newTag;
        if (showDebugInfo)
        {
            Debug.Log($"FallDetector: Player tag updated to {newTag}");
        }
    }
    
    public void SetReloadDelay(float newDelay)
    {
        reloadDelay = Mathf.Max(0f, newDelay);
        if (showDebugInfo)
        {
            Debug.Log($"FallDetector: Reload delay updated to {reloadDelay}");
        }
    }
    
    public void EnableTriggerDetection(bool enable)
    {
        reloadOnTrigger = enable;
        if (showDebugInfo)
        {
            Debug.Log($"FallDetector: Trigger detection {(enable ? "enabled" : "disabled")}");
        }
    }
    
    public void ResetTriggerState()
    {
        hasTriggered = false;
        reloadTimer = 0f;
        if (showDebugInfo)
        {
            Debug.Log("FallDetector: Trigger state reset");
        }
    }
    
    // Gizmos for debugging
    void OnDrawGizmosSelected()
    {
        if (showTriggerZone)
        {
            // Draw the trigger zone
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
                
                if (col is BoxCollider boxCol)
                {
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawCube(boxCol.center, boxCol.size);
                }
                else if (col is SphereCollider sphereCol)
                {
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawSphere(sphereCol.center, sphereCol.radius);
                }
                else if (col is CapsuleCollider capsuleCol)
                {
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawCube(capsuleCol.center, new Vector3(capsuleCol.radius * 2, capsuleCol.height, capsuleCol.radius * 2));
                }
                else
                {
                    // Generic collider bounds
                    Gizmos.DrawWireCube(transform.position, col.bounds.size);
                }
            }
        }
    }
} 