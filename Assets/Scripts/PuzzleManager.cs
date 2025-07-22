using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }
    public int puzzlesCompleted = 0;
    public GameObject exitZone;
    public TMP_Text puzzleProgressText; // Assign in Inspector
    public TMP_Text messageDisplayText; // Assign in Inspector
    public Color completionColor = Color.green; // Set in Inspector
    
    [Header("Level Objects")]
    public GameObject[] levelObjects; // 5 puzzle level objects (FirstPuzzle, SecondPuzzle, etc.)
    
    [Header("Narrative Settings")]
    public NarrativeCanvasManager narrativeManager;
    
    [Header("Audio")]
    public AudioSource audioSource; // Assign in Inspector
    
    private bool finalTextShown = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("PuzzleManager Start called");
        if (exitZone != null)
            exitZone.SetActive(false);

        // Initialize level objects (disable all except first)
        InitializeLevelObjects();
        
        UpdatePuzzleProgressText();
        StartCoroutine(StartNarrativeWithDelay());
    }
    
    private void Update()
    {
        // Check for R key restart when final text is shown
        if (finalTextShown && Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R key pressed - restarting scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private IEnumerator StartNarrativeWithDelay()
    {
        // Wait until NarrativeCanvasManager is ready
        while (NarrativeCanvasManager.Instance == null)
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(3f);

        NarrativeCanvasManager.Instance.StartTextRoutine(
            new[] { "ACTO 1", "EL ARTE SUSURRA SECRETOS A QUIEN SE ACERCA CON PACIENCIA." },
            new[] { "write", "fade" },
            new[] { "fade", "fade" },
            new[] { 0.8f, 4f } // 1 second for the first, 2 seconds for the second
        );
    }

    public static void CompletePuzzle(int puzzleNumber)
    {
        if (Instance == null) return;

        // Prevent completion if already at max (5 puzzles)
        if (Instance.puzzlesCompleted >= 5)
        {
            Debug.Log("All puzzles already completed!");
            return;
        }

        Instance.puzzlesCompleted++;
        // Note: Puzzle texts remain active - they are never deactivated

        Instance.UpdatePuzzleProgressText();
        
        // Play completion sound
        if (Instance.audioSource != null)
        {
            Instance.audioSource.Play();
        }
        
        // Activate next level object
        Instance.ActivateLevelObject(puzzleNumber);
        
        // Show narrative text for next act (only if not the last puzzle)
        if (Instance.puzzlesCompleted < 5)
        {
            Instance.StartCoroutine(Instance.ShowNextActText(puzzleNumber));
        }
        
        if (Instance.puzzlesCompleted >= 5)
        {
            Instance.ShowFinalText();
        }
        else if (Instance.exitZone != null)
        {
            Instance.exitZone.SetActive(true);
        }
        
        // Handle completion text display
        if (Instance.messageDisplayText != null)
        {
            Instance.StartCoroutine(Instance.ShowCompletionText());
        }
    }

    private void UpdatePuzzleProgressText()
    {
        if (puzzleProgressText != null)
        {
            puzzleProgressText.text = $"{puzzlesCompleted}/5";
        }
    }
    
    private IEnumerator ShowCompletionText()
    {
        // Enable the text component
        messageDisplayText.gameObject.SetActive(true);
        messageDisplayText.text = "COMPLETADO";
        messageDisplayText.color = completionColor;
        
        // Wait for 2 seconds to show completion message
        yield return new WaitForSeconds(2f);
        
        // Clear the text and disable the component
        messageDisplayText.text = "";
        messageDisplayText.gameObject.SetActive(false);
    }
    
    private void InitializeLevelObjects()
    {
        if (levelObjects != null)
        {
            for (int i = 0; i < levelObjects.Length; i++)
            {
                if (levelObjects[i] != null)
                {
                    // Enable only the first level object, disable the rest
                    levelObjects[i].SetActive(i == 0);
                }
            }
        }
    }
    
    private void ActivateLevelObject(int completedPuzzleNumber)
    {
        // Activate the next level object (puzzle 1 completes -> activate level 2 object)
        // Note: Previous level objects remain active - they are never deactivated
        int nextLevelIndex = completedPuzzleNumber; // 1-based to 0-based conversion
        if (levelObjects != null && nextLevelIndex < levelObjects.Length)
        {
            if (levelObjects[nextLevelIndex] != null)
            {
                levelObjects[nextLevelIndex].SetActive(true);
                Debug.Log($"Activated level object {nextLevelIndex + 1} (previous levels remain active)");
            }
        }
    }
    
    private IEnumerator ShowNextActText(int completedPuzzleNumber)
    {
        // Wait 4 seconds after puzzle completion
        yield return new WaitForSeconds(4f);
        
        if (narrativeManager != null)
        {
            string[] actTexts = GetActTexts();
            int nextActIndex = completedPuzzleNumber; // 0-based index for next act
            
            if (nextActIndex < actTexts.Length)
            {
                narrativeManager.StartTextRoutine(
                    new[] { $"ACTO {nextActIndex + 1}", actTexts[nextActIndex] },
                    new[] { "write", "fade" },
                    new[] { "fade", "fade" },
                    new[] { 0.8f, 4f }
                );
            }
        }
    }
    
    private void ShowFinalText()
    {
        if (narrativeManager != null)
        {
            narrativeManager.StartTextRoutine(
                new[] { "CORREDOR", "PORQUE YO, QUE TE QUIERO, SOLO TE PIENSO\n(R RESTART)" },
                new[] { "write", "fade" },
                new[] { "fade", "fade" },
                new[] { 0.8f, 4f },
                true // Special parameter to not fade out
            );
            
            // Enable R key restart functionality
            finalTextShown = true;
            Debug.Log("Final text shown - R key restart enabled");
        }
    }
    
    private string[] GetActTexts()
    {
        return new string[]
        {
            "EL ARTE SUSURRA SECRETOS A QUIEN SE ACERCA CON PACIENCIA.",
            "LA CONTEMPLACIÓN VERDADERA EXIGE LA DEVOCIÓN DEL TIEMPO Y LA CONSTANCIA.",
            "CADA FORMA BUSCA SU DESTINO ENTRE LA UTILIDAD Y LA BELLEZA PURA.",
            "EL SENDERO DEL SABER ESTÁ SEMBRADO DE VERDADES QUE HIEREN AL DISTRAÍDO.",
            "EL CAMINO HACIA LA VERDAD SIGUE UN ORDEN QUE SOLO LA INTUICIÓN COMPRENDE."
        };
    }
}