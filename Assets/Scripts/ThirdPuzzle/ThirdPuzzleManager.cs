using UnityEngine;

public class ThirdPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] private DraggableCube[] draggableCubes;
    
    private bool puzzleCompleted = false;
    
    private void Update()
    {
        if (!puzzleCompleted)
        {
            CheckPuzzleCompletion();
        }
    }
    
    private void CheckPuzzleCompletion()
    {
        int placedCubes = 0;
        
        foreach (DraggableCube cube in draggableCubes)
        {
            if (cube.IsPlaced())
            {
                placedCubes++;
            }
        }
        
        if (placedCubes == draggableCubes.Length)
        {
            CompletePuzzle();
        }
    }
    
    private void CompletePuzzle()
    {
        puzzleCompleted = true;
        PuzzleManager.CompletePuzzle(3);
        Debug.Log("Third puzzle completed: Arte vs. Artesanía");
    }
    
    public void ResetPuzzle()
    {
        puzzleCompleted = false;
        foreach (DraggableCube cube in draggableCubes)
        {
            cube.ResetCube();
        }
    }
} 