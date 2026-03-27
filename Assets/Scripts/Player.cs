using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI; //new system from unity
using TMPro;

public class Player : MonoBehaviour
{
    public PlayerProfile playerProfile; // Reference to PlayerProfile
    private Animator animator; // Reference to the Animator
    private CharacterController characterController; // For player movement
    private Vector2 moveInput; // Movement input from the player
    private bool nearFishingArea = false; // Checks if the player is near a fishing area
    private bool isFishing = false; // Checks if the player is currently fishing
    public float moveSpeed = 3f; // Character walking speed
    public float gravity = -9f; // Character Gravity
    private float verticalVelocity = 0f;
    public FishingManager fishingManager; // Reference to the FishingManager
    public TMP_Text fishscoretxt; //FISHSCORE INTERFACE
    public GameObject pressF; //Press F button pup up
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Find PlayerProfile Created in MainMenu
        if (playerProfile == null)
            playerProfile = FindFirstObjectByType<PlayerProfile>();
        // Initialize the Animator and CharacterController components
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        // Ensure PlayerProfile is assigned
        if (playerProfile == null)
        {
            Debug.LogError("PlayerProfile not assigned!");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!isFishing)
            {
             HandleMovement();
            }
    }

    public void OnMove(InputValue value)
    {
        // Get movement input
        moveInput = value.Get<Vector2>();
    }
    public void OnStartFishing()
    {
        if (nearFishingArea && !isFishing)
        {
            isFishing = true;
            animator.SetTrigger("Rod_Cast");
            Invoke("SetRodIdle", 1.5f);
            Invoke(nameof(StartFishingMiniGame), 1.5f); // Adjust delay to match your Rod_Cast animation length
            pressF.SetActive(false); //disable press F so it doesnt distract user while playing
        }
    }
    private void StartFishingMiniGame()
    {
        // Call the FishingManager's StartMiniGame method
        if (fishingManager != null)
        {
            fishingManager.onFishingSuccess.RemoveListener(FishingSuccess);
            fishingManager.onFishingFail.RemoveListener(FishingFail);

            fishingManager.StartMiniGame();

            fishingManager.onFishingSuccess.AddListener(FishingSuccess);
            fishingManager.onFishingFail.AddListener(FishingFail);
        }
        else
        {
            Debug.LogWarning("FishingManager is not assigned!");
        }
    }
    private void FishingSuccess()
    {
        if (fishscoretxt != null && playerProfile != null)
        fishscoretxt.text = playerProfile.fishscore.ToString();
        CatchFish();
    }

    private void FishingFail()
    {
        CatchFish();
    }

    private void HandleMovement()
    {
        // 1) Gravity
        if (characterController.isGrounded)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        Vector3 motion = Vector3.up * verticalVelocity * Time.deltaTime;

        // If there's movement input, move and set the Walk animation
        if (moveInput != Vector2.zero)
        {
            //Create movement vector
            Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
            // Rotate the character to face the movement direction
            Vector3 moveDirection = Camera.main.transform.TransformDirection(move);
            moveDirection.y = 0; // Ensure movement is horizontal

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            // Smoothly rotate the character
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            // Move the character in the forward direction
            Vector3 forwardMovement = transform.forward * moveSpeed * Time.deltaTime;
            motion += forwardMovement;

            // Set the Walk animation
            animator.SetBool("isWalking", true);
        }
        else
        {
            // Set Idle animation
            animator.SetBool("isWalking", false);
        }

        characterController.Move(motion);
    }

    private void SetRodIdle()
    {
        // Play the Rod_Idle animation
        animator.SetBool("isFishing", true);
    }
    private void CatchFish()
    {
        // Play the Rod_Reel animation
        animator.SetTrigger("Rod_Reel");
        // Finish fishing after reeling
        Invoke("FinishFishing", 2f); // Adjust 2f to match your Rod_Reel animation length
        pressF.SetActive(true); //re notify the user of pressing button
    }

    private void FinishFishing()
    {
        // Reset variables and animations
        isFishing = false;
        animator.SetBool("isFishing", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FishingArea"))
        {
            // Player enters the fishing area
            nearFishingArea = true;
            pressF.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FishingArea"))
        {
            pressF.SetActive(false);
            // Player exits the fishing area
            nearFishingArea = false;
        }
    }

}
