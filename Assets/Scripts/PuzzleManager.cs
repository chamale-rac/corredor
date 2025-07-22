using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }
    public int puzzlesCompleted = 0;
    public GameObject[] puzzleTexts; // Assign in Inspector
    public GameObject exitZone;

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

        StartCoroutine(StartNarrativeWithDelay());
    }

    private IEnumerator StartNarrativeWithDelay()
    {
        // Wait until NarrativeCanvasManager is ready
        while (NarrativeCanvasManager.Instance == null)
        {
            yield return null;
        }
        
        NarrativeCanvasManager.Instance.StartTextRoutine(
        new[] { "ACTO 1", "EL ARTE SUSURRA SECRETOS A QUIEN SE ACERCA CON PACIENCIA." },
        new[] { "write", "fade" },
        new[] { "fade", "fade" }
        );
    }

    public static void CompletePuzzle(int puzzleNumber)
    {
        if (Instance == null) return;

        Instance.puzzlesCompleted++;
        if (Instance.puzzleTexts != null && puzzleNumber - 1 < Instance.puzzleTexts.Length)
            Instance.puzzleTexts[puzzleNumber - 1].SetActive(false);

        if (Instance.puzzlesCompleted >= 5 && Instance.exitZone != null)
            Instance.exitZone.SetActive(true);
    }
}