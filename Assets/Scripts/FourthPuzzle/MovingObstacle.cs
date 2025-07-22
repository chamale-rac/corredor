using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float maxX = 5f;
    [SerializeField] private float minX = -5f;
    
    [Header("Restart Settings")]
    [SerializeField] private Transform restartPos;
    
    private void Update()
    {
        // Move obstacle side to side
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        
        // Reverse direction when hitting boundaries
        if (transform.position.x >= maxX)
        {
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
            speed = -Mathf.Abs(speed); // Ensure negative speed
        }
        else if (transform.position.x <= minX)
        {
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
            speed = Mathf.Abs(speed); // Ensure positive speed
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit obstacle! Moving to restart position...");
            
            if (restartPos != null)
            {
                // Handle different player movement components
                CharacterController characterController = other.GetComponent<CharacterController>();
                Rigidbody rigidbody = other.GetComponent<Rigidbody>();
                
                if (characterController != null)
                {
                    // For CharacterController, we need to disable it temporarily
                    characterController.enabled = false;
                    other.transform.position = restartPos.position;
                    characterController.enabled = true;
                }
                else if (rigidbody != null)
                {
                    // For Rigidbody, set position and clear velocity
                    rigidbody.position = restartPos.position;
                    rigidbody.linearVelocity = Vector3.zero;
                    rigidbody.angularVelocity = Vector3.zero;
                }
                else
                {
                    // For regular Transform
                    other.transform.position = restartPos.position;
                }
                
                Debug.Log($"Player moved to restart position: {restartPos.position}");
            }
            else
            {
                Debug.LogWarning("RestartPos not assigned! Please assign a restart position in the Inspector.");
            }
        }
    }
    
    // Optional: Visualize movement bounds in scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 minPos = transform.position;
        Vector3 maxPos = transform.position;
        minPos.x = minX;
        maxPos.x = maxX;
        Gizmos.DrawLine(minPos, maxPos);
        Gizmos.DrawWireSphere(minPos, 0.5f);
        Gizmos.DrawWireSphere(maxPos, 0.5f);
    }
} 