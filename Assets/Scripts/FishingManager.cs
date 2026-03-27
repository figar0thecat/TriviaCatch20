using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using TMPro; // Import the TextMeshPro namespace
using UnityEngine.Networking; // to connect online 
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Net; //DECODE OPENTRIVIA HTML 
public class FishingManager : MonoBehaviour
{
    public PlayerProfile playerProfile; // Reference to PlayerProfile
    public GameTimer gameTime; // Reference to Gametimer
    private float elapsedtime;
    private bool isPlayingMiniGame = false; // Is a mini-game active?
    private int gameChoice;
    private int trialIndex = 0;
    private bool timeOut = false; //timeout flag
    private int sameMiniGameStrike = 0;
    private int lastMiniGame = -1;
    [Header("Reaction Game")]
    //REACTION GAME
    public GameObject reactionMiniGameUI; // UI for Reaction Mini Game
    private Button reactionButton = null; // Button for Reaction Mini-Game
    public List<GameObject> fishLocations = new List<GameObject>();
    private float reactionStartTime; // Variable helping calculate reaction time
    [Header("Trivia Game")]
    //TRIVIA GAME
    public GameObject triviaMiniGameUI; // UI for Trivia Mini Game
    public TMP_Text triviaQuestionText; // Question Text
    public Button buttonA, buttonB; // Buttons for Question answers
    private string correctAnswer; // Stores the correct answer
    private int triviaAttempts = 1; // Track number of trivia attempts
    private int triviaPoints = 2;   // Points awarded for trivia success
    private float triviaRT1 = 0; // Reaction time for attempt 1
    private float triviaRT2 = 0; // Reaction time for attempt 2
    [Header("Retry UI")]
    public GameObject retryPopup;       // panel container
    public TMP_Text retryText;          // "Retrying in... 3/2/1/RETRY!"
    public Button retryButton;          // Retry button to fetch a new question
    [Header("Pop Ups")]
    // START / FINISH POP UP UI's
    public GameObject startFishingPOPUP; // UI for Start Fishing POP UP
    public GameObject successFishingPOPUP;
    public GameObject failedFishingPOPUP;
    [Header("Timer")]
    //TIMERS 
    public Slider timerBar;
    public Image timerBarFill;
    private float timeLimit = 5f;
    private Coroutine timerRoutine; //για να δηλωσω το coroutine του timer για να μην κανω stopall
    // ON SUCCESS / FAIL 
    public UnityEvent onFishingSuccess; // Event triggered on success
    public UnityEvent onFishingFail; // Event triggered on failure
    //ΑΙ ADAPTIVE DIFFICULTY 0% - Easy, 50% - Medium, 100% - Hard
    private float adaptiveDifficulty = 0.5f; // Ξεκινάει στο “medium” επίπεδο [0.0, 1.0]
    private const float minDifficulty = 0.0f;
    private const float maxDifficulty = 1.0f;
    [Header("Difficulty")]
    // ADAPTIVE DIFFICULTY UI 
    public TMP_Text AdaptiveDifficultyText;
    [Header("Sounds")]
    public AudioClip successClip;
    public AudioClip failClip;
    public AudioSource audioSource;
    //GAME FINISH
    [Header("Game Complete Popup")]
    public GameObject sessionCompletePopup;
    public TMP_Text sessionStatusText;
    public Button exitToMenuButton;
    public TMP_Text exitToMenuLabel;
    //TEMPORARY SOLUTION TRIVIA DELETE AFTER
    // --- Local fallback για Animals | hard | boolean (True/False) ---
    [SerializeField] private bool useLocalAnimalsHardTF = true;
    // Συμπλήρωσέ τα από το Inspector: κάθε γραμμή "Ερώτηση ||| True" ή "Ερώτηση ||| False"
    [TextArea(2, 4)] public List<string> localAnimalsHardTF = new List<string>();
    private System.Random localRng = new System.Random();
    private int localIndex = 0; // κυκλικός δείκτης για να μην επαναλαμβάνονται συνέχεια οι ίδιες
    void Start()
    {
        //Find PlayerProfile Created in MainMenu
        if (playerProfile == null)
            playerProfile = FindFirstObjectByType<PlayerProfile>();
        // Find Game Time 
        gameTime = FindFirstObjectByType<GameTimer>();
        // Hide the UI's
        startFishingPOPUP.SetActive(false);
        reactionMiniGameUI.SetActive(false);
        triviaMiniGameUI.SetActive(false);
        successFishingPOPUP.SetActive(false);

    }

