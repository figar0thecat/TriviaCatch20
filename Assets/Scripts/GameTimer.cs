using UnityEngine;
using TMPro; // For TextMeshPro support

public class GameTimer : MonoBehaviour
{
    public TMP_Text timerText; // Reference to the UI text to display the timer
    private float elapsedTime = 0f; // Total elapsed time in seconds
    private bool isRunning = false; // Timer state

    void Update()
    {
        if (isRunning)
        {
            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Update the timer UI
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}"; // Format as MM:SS
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
