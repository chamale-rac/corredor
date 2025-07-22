using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NarrativeCanvasManager : MonoBehaviour
{
    [Header("UI References")]
    public Image topPanel;
    public Image bottomPanel;
    public TMP_Text narrativeText;
    public Canvas narrativeCanvas;
    public bool autoToggleCanvas = true;
    
    [Header("Additional Panels to Disable")]
    public GameObject[] panelsToDisable; // Additional panels to disable during text display

    [Header("Transition Settings")]
    public float panelFadeDuration = 0.5f;
    public float textFadeDuration = 0.5f;
    public float typewriterSpeed = 0.04f;

    private Coroutine routine;
    private CanvasGroup topPanelGroup;
    private CanvasGroup bottomPanelGroup;

    public static NarrativeCanvasManager Instance { get; private set; }

    private void Awake()
    {
        Debug.Log("NarrativeCanvasManager Awake called");
        
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroying duplicate NarrativeCanvasManager");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("NarrativeCanvasManager Instance set");
        
        // Validate references
        if (topPanel == null) Debug.LogError("Top Panel is not assigned!");
        if (bottomPanel == null) Debug.LogError("Bottom Panel is not assigned!");
        if (narrativeText == null) Debug.LogError("Narrative Text is not assigned!");
        if (narrativeCanvas == null) Debug.LogError("Narrative Canvas is not assigned!");
        
        // Ensure CanvasGroup exists for fading
        if (topPanel != null)
        {
            topPanelGroup = topPanel.GetComponent<CanvasGroup>();
            if (topPanelGroup == null) 
            {
                topPanelGroup = topPanel.gameObject.AddComponent<CanvasGroup>();
                Debug.Log("Added CanvasGroup to top panel");
            }
            // Set initial opacity to 0
            topPanelGroup.alpha = 0f;
            topPanelGroup.interactable = false;
            topPanelGroup.blocksRaycasts = false;
        }
        
        if (bottomPanel != null)
        {
            bottomPanelGroup = bottomPanel.GetComponent<CanvasGroup>();
            if (bottomPanelGroup == null) 
            {
                bottomPanelGroup = bottomPanel.gameObject.AddComponent<CanvasGroup>();
                Debug.Log("Added CanvasGroup to bottom panel");
            }
            // Set initial opacity to 0
            bottomPanelGroup.alpha = 0f;
            bottomPanelGroup.interactable = false;
            bottomPanelGroup.blocksRaycasts = false;
        }
        
        // Set narrative text to empty and invisible at start
        if (narrativeText != null)
        {
            narrativeText.text = "";
            narrativeText.alpha = 0f;
        }
    }

    /// <summary>
    /// Starts a narrative text routine.
    /// </summary>
    /// <param name="texts">Array of texts to display in sequence.</param>
    /// <param name="textTransitions">Array of transitions for each text: "write", "fade", "instant".</param>
    /// <param name="panelTransitions">Array of panel transitions: "fade", "instant". [in, out]</param>
    /// <param name="displayTimes">Array of times (in seconds) to wait after each text appears. Use -1f for default.</param>
    public void StartTextRoutine(string[] texts, string[] textTransitions, string[] panelTransitions)
    {
        Debug.Log("StartTextRoutine called with " + texts.Length + " texts");
        
        // Validate that all references exist
        if (topPanelGroup == null || bottomPanelGroup == null || narrativeText == null)
        {
            Debug.LogError("Missing UI references! Cannot start text routine.");
            return;
        }
        
        if (routine != null) 
        {
            Debug.Log("Stopping existing routine");
            StopCoroutine(routine);
        }
        
        routine = StartCoroutine(TextRoutine(texts, textTransitions, panelTransitions, null));
    }

    /// <summary>
    /// Overload to support displayTimes parameter.
    /// </summary>
    public void StartTextRoutine(string[] texts, string[] textTransitions, string[] panelTransitions, float[] displayTimes)
    {
        Debug.Log("StartTextRoutine (with displayTimes) called with " + texts.Length + " texts");
        if (topPanelGroup == null || bottomPanelGroup == null || narrativeText == null)
        {
            Debug.LogError("Missing UI references! Cannot start text routine.");
            return;
        }
        if (routine != null)
        {
            Debug.Log("Stopping existing routine");
            StopCoroutine(routine);
        }
        routine = StartCoroutine(TextRoutine(texts, textTransitions, panelTransitions, displayTimes, false));
    }
    
    /// <summary>
    /// Overload to support displayTimes parameter and noFadeOut flag for final text.
    /// </summary>
    public void StartTextRoutine(string[] texts, string[] textTransitions, string[] panelTransitions, float[] displayTimes, bool noFadeOut)
    {
        Debug.Log("StartTextRoutine (with displayTimes and noFadeOut) called with " + texts.Length + " texts, noFadeOut: " + noFadeOut);
        if (topPanelGroup == null || bottomPanelGroup == null || narrativeText == null)
        {
            Debug.LogError("Missing UI references! Cannot start text routine.");
            return;
        }
        if (routine != null)
        {
            Debug.Log("Stopping existing routine");
            StopCoroutine(routine);
        }
        routine = StartCoroutine(TextRoutine(texts, textTransitions, panelTransitions, displayTimes, noFadeOut));
    }

    private IEnumerator TextRoutine(string[] texts, string[] textTransitions, string[] panelTransitions, float[] displayTimes, bool noFadeOut = false)
    {
        Debug.Log("TextRoutine started");
        
        // Disable additional panels
        DisableAdditionalPanels();
        
        // ENABLE CANVAS HERE
        if (autoToggleCanvas && narrativeCanvas != null)
        {
            Debug.Log("Enabling canvas: " + narrativeCanvas.name);
            narrativeCanvas.enabled = true;
        }
        else if (autoToggleCanvas && narrativeCanvas == null)
        {
            Debug.LogError("Narrative Canvas is null but autoToggleCanvas is true!");
        }
        
        // Panel fade in
        string panelInTransition = panelTransitions.Length > 0 ? panelTransitions[0] : "fade";
        Debug.Log("Panel fade in with transition: " + panelInTransition);
        yield return PanelTransition(panelInTransition, true);

        for (int i = 0; i < texts.Length; i++)
        {
            string transition = textTransitions.Length > i ? textTransitions[i] : "write";
            Debug.Log($"Displaying text {i+1}: '{texts[i]}' with transition: {transition}");
            yield return TextTransition(texts[i], transition);
            
            // Wait for the specified display time, or default to 1f if not provided or -1f
            float waitTime = 1f;
            if (displayTimes != null && displayTimes.Length > i && displayTimes[i] >= 0f)
            {
                waitTime = displayTimes[i];
            }
            yield return new WaitForSeconds(waitTime);
        }

        // After the last text animation, clear the text
        narrativeText.text = "";

        // Panel fade out (skip if noFadeOut is true)
        if (!noFadeOut)
        {
            string panelOutTransition = panelTransitions.Length > 1 ? panelTransitions[1] : "fade";
            Debug.Log("Panel fade out with transition: " + panelOutTransition);
            yield return PanelTransition(panelOutTransition, false);

            if (autoToggleCanvas && narrativeCanvas != null)
            {
                Debug.Log("Disabling canvas");
                // narrativeCanvas.enabled = false;
            }
            
            // Re-enable additional panels
            EnableAdditionalPanels();
        }
        else
        {
            Debug.Log("Skipping panel fade out (noFadeOut = true)");
            // Note: Additional panels remain disabled when noFadeOut is true
        }
        
        Debug.Log("TextRoutine completed");
    }

    private IEnumerator PanelTransition(string transition, bool fadeIn)
    {
        Debug.Log($"PanelTransition: {transition}, fadeIn: {fadeIn}");
        
        float target = fadeIn ? 1f : 0f;
        if (transition == "instant")
        {
            topPanelGroup.alpha = target;
            bottomPanelGroup.alpha = target;
            topPanelGroup.interactable = fadeIn;
            bottomPanelGroup.interactable = fadeIn;
            topPanelGroup.blocksRaycasts = fadeIn;
            bottomPanelGroup.blocksRaycasts = fadeIn;
            Debug.Log($"Instant panel transition to alpha: {target}");
            yield break;
        }

        // Ensure panels start fully transparent for fade-in
        if (fadeIn)
        {
            topPanelGroup.alpha = 0f;
            bottomPanelGroup.alpha = 0f;
        }

        float t = 0;
        float start = topPanelGroup.alpha;
        Debug.Log($"Starting panel fade from {start} to {target}");
        
        while (t < panelFadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(start, target, t / panelFadeDuration);
            topPanelGroup.alpha = a;
            bottomPanelGroup.alpha = a;
            yield return null;
        }
        
        topPanelGroup.alpha = target;
        bottomPanelGroup.alpha = target;
        topPanelGroup.interactable = fadeIn;
        bottomPanelGroup.interactable = fadeIn;
        topPanelGroup.blocksRaycasts = fadeIn;
        bottomPanelGroup.blocksRaycasts = fadeIn;
        
        Debug.Log($"Panel transition completed, final alpha: {target}");
    }

    private IEnumerator TextTransition(string text, string transition)
    {
        Debug.Log($"TextTransition: '{text}' with transition: {transition}");
        
        if (transition == "instant")
        {
            narrativeText.text = text;
            narrativeText.alpha = 1f;
            Debug.Log("Instant text display");
            yield break;
        }
        if (transition == "fade")
        {
            Debug.Log("Starting text fade out");
            // Fade out current text
            yield return FadeText(0f);
            narrativeText.text = text;
            Debug.Log("Starting text fade in");
            // Fade in new text
            yield return FadeText(1f);
            yield break;
        }
        if (transition == "write")
        {
            Debug.Log("Starting typewriter effect");
            narrativeText.text = "";
            narrativeText.alpha = 1f;
            for (int i = 0; i <= text.Length; i++)
            {
                narrativeText.text = text.Substring(0, i);
                yield return new WaitForSeconds(typewriterSpeed);
            }
            Debug.Log("Typewriter effect completed");
            yield break;
        }
        // Default fallback
        Debug.Log("Using default text display");
        narrativeText.text = text;
        narrativeText.alpha = 1f;
    }

    private IEnumerator FadeText(float targetAlpha)
    {
        float t = 0;
        float start = narrativeText.alpha;
        Debug.Log($"Fading text from {start} to {targetAlpha}");
        
        while (t < textFadeDuration)
        {
            t += Time.deltaTime;
            narrativeText.alpha = Mathf.Lerp(start, targetAlpha, t / textFadeDuration);
            yield return null;
        }
        narrativeText.alpha = targetAlpha;
        Debug.Log($"Text fade completed, final alpha: {targetAlpha}");
    }
    
    private void DisableAdditionalPanels()
    {
        if (panelsToDisable != null)
        {
            foreach (GameObject panel in panelsToDisable)
            {
                if (panel != null)
                {
                    panel.SetActive(false);
                    Debug.Log($"Disabled additional panel: {panel.name}");
                }
            }
        }
    }
    
    private void EnableAdditionalPanels()
    {
        if (panelsToDisable != null)
        {
            foreach (GameObject panel in panelsToDisable)
            {
                if (panel != null)
                {
                    panel.SetActive(true);
                    Debug.Log($"Enabled additional panel: {panel.name}");
                }
            }
        }
    }
}