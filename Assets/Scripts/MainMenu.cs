using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class MainMenu : MonoBehaviour
{
    [Header("User Inputs")]
    public TMP_InputField ageInputField;         // Drag εδώ το AgeInputField
    public TMP_Dropdown genderDropdown;          // Drag εδώ το GenderDropdown
    public Toggle agreement; // Agreement
    public PlayerProfile playerProfile;          // Drag το PlayerProfile object εδώ
    [Header("Loading Popup UI")]
    public GameObject loadingPopup;           // your popup/window GameObject
    public Slider loadingSlider;              // if you use a Slider bar (optional)
    public Image loadingFillImage;            // or an Image (Fill type) (optional)
    public TextMeshProUGUI loadingPercent;    // optional "95%" text
    public float minVisibleTime = 0.8f;       // keeps popup visible briefly
    [Header("Error Popup UI")]
    public GameObject errorPopup;
    public TMP_Text errorMessageText;
    private bool isLoading;

    public void Play()
    {
        if (isLoading) return;

        // Εξασφαλίζεις ότι όλα έχουν συμπληρωθεί και εχει συμφωνησει
        if (string.IsNullOrEmpty(ageInputField.text) || genderDropdown.value == 0)
        {
            errorMessageText.text = "Παρακαλώ συμπλήρωσε ηλικία ή και φύλο!";
            errorPopup.SetActive(true);
            return;
        }
        int age = int.Parse(ageInputField.text);
        if (age < 15 || age > 65)
        {
            errorMessageText.text = "Παρακαλώ συμπλήρωσε ηλικία απο 15 εως 65 !";
            errorPopup.SetActive(true);
            return;
        }
        if (!agreement.isOn)
        {
            errorMessageText.text = "Δεν έχετε δεχτεί τους όρους !";
            errorPopup.SetActive(true);
            return;
        }
            
        string gender = genderDropdown.options[genderDropdown.value].text;

        if (playerProfile == null)
            playerProfile = FindFirstObjectByType<PlayerProfile>();

        playerProfile.age = age;
        playerProfile.gender = gender;
        playerProfile.StartNewSession();
       //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        // Begin async load with popup
        StartCoroutine(LoadNextSceneWithPopup());
    }

    private IEnumerator LoadNextSceneWithPopup()
    {
        isLoading = true;

        // Show popup and reset UI
        if (loadingPopup) loadingPopup.SetActive(true);
        SetProgress(0f);

        // Get next scene index
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Start async load, hold activation until we finish animating to 100%
        AsyncOperation op = SceneManager.LoadSceneAsync(nextIndex);
        op.allowSceneActivation = false;

        float displayed = 0f;
        float tVisible = 0f;

        // Let UI draw
        yield return null;

        while (op.progress < 0.9f)
        {
            // Map 0..0.9 -> 0..1 and smooth the visual value
            float target = Mathf.Clamp01(op.progress / 0.9f);
            displayed = Mathf.MoveTowards(displayed, target, Time.unscaledDeltaTime * 1.5f);
            SetProgress(displayed);
            tVisible += Time.unscaledDeltaTime;
            yield return null;
        }

        // Finish to 100% and ensure popup stayed a moment
        while (displayed < 1f || tVisible < minVisibleTime)
        {
            displayed = Mathf.MoveTowards(displayed, 1f, Time.unscaledDeltaTime * 1.5f);
            SetProgress(displayed);
            tVisible += Time.unscaledDeltaTime;
            yield return null;
        }

        SetProgress(1f);
        op.allowSceneActivation = true; // switch to the loaded scene
    }

    private void SetProgress(float p)
    {
        if (loadingSlider) loadingSlider.value = p;
        if (loadingFillImage) loadingFillImage.fillAmount = p;
        if (loadingPercent) loadingPercent.text = Mathf.RoundToInt(p * 100f) + "%";
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player Quit Game");
    }
    
}
