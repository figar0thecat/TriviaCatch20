using UnityEngine;
using Unity.Cinemachine;
public class CameraInputController : MonoBehaviour
{
    public CinemachineInputAxisController inputAxisController;

    void Start()
    {
        // Auto-find if not assigned in Inspector
        if (inputAxisController == null)
            inputAxisController = GetComponent<CinemachineInputAxisController>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (inputAxisController != null)
                inputAxisController.enabled = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (inputAxisController != null)
                inputAxisController.enabled = false;
        }
    }
}
