[System.Serializable]
public class GameLogEntry
{
    public string sessionID;
    public int age;
    public string gender;
    public int trialIndex; // αυξωντας αριθμος των trials 
    public string gameType;         // "reaction" ή "trivia"
    public int attempts; // 1 ή 2   
    public float reactionTime;      // Reaction Time μεχρι να απαντησει ο χρήστης
    public float adaptiveTimeLimit; //το current max adaptivetimelimit
    public bool success;
    public string result; //success ή fail_wrong ή fail_timeout
    public float adaptiveDifficultyValue; //το αριθμιτικο value της δυσκολίας 
    public string difficulty;          // easy / medium / hard
    public float gameTime; //elapsed gametime
}
