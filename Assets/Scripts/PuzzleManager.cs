using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }
    public int puzzlesCompleted = 0;
    public GameObject[] puzzleTexts; // Assign in Inspector
    public GameObject exitZone;
    public TMP_Text puzzleProgressText; // Assign in Inspector
    public TMP_Text messageDisplayText; // Assign in Inspector
    public Color completionColor = Color.green; // Set in Inspector

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

        UpdatePuzzleProgressText();
        StartCoroutine(StartNarrativeWithDelay());
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

        Instance.puzzlesCompleted++;
        if (Instance.puzzleTexts != null && puzzleNumber - 1 < Instance.puzzleTexts.Length)
            Instance.puzzleTexts[puzzleNumber - 1].SetActive(false);

        Instance.UpdatePuzzleProgressText();
        if (Instance.puzzlesCompleted >= 5 && Instance.exitZone != null)
            Instance.exitZone.SetActive(true);
        
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
}