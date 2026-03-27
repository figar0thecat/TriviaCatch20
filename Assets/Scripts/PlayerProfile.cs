using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
//FOR EXPORT
using System.IO;
using System.Text;
[System.Serializable]
public class PlayerProfile : MonoBehaviour
{
    public List<GameLogEntry> gameLogs = new List<GameLogEntry>();
    public int fishscore; // Player's fishing score
    public int maxScore = 20; // Player's max score
    public int age; // Player's Age
    public string gender; // Player's Gender
    public bool isUploading; // Upload flag
    private string currentSessionID;
    public PlayerProfile()
    {
        fishscore = 0;
    }

    public void IncrementFishScore(int score)
    {
        if (fishscore < maxScore)
        {
            fishscore = fishscore + score;
            if (fishscore >= maxScore)
            {
                //ExportLogsToCSV();
                UploadAllLogsToSheet();
            }
        }
        else
        {
            //ExportLogsToCSV();
        }
    }
    public void LogMinigameResult(int trialIndex,string gameType, int attempts, float reactionTime,float adaptiveTimeLimit, bool success,string result,float adaptiveDifficultyValue, string difficulty, float gameTime)
    {
        GameLogEntry entry = new GameLogEntry
        {
            sessionID = currentSessionID,
            age = this.age,
            gender = this.gender,
            trialIndex = trialIndex,
            gameType = gameType,
            attempts = attempts,
            reactionTime = reactionTime,
            adaptiveTimeLimit = adaptiveTimeLimit,
            success = success,
            result = result,
            adaptiveDifficultyValue = adaptiveDifficultyValue,
            difficulty = difficulty,
            gameTime = gameTime
        };
        gameLogs.Add(entry);
    }
    /*public void ExportLogsToCSV()
    {
        string fileName = $"FishingGameLog_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        StringBuilder csv = new StringBuilder();
        // Header
        csv.AppendLine("sessionID,age,gender,gameType,attempts,reactionTime,success,difficulty,gameTime");

        foreach (var entry in gameLogs)
        {
            csv.AppendLine($"{entry.sessionID},{entry.age},{entry.gender},{entry.gameType},{entry.attempts},{entry.reactionTime},{entry.success},{entry.difficulty},{entry.gameTime}");
        }

        File.WriteAllText(filePath, csv.ToString());
        Debug.Log($"Exported CSV to: {filePath}");
    } */

    //ONLINE APPEND TO GOOGLE DRIVE SHEET
    [System.Serializable]
    public class GameLogBatch { public List<GameLogEntry> entries; }
    public const bool UPLOAD_ENABLED = true;
    public const string WEB_APP_URL = "https://script.google.com/macros/s/AKfycbwClmySpe1CSKAfFMShi_3ieeVHhgrtcbo8K6t6iB2YL1-gRgTr1cP4zl1jfcWrVWZqqw/exec";

    public void UploadAllLogsToSheet()
    {
        if (!UPLOAD_ENABLED || string.IsNullOrEmpty(WEB_APP_URL)) return;

        var batch = new GameLogBatch { entries = this.gameLogs };
        var json = JsonUtility.ToJson(batch);
        isUploading= true;
        StartCoroutine(PostJson(WEB_APP_URL, json));
    }

    private System.Collections.IEnumerator PostJson(string url, string json)
    {
        using (var req = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Batch upload failed: {req.error}");
            else
            {
                Debug.Log($"Batch upload ok: {req.downloadHandler.text}");
                isUploading = false;
            }

                
        }
    }
    //dont detroy in main menu
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        StartNewSession(); // safe here, will run when the object is created
    }
    public void StartNewSession()
    {
        currentSessionID = System.Guid.NewGuid().ToString();
        gameLogs.Clear();
        fishscore = 0;
    }
}
