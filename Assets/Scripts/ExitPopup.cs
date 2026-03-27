using UnityEngine;
using UnityEngine.UI;

public class ExitPopup : MonoBehaviour
{
    public GameObject popupUI;  // Assign your popup panel here
    private bool isPopupActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePopup();
        }
    }

    public void TogglePopup()
    {
        isPopupActive = !isPopupActive;
        popupUI.SetActive(isPopupActive);
    }

    public void ExitGame()
    {
        Debug.Log("Game is closing...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in editor
#else
        Application.Quit(); // Closes the game in build
#endif
    }
}
