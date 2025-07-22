using UnityEngine;
using TMPro;

public class TrackingSphere : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] private float baseSpeed = 20f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float trackingDistance = 10f;
    [SerializeField] private float requiredTrackingTime = 5f;
    [SerializeField] private Color completedColor = Color.green;
    [SerializeField] private Transform centerPoint;
    
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private TextMeshProUGUI timeDisplayText;
    
    private float timer = 0f;
    private bool isCompleted = false;
    private float currentSpeed;
    private Renderer sphereRenderer;
    
    private void Start()
    {
        // If no center point is assigned, use the sphere's initial position
        if (centerPoint == null)
        {
            GameObject centerObj = new GameObject("CenterPoint");
            centerObj.transform.position = transform.position;
            centerPoint = centerObj.transform;
        }
        
        // If no camera is assigned, try to find the main camera
        if (playerCamera == null)
            playerCamera = Camera.main;
            
        sphereRenderer = GetComponent<Renderer>();
        currentSpeed = baseSpeed;
        
        // Initialize time display
        if (timeDisplayText != null)
        {
            timeDisplayText.gameObject.SetActive(false); // Disabled by default
        }
        UpdateTimeDisplay();
    }
    
    private void Update()
    {
        if (isCompleted) return;
        
        // Move sphere in circle
        transform.RotateAround(centerPoint.position, Vector3.up, currentSpeed * Time.deltaTime);
        
        // Check if player is tracking the sphere with raycast
        if (IsPlayerTrackingSphere())
        {
            // Enable time display when tracking starts
            if (timeDisplayText != null && !timeDisplayText.gameObject.activeInHierarchy)
            {
                timeDisplayText.gameObject.SetActive(true);
            }
            
            timer += Time.deltaTime;
            
            // Increase speed as time progresses (making it harder)
            float progress = timer / requiredTrackingTime;
            currentSpeed = Mathf.Lerp(baseSpeed, maxSpeed, progress);
            
            // Visual feedback - change color based on progress
            if (sphereRenderer != null)
            {
                Color progressColor = Color.Lerp(Color.white, completedColor, progress);
                sphereRenderer.material.color = progressColor;
            }
            
            // Update time display
            UpdateTimeDisplay();
            
            // Check if puzzle is completed
            if (timer >= requiredTrackingTime)
            {
                CompletePuzzle();
            }
        }
        else
        {
            // Reset if player loses track
            timer = 0f;
            currentSpeed = baseSpeed;
            
            // Reset color
            if (sphereRenderer != null)
                sphereRenderer.material.color = Color.white;
            
            // Disable time display when not tracking
            if (timeDisplayText != null)
            {
                timeDisplayText.gameObject.SetActive(false);
            }
                
            // Update time display
            UpdateTimeDisplay();
        }
    }
    
    private void UpdateTimeDisplay()
    {
        if (timeDisplayText != null)
        {
            float remainingTime = Mathf.Max(0f, requiredTrackingTime - timer);
            timeDisplayText.text = $"{remainingTime:F1}s";
            
            // Change text color based on progress
            if (timer > 0)
            {
                float progress = timer / requiredTrackingTime;
                timeDisplayText.color = Color.Lerp(Color.white, completedColor, progress);
            }
            else
            {
                timeDisplayText.color = Color.white;
            }
        }
    }
    
    private bool IsPlayerTrackingSphere()
    {
        if (playerCamera == null) return false;
        
        // Cast ray from camera center
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, trackingDistance))
        {
            // Check if the ray hit this sphere
            if (hit.collider.gameObject == gameObject)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private void CompletePuzzle()
    {
        isCompleted = true;
        
        // Set final color
        if (sphereRenderer != null)
            sphereRenderer.material.color = completedColor;
        
        // Clear the time display
        if (timeDisplayText != null)
        {
            timeDisplayText.text = "";
        }
        
        // Complete the puzzle
        PuzzleManager.CompletePuzzle(2);
        
        Debug.Log("Puzzle 2 completed: La Mirada del Observador");
    }
    

    
    // Optional: Visualize the raycast in the scene view for debugging
    private void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.red;
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Gizmos.DrawRay(ray.origin, ray.direction * trackingDistance);
        }
    }
} 