    //Starts Minigames with pop up 
    public void StartMiniGame()
    {
        if (isPlayingMiniGame) return;

        isPlayingMiniGame = true;
        startFishingPOPUP.SetActive(true);
        trialIndex++; // increase trials id
        StartCoroutine(StartMiniGameWithCountdown());
    }

    private IEnumerator StartMiniGameWithCountdown()
    {
        TMP_Text countdownText = startFishingPOPUP.GetComponentInChildren<TMP_Text>(); // Ensure the popup has a Text element
        if (countdownText == null)
        {
            Debug.LogWarning("No Text component found in startFishingPOPUP!");
            yield break;
        }

        // Countdown logic
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString(); // Update the countdown number
            yield return new WaitForSeconds(1f); // Wait for 1 second
        }
        startFishingPOPUP.SetActive(false);

        // Hide the popup and start randomly 50% reaction or trivia game !
        // επειδη υπηρχε προβλημα και μπορει να υπηρχε συνεχεια σχεδον και 3 4 συνεχωμενα απο το 1 μονο mini game 
        // επρεπε καπως να το κανω νεμεν random αλλα να ειναι και δικαιο για το εκαστοτε mini game
        gameChoice = UnityEngine.Random.Range(0, 2); // 0 = Reaction, 1 = Trivia

        if (gameChoice == lastMiniGame)
        {
            sameMiniGameStrike++;
            if (sameMiniGameStrike >= 2)
            {
                gameChoice = 1 - lastMiniGame;
                sameMiniGameStrike = 0;
            }
        }

