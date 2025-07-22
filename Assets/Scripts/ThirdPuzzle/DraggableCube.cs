using UnityEngine;

public class DraggableCube : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] private Color completedColor = Color.green;
    [SerializeField] private int cubeID = 0; // 0, 1, 2 for the three cubes
    
    private bool isPlaced = false;
    private bool isDragging = false;
    private Renderer cubeRenderer;
    private Vector3 originalPosition;
    private Camera playerCamera;
    private Rigidbody cubeRigidbody;
    
    private void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
        originalPosition = transform.position;
        playerCamera = Camera.main;
        
        // Add Rigidbody for collision detection during dragging
        cubeRigidbody = GetComponent<Rigidbody>();
        if (cubeRigidbody == null)
        {
            cubeRigidbody = gameObject.AddComponent<Rigidbody>();
        }
        
        // Configure Rigidbody for dragging
        cubeRigidbody.useGravity = false;
        cubeRigidbody.freezeRotation = true;
    }
    
    private void OnMouseDown()
    {
        if (!isPlaced)
        {
            isDragging = true;
        }
    }
    
    private void OnMouseDrag()
    {
        if (isDragging && !isPlaced)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = playerCamera.WorldToScreenPoint(transform.position).z;
            Vector3 worldPos = playerCamera.ScreenToWorldPoint(mousePos);
            
            // Use MovePosition to respect collisions during dragging
            Vector3 targetPos = new Vector3(worldPos.x, transform.position.y, worldPos.z);
            cubeRigidbody.MovePosition(targetPos);
        }
    }
    
    private void OnMouseUp()
    {
        if (isDragging && !isPlaced)
        {
            isDragging = false;
            CheckForCorrectPedestal();
        }
    }
    
    private void CheckForCorrectPedestal()
    {
        Pedestal[] pedestals = FindObjectsByType<Pedestal>(FindObjectsSortMode.None);
        
        foreach (Pedestal pedestal in pedestals)
        {
            if (pedestal.GetPedestalID() == cubeID)
            {
                float distance = Vector3.Distance(transform.position, pedestal.transform.position);
                if (distance < 2f)
                {
                    // Snap to pedestal with proper height calculation
                    Vector3 snapPos = pedestal.transform.position;
                    
                    // Calculate proper height to avoid intersection
                    float pedestalHeight = pedestal.transform.localScale.y;
                    float cubeHeight = transform.localScale.y;
                    float pedestalTop = pedestal.transform.position.y + (pedestalHeight / 2);
                    float cubeBottom = pedestalTop + (cubeHeight / 2);
                    
                    snapPos.y = cubeBottom;
                    transform.position = snapPos;
                    
                    // Mark as placed and change color
                    isPlaced = true;
                    cubeRenderer.material.color = completedColor;
                    
                    // Re-enable gravity when placed
                    cubeRigidbody.useGravity = true;
                    
                    Debug.Log($"Cube {cubeID} placed on correct pedestal!");
                    return;
                }
            }
        }
    }
    
    public bool IsPlaced()
    {
        return isPlaced;
    }
    
    public int GetCubeID()
    {
        return cubeID;
    }
    
    public void ResetCube()
    {
        isPlaced = false;
        isDragging = false;
        transform.position = originalPosition;
        cubeRenderer.material.color = Color.white;
        
        // Reset Rigidbody
        cubeRigidbody.useGravity = false;
        cubeRigidbody.velocity = Vector3.zero;
        cubeRigidbody.angularVelocity = Vector3.zero;
    }
} 