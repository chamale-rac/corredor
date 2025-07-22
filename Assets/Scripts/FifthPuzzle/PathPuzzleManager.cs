using UnityEngine;
using System.Collections.Generic;

public class PathPuzzleManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 4;
    [SerializeField] private float platformSize = 2f;
    [SerializeField] private float platformHeight = 0.5f;
    [SerializeField] private float spacing = 0.5f;
    
    [Header("Path Settings")]
    [SerializeField] private List<Vector2Int> correctPath = new List<Vector2Int>();
    [SerializeField] private Color correctPathColor = Color.green;
    [SerializeField] private Color wrongPathColor = Color.red;
    [SerializeField] private Color defaultColor = Color.gray;
    
    [Header("Restart Settings")]
    [SerializeField] private Transform restartPos;
    
    [Header("Visual Settings")]
    [SerializeField] private bool showPathPreview = true;
    [SerializeField] private Light indicatorLight;
    [SerializeField] private float lightHeight = 3f;
    
    [Header("References")]
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private CompletionZone completionZone;
    
    private GameObject[,] gridPlatforms;
    private int currentStep = 0;
    private bool puzzleCompleted = false;
    
    private void Start()
    {
        CreateGrid();
        SetupCorrectPath();
        UpdatePathVisualization();
    }
    
    private void Update()
    {
        if (!puzzleCompleted && indicatorLight != null)
        {
            UpdateIndicatorLight();
        }
    }
    
    private void CreateGrid()
    {
        gridPlatforms = new GameObject[rows, columns];
        
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                CreatePlatform(row, col);
            }
        }
    }
    
    private void CreatePlatform(int row, int col)
    {
        Vector3 position = CalculatePlatformPosition(row, col);
        
        GameObject platform;
        if (platformPrefab != null)
        {
            platform = Instantiate(platformPrefab, position, Quaternion.identity, transform);
        }
        else
        {
            // Create default cube platform
            platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.transform.SetParent(transform);
            platform.transform.position = position;
            platform.transform.localScale = new Vector3(platformSize, platformHeight, platformSize);
        }
        
        // Add platform script
        PathPlatform platformScript = platform.AddComponent<PathPlatform>();
        platformScript.Initialize(row, col, this);
        
        gridPlatforms[row, col] = platform;
    }
    
    private Vector3 CalculatePlatformPosition(int row, int col)
    {
        float x = col * (platformSize + spacing);
        float z = row * (platformSize + spacing);
        Vector3 localPosition = new Vector3(x, platformHeight / 2, z);
        return transform.position + localPosition;
    }
    
    private void SetupCorrectPath()
    {
        // Test path for 7x5 grid: Start at bottom, zigzag to top
        if (correctPath.Count == 0)
        {
            // Start from bottom-left (row 0, col 0)
            correctPath.Add(new Vector2Int(0, 0)); // Start
            
            // Zigzag path to the top
            correctPath.Add(new Vector2Int(0, 1));
            correctPath.Add(new Vector2Int(1, 1));
            correctPath.Add(new Vector2Int(1, 2));
            correctPath.Add(new Vector2Int(2, 2));
            correctPath.Add(new Vector2Int(2, 3));
            correctPath.Add(new Vector2Int(3, 3));
            correctPath.Add(new Vector2Int(3, 4));
            correctPath.Add(new Vector2Int(4, 4));
            correctPath.Add(new Vector2Int(4, 3));
            correctPath.Add(new Vector2Int(5, 3));
            correctPath.Add(new Vector2Int(5, 2));
            correctPath.Add(new Vector2Int(6, 2));
            correctPath.Add(new Vector2Int(6, 1));
            correctPath.Add(new Vector2Int(6, 0)); // Finish at bottom-right of last row
            correctPath.Add(new Vector2Int(7, 0));
        }
    }
    
    private void UpdatePathVisualization()
    {
        if (!showPathPreview) return;
        
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject platform = gridPlatforms[row, col];
                if (platform != null)
                {
                    Renderer renderer = platform.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        Vector2Int pos = new Vector2Int(row, col);
                        if (correctPath.Contains(pos))
                        {
                            int pathIndex = correctPath.IndexOf(pos);
                            if (pathIndex == currentStep)
                            {
                                renderer.material.color = correctPathColor;
                            }
                            else if (pathIndex < currentStep)
                            {
                                renderer.material.color = defaultColor; // Completed steps
                            }
                            else
                            {
                                renderer.material.color = wrongPathColor; // Future steps (hidden)
                            }
                        }
                        else
                        {
                            renderer.material.color = wrongPathColor;
                        }
                    }
                }
            }
        }
    }
    
    private void UpdateIndicatorLight()
    {
        if (currentStep < correctPath.Count)
        {
            Vector2Int currentPos = correctPath[currentStep];
            GameObject currentPlatform = gridPlatforms[currentPos.x, currentPos.y];
            
            if (currentPlatform != null)
            {
                Vector3 lightPosition = currentPlatform.transform.position + Vector3.up * lightHeight;
                indicatorLight.transform.position = lightPosition;
            }
        }
    }
    
    public void OnPlatformStepped(int row, int col)
    {
        if (puzzleCompleted) return;
        
        Vector2Int steppedPos = new Vector2Int(row, col);
        
        if (currentStep < correctPath.Count && correctPath[currentStep] == steppedPos)
        {
            // Correct step
            currentStep++;
            Debug.Log($"Correct step! Progress: {currentStep}/{correctPath.Count}");
            
            if (currentStep >= correctPath.Count)
            {
                CompletePuzzle();
            }
        }
        else
        {
            // Wrong step - restart player position
            currentStep = 0;
            Debug.Log("Wrong step! Restarting player position.");
            
            // Find the player and restart them
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && restartPos != null)
            {
                RestartPlayer(player);
            }
        }
        
        UpdatePathVisualization();
    }
    
    private void RestartPlayer(GameObject player)
    {
        // Handle different player movement components (same as MovingObstacle)
        CharacterController characterController = player.GetComponent<CharacterController>();
        Rigidbody rigidbody = player.GetComponent<Rigidbody>();
        
        if (characterController != null)
        {
            // For CharacterController, we need to disable it temporarily
            characterController.enabled = false;
            player.transform.position = restartPos.position;
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
            player.transform.position = restartPos.position;
        }
        
        Debug.Log($"Player moved to restart position: {restartPos.position}");
    }
    
    private void CompletePuzzle()
    {
        puzzleCompleted = true;
        Debug.Log("Fifth puzzle completed: La Elección Final");
        PuzzleManager.CompletePuzzle(5);
        
        // Activate completion zone
        if (completionZone != null)
        {
            completionZone.gameObject.SetActive(true);
        }
    }
    
    // Editor methods for path editing
    public void AddPathStep(int row, int col)
    {
        Vector2Int step = new Vector2Int(row, col);
        if (!correctPath.Contains(step))
        {
            correctPath.Add(step);
            UpdatePathVisualization();
        }
    }
    
    public void RemovePathStep(int row, int col)
    {
        Vector2Int step = new Vector2Int(row, col);
        correctPath.Remove(step);
        UpdatePathVisualization();
    }
    
    public void ClearPath()
    {
        correctPath.Clear();
        UpdatePathVisualization();
    }
    
    // Gizmos for visual debugging
    private void OnDrawGizmos()
    {
        if (!showPathPreview) return;
        
        // Draw grid
        Gizmos.color = Color.red;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 pos = CalculatePlatformPosition(row, col);
                Gizmos.DrawWireCube(pos, new Vector3(platformSize, platformHeight, platformSize));
            }
        }
        
        // Draw path
        Gizmos.color = correctPathColor;
        for (int i = 0; i < correctPath.Count - 1; i++)
        {
            Vector3 start = CalculatePlatformPosition(correctPath[i].x, correctPath[i].y);
            Vector3 end = CalculatePlatformPosition(correctPath[i + 1].x, correctPath[i + 1].y);
            Gizmos.DrawLine(start, end);
        }
        
        // Draw center point to show where the grid starts
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
} 