        lastMiniGame = gameChoice;
        //0 
        if (gameChoice == 5)
            StartReactionMiniGame();
        else
            StartTriviaMiniGame();

    }
    
    //REACTION GAME CODE LOGIC
    // Reaction Mini-Game Start Pop up UI
    private void StartReactionMiniGame()
    {
        //AI ADAPTIVE DIFFICULTY
        timeLimit = GetAdaptiveReactionTimeLimit();
        //UI
        reactionMiniGameUI.SetActive(true);
        // Assign Random Fish button its listener
        int randomIndex = UnityEngine.Random.Range(0, fishLocations.Count);
        GameObject chosenFish = fishLocations[randomIndex];
        reactionButton = chosenFish.GetComponent<Button>();
        reactionButton.onClick.AddListener(HandleReactionButtonPress);

        StartCoroutine(ReactionMiniGameCoroutine());
    }

    private IEnumerator ReactionMiniGameCoroutine()
    {
        // Wait a random time before activating the button
        float randomWait = UnityEngine.Random.Range(1f, 2f);
        yield return new WaitForSeconds(randomWait);
        reactionStartTime = Time.time; //RT BEGIN OF REACTION GAME
        reactionButton.gameObject.SetActive(true); // Enable button
        StartTimer();
    }

    private void HandleReactionButtonPress()
    {
        float reactionTime = Time.time - reactionStartTime; // find the press time
        playerProfile.IncrementFishScore(1);
        StopTimer(); //stops the timer
        reactionButton.gameObject.SetActive(false); // Disable button
        reactionButton.onClick.RemoveListener(HandleReactionButtonPress); //Remove Listener 
        reactionMiniGameUI.SetActive(false); // Disable UI
        elapsedtime = gameTime != null ? gameTime.GetElapsedTime() : 0f;
        playerProfile.LogMinigameResult(trialIndex,"reaction", 1, reactionTime,GetAdaptiveReactionTimeLimit(), true,"success",adaptiveDifficulty, GetDifficultyLevel(), elapsedtime);
        MiniGameSuccess();
    }

    //TRIVIA GAME CODE LOGIC

    // Classes to parse JSON response from Open Trivia Database API
    [System.Serializable]
    public class TriviaResponse
    {
        public TriviaResult[] results;
    }

    [System.Serializable]
    public class TriviaResult
    {
        public string question;
        public string correct_answer;
        public string[] incorrect_answers;
    }

    private void StartTriviaMiniGame()
    {
        //AI ADAPTIVE DIFFICULTY 
        timeLimit = GetCurrentTriviaTimeLimit();
        //UI
        triviaMiniGameUI.SetActive(true);
        // Assign button listeners for trivia answers
        buttonA.onClick.RemoveAllListeners();
        buttonB.onClick.RemoveAllListeners();
        StartCoroutine(FetchTriviaQuestion(startTimer: triviaAttempts == 1));
    }

    private IEnumerator FetchTriviaQuestion(bool startTimer)
    {
        string url = $"https://opentdb.com/api.php?amount=1&category=27&difficulty={GetDifficultyLevel()}&type=boolean"; 
        bool success = false;
        //TEMP CODE FOR TRIVIA
        // ==== ΠΡΟΣΘΕΣΕ από εδώ: local fallback για Animals | hard | boolean ====
        if (GetDifficultyLevel() == "hard" && TryGetLocalAnimalsHardTF(out var localQ, out var localCorrect))
        {
            // Στήσε UI όπως όταν έρχεται από API
            triviaQuestionText.text = localQ;

            // Ανακάτεψε True/False ώστε να μην «μαθαίνονται» θέσεις
            var opts = new List<string> { "True", "False" };
            for (int i = opts.Count - 1; i > 0; i--)
            {
                int j = localRng.Next(i + 1);
                (opts[i], opts[j]) = (opts[j], opts[i]);
            }

            buttonA.GetComponentInChildren<TMPro.TMP_Text>().text = opts[0];
            buttonB.GetComponentInChildren<TMPro.TMP_Text>().text = opts[1];

            // Θυμήσου την σωστή απάντηση για το CheckTriviaAnswer(...)
            correctAnswer = localCorrect;

            // Καθάρισε και βάλε listeners όπως ήδη κάνεις στο API path
            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.RemoveAllListeners();
            buttonA.onClick.AddListener(() => CheckTriviaAnswer(buttonA));
            buttonB.onClick.AddListener(() => CheckTriviaAnswer(buttonB));

            // RT markers + timer όπως τώρα (βλέπε κώδικα API path)
            if (triviaAttempts == 1) triviaRT1 = Time.time; else triviaRT2 = Time.time;
            if (startTimer) StartTimer(); else StopTimer();

            // Τέλος: βγήκες από το coroutine ΧΩΡΙΣ να μπει στο API loop
            yield break;
        }
        // ==== μέχρι εδώ το πρόσθετο ====
        triviaQuestionText.text = "Loading trivia question...";
        while (!success)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Απάντηση Απο OPEN TRIVIA API : " + webRequest.downloadHandler.text);
                var jsonResponse = JsonUtility.FromJson<TriviaResponse>(webRequest.downloadHandler.text);
                if (jsonResponse.results.Length > 0)
                {
                    TriviaResult trivia = jsonResponse.results[0];
                    //triviaQuestionText.text = UnityWebRequest.UnEscapeURL(trivia.question);
                    //DECODE HTML RESULT OF OPEN TRIVIA 
                    triviaQuestionText.text = WebUtility.HtmlDecode(trivia.question);

                    // add answers
                    //put wrong in [0]
                    List<string> answers = new List<string>(trivia.incorrect_answers);
                    //put correct in [1]
                    answers.Add(trivia.correct_answer);
                    //random index between 0 - 1
                    int triviaRandomIndex = UnityEngine.Random.Range(0, 2);
                    //put the random number in the first button
                    buttonA.GetComponentInChildren<TMP_Text>().text = answers[triviaRandomIndex];
                    //then put the other number in second button
                    if (triviaRandomIndex != 0)
                        buttonB.GetComponentInChildren<TMP_Text>().text = answers[0];
                    else
                        buttonB.GetComponentInChildren<TMP_Text>().text = answers[1];

                    correctAnswer = trivia.correct_answer;

                    buttonA.onClick.AddListener(() => CheckTriviaAnswer(buttonA));
                    buttonB.onClick.AddListener(() => CheckTriviaAnswer(buttonB));

                    //RT CHECKERS BEGIN
                    if (triviaAttempts == 1)
                        triviaRT1 = Time.time;
                    else
                        triviaRT2 = Time.time;

                    if (startTimer)
                        StartTimer();
                    else
                        StopTimer();

                    success = true; // Exit the loop, success!
                }
                //IF API RESPONDS BUT WITH NO CONTEXT INSIDE
                else
                {
                    Debug.LogError("API returned no questions, retrying...");
                    triviaQuestionText.text = "No questions found, retrying...";
                    yield return new WaitForSeconds(2f); // Wait before retrying
                }
            }
            //IF HTTP REQUEST FAILS
            else
            {
                Debug.LogError("Failed to fetch trivia question. ERROR: " + webRequest.error + " Retrying...");
                triviaQuestionText.text = "Wrong Answer!Retrying without time limit... \n Your Question is getting ready...";
                //δεν εναι ιδανικη λυση
                yield return new WaitForSeconds(2f); // Wait before retrying
            }

        }

    }
    private void CheckTriviaAnswer(Button selectedButton)
    {
        float rtToLog;
        if (triviaAttempts == 1)
            rtToLog = Time.time - triviaRT1;
        else
            rtToLog = Time.time - triviaRT2;

        if (selectedButton.GetComponentInChildren<TMP_Text>().text == correctAnswer)
        {
            playerProfile.IncrementFishScore(triviaPoints); // Add score based on attempts
            elapsedtime = gameTime != null ? gameTime.GetElapsedTime() : 0f;
            //Αν ειναι το 1o attempt μετρα adaptive time limit αλλιως στο 2ο attempt untimed -1
            float limitToLog = (triviaAttempts == 1) ? GetCurrentTriviaTimeLimit() : -1f;
            playerProfile.LogMinigameResult(trialIndex,"trivia", triviaAttempts, rtToLog,limitToLog, true,"success",adaptiveDifficulty, GetDifficultyLevel(), elapsedtime);
            MiniGameSuccess();
            triviaAttempts = 1; //reset for next time
            triviaPoints = 2;
            //reset timers
            triviaRT1 = 0;
            triviaRT2 = 0;
            CleanUpTriviaUI();

        }
        else
        {
            // Check if it's the first attempt, retry without a timer
            if (triviaAttempts == 1)
            {
                //LOG IT
                elapsedtime = gameTime != null ? gameTime.GetElapsedTime() : 0f;
                float limitToLog = (triviaAttempts == 1) ? GetCurrentTriviaTimeLimit() : -1f;
                playerProfile.LogMinigameResult(trialIndex, "trivia", triviaAttempts, rtToLog, limitToLog, false, "fail_wrong", adaptiveDifficulty, GetDifficultyLevel(), elapsedtime);

                triviaAttempts++;
                triviaPoints = 1;  // Reduce points for the second attempt
                triviaQuestionText.text = "Wrong Answer!Retrying without time limit...";
                buttonA.onClick.RemoveAllListeners();
                buttonB.onClick.RemoveAllListeners();
                StopTimer();
                //play fail sound
                audioSource.PlayOneShot(failClip);
                //show retry UI 
                StartCoroutine(ShowRetryUI());
                //StartCoroutine(FetchTriviaQuestion(startTimer: false));
                return;
            }
            elapsedtime = gameTime != null ? gameTime.GetElapsedTime() : 0f;
            playerProfile.LogMinigameResult(trialIndex,"trivia", triviaAttempts, rtToLog, -1f, false,"fail_wrong",adaptiveDifficulty, GetDifficultyLevel(), elapsedtime);
            MiniGameFail(); // If second attempt also fails, end the game
            triviaAttempts = 1; // reset and clean for next time
            triviaPoints = 2;
            //reset timers
            triviaRT1 = 0;
            triviaRT2 = 0;
            CleanUpTriviaUI();
        }
    }
    //RETRY TRIVIA UI CODE
    private IEnumerator ShowRetryUI()
    {
        retryPopup.SetActive(true);
        retryButton.interactable = false;
        retryButton.onClick.RemoveAllListeners();

        // countdown to text
        for (int i = 4; i > 0; i--)
        {
            retryText.text = $"Retrying in... {i}";
            yield return new WaitForSeconds(1f);
        }

        // Enable the button
        retryText.text = "RETRY!";
        retryButton.interactable = true;

        retryButton.onClick.AddListener(() =>
        {
            // Hide popup and fetch a new question WITHOUT a timer
            retryPopup.SetActive(false);
            StartCoroutine(FetchTriviaQuestion(startTimer: false));
        });
    }
    // CLEANS THE TRIVIA INTERFACE
    private void CleanUpTriviaUI()
    {
        StopTimer();
        triviaMiniGameUI.SetActive(false);
        buttonA.onClick.RemoveAllListeners();
        buttonB.onClick.RemoveAllListeners();
    }
    //TIMERS CODE LOGIC
    private void StartTimer()
    {
        timerBar.gameObject.SetActive(true);
        // αν τρέχει ήδη, σταμάτα το παλιό
        if (timerRoutine != null) { StopCoroutine(timerRoutine); }
        timerRoutine = StartCoroutine(TimerCountdown());
    }

    private void StopTimer()
    {
        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerRoutine = null;
        }
        timerBar.gameObject.SetActive(false);
    }

    private IEnumerator TimerCountdown()
    {
        float elapsedTime = 0f;
        while (elapsedTime < timeLimit)
        {
            elapsedTime += Time.deltaTime;
            timerBar.value = 1 - (elapsedTime / timeLimit);
            timerBarFill.color = Color.Lerp(Color.red, Color.green, timerBar.value);
            yield return null;
        }
        timeOut = true;
        MiniGameFail();
    }

    // Handle success
    private void MiniGameSuccess()
    {
        //UPDATE AI ADAPTIVE DIFFICULTY
        UpdateAdaptiveDifficulty(true);
        //UPDATE DIFFICULTY UI
        UpdateDifficultyUI();
        //CLEAR UI
        isPlayingMiniGame = false;
        onFishingSuccess?.Invoke();
        successFishingPOPUP.SetActive(true);
        StopTimer();
        Invoke("HideSuccessPopup", 2f);
        audioSource.PlayOneShot(successClip);

        //FINISH GAME ( Catch 20 Fish )
        if (playerProfile != null && playerProfile.fishscore >= playerProfile.maxScore)
        {
            StartCoroutine(SaveAndExit());
        }
    }
    private void HideSuccessPopup()
    {
        successFishingPOPUP.SetActive(false);
    }

    private void HideFailedPopup()
    {
        failedFishingPOPUP.SetActive(false);
    }

    // Handle failure
    private void MiniGameFail()
    {
        //Get Current Time
        elapsedtime = gameTime != null ? gameTime.GetElapsedTime() : 0f;
        //CLEAR MINI GAMES
        isPlayingMiniGame = false;
        if (gameChoice == 0)
        {
            reactionMiniGameUI.SetActive(false);
            reactionButton.gameObject.SetActive(false);
            reactionButton.onClick.RemoveListener(HandleReactionButtonPress);
            //IF FAILED BY TIMEOUT LOG IT
            if (timeOut == true)
            {
                playerProfile.LogMinigameResult(trialIndex, "reaction", 1, 0, GetAdaptiveReactionTimeLimit(), false, "fail_timeout", adaptiveDifficulty, GetDifficultyLevel(), elapsedtime);
                timeOut = false;
            }

        }
        else
        {
            triviaMiniGameUI.SetActive(false);
            buttonA.onClick.RemoveAllListeners();
            buttonB.onClick.RemoveAllListeners();
            triviaPoints = 2; //reset trivia points
            //IF FAILED BY TIMEOUT LOG IT
            if (timeOut == true)
            {
                playerProfile.LogMinigameResult(trialIndex,"trivia", triviaAttempts, 0, GetCurrentTriviaTimeLimit(), false,"fail_timeout",adaptiveDifficulty, GetDifficultyLevel(), elapsedtime);
                timeOut = false;
            }
        }
        //UPDATE AI ADAPTIVE DIFFICULTY
        UpdateAdaptiveDifficulty(false);
        //UPDATE DIFFICULTY UI
        UpdateDifficultyUI();
        onFishingFail?.Invoke();
        failedFishingPOPUP.SetActive(true);
        StopTimer();
        Invoke("HideFailedPopup", 2f);
        audioSource.PlayOneShot(failClip);

    }


    //AI ADAPTIVE DIFFICULTY
    private void UpdateAdaptiveDifficulty(bool result)
    {   // + 5% -10% per success / failure 
        adaptiveDifficulty += result ? 0.05f : -0.10f;
        adaptiveDifficulty = Mathf.Clamp(adaptiveDifficulty, minDifficulty, maxDifficulty);
    }

    private float GetAdaptiveReactionTimeLimit()
    {
        float baseTime = 2.0f; // Αρχικός χρόνος
        float minTime = 0.5f;  // Ελάχιστος χρόνος
        return Mathf.Lerp(baseTime, minTime, adaptiveDifficulty);
    }
    private float GetCurrentTriviaTimeLimit()
    {
        float baseTime = 5.0f;
        float minTime = 2.5f;
        return Mathf.Lerp(baseTime, minTime, adaptiveDifficulty);
    }
    private string GetDifficultyLevel()
    {
    if (adaptiveDifficulty < 0.33f)
        return "easy";
    else if (adaptiveDifficulty < 0.66f)
        return "medium";
    else
        return "hard";
    }

    private void UpdateDifficultyUI()
    {
        if (adaptiveDifficulty < 0.33f)
            AdaptiveDifficultyText.text = "Easy";
        else if (adaptiveDifficulty < 0.66f)
            AdaptiveDifficultyText.text = "Medium";
        else
        {
            AdaptiveDifficultyText.text = "Hard";
        }


    }
    private IEnumerator SaveAndExit()
    {
        // Show your session complete popup
        if (sessionCompletePopup != null)
            sessionCompletePopup.SetActive(true);

        if (sessionStatusText != null)
            sessionStatusText.text = "You Successfuly Catched 20 Fish !";

        //saving loop animation
        string[] dots = { ".", "..", "...", "...." };
        int piu = 1;
        while (playerProfile.isUploading)
        {
            exitToMenuLabel.text = "Saving" + dots[piu % dots.Length];
            piu++;
            yield return new WaitForSeconds(0.5f);
        }
        if (exitToMenuButton != null)
        {
            exitToMenuLabel.text = "EXIT GAME";
            exitToMenuButton.onClick.RemoveAllListeners();
            exitToMenuButton.onClick.AddListener(() =>
            {
                Application.Quit();
                //SceneManager.LoadScene(0); // back to Main Menu
            });
        }
    }
    //TEMP METHOD
    //TEMP METHOD
    //TEMP METHOD
    private bool TryGetLocalAnimalsHardTF(out string question, out string correct)
    {
        question = null; correct = null;
        if (!useLocalAnimalsHardTF || localAnimalsHardTF == null || localAnimalsHardTF.Count == 0)
            return false;

        // Στην πρώτη φορά κάνε ένα shuffle για ποικιλία
        if (localIndex == 0 && localAnimalsHardTF.Count > 1)
        {
            for (int i = localAnimalsHardTF.Count - 1; i > 0; i--)
            {
                int j = localRng.Next(i + 1);
                (localAnimalsHardTF[i], localAnimalsHardTF[j]) = (localAnimalsHardTF[j], localAnimalsHardTF[i]);
            }
        }

        // Κυκλικά
        string line = localAnimalsHardTF[localIndex % localAnimalsHardTF.Count];
        localIndex++;

        int sep = line.IndexOf("|||", System.StringComparison.Ordinal);
        if (sep < 0) return false;

        question = line.Substring(0, sep).Trim();
        string ans = line.Substring(sep + 3).Trim();

        // Δέξου "True"/"False" (κεφαλαία/πεζά)
        if (ans.Equals("true", System.StringComparison.OrdinalIgnoreCase))
            correct = "True";
        else if (ans.Equals("false", System.StringComparison.OrdinalIgnoreCase))
            correct = "False";
        else
            return false;

        return true;
    }

}
