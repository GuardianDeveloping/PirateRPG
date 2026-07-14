
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

/*
    This file has a commented version with details about how each line works. 
    The commented version contains code that is easier and simpler to read. This file is minified.
*/


/// <summary>
/// Main script for third-person movement of the character in the game.
/// Make sure that the object that will receive this script (the player) 
/// has the Player tag and the Character Controller component.
/// </summary>
public class ThirdPersonController : MonoBehaviour
{

    [Tooltip("Speed ​​at which the character moves. It is not affected by gravity or jumping.")]
    public float velocity = 5f;
    [Tooltip("This value is added to the speed value while the character is sprinting.")]
    public float sprintAdittion = 3.5f;
    [Tooltip("The higher the value, the higher the character will jump.")]
    public float jumpForce = 18f;
    [Tooltip("Stay in the air. The higher the value, the longer the character floats before falling.")]
    public float jumpTime = 0.85f;
    [Space]
    [Tooltip("Force that pulls the player down. Changing this value causes all movement, jumping and falling to be changed as well.")]
    public float gravity = 9.8f;


    // Player states
    bool isSprinting = false;
    bool isCrouching = false;

    // Inputs
    float inputHorizontal;
    float inputVertical;
    bool inputJump;
    bool inputCrouch;
    bool inputSprint;
    bool IsSwordDrawn;
    private float verticalVelocity;

    private bool isAttacking;

    Animator animator;
    CharacterController cc;


    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Message informing the user that they forgot to add an animator
        if (animator == null)
            Debug.LogWarning("Hey buddy, you don't have the Animator component in your player. Without it, the animations won't work.");
    }






    // Update is only being used here to identify keys and trigger animations
    void Update()
    {

        // Input checkers
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetButtonDown("Jump");
        inputSprint = Input.GetAxis("Fire3") == 1f;
        // Unfortunately GetAxis does not work with GetKeyDown, so inputs must be taken individually
        inputCrouch = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.JoystickButton1);


        if (Input.GetKeyDown(KeyCode.R))
        {
            if (IsSwordDrawn)
            {
                animator.SetTrigger("SheatheSword");

                IsSwordDrawn = false;
                animator.SetBool("IsSwordDrawn", false);
            }
            else
            {
                animator.SetTrigger("DrawSword");

                IsSwordDrawn = true;
                animator.SetBool("IsSwordDrawn", true);
            }
        }

        if (Input.GetMouseButtonDown(0) && IsSwordDrawn && !isAttacking)
        {
            animator.SetTrigger("Attacking");
            isAttacking = true;
        }


        // Check if you pressed the crouch input key and change the player's state
        if (inputCrouch)
            isCrouching = !isCrouching;

        // Run and Crouch animation
        // If dont have animator component, this block wont run
        if (cc.isGrounded && animator != null)
        {

            // Crouch
            // Note: The crouch animation does not shrink the character's collider
            animator.SetBool("crouch", isCrouching);

            // Run
            float minimumSpeed = 0.9f;
            animator.SetBool("run", cc.velocity.magnitude > minimumSpeed);

            // Sprint
            isSprinting = cc.velocity.magnitude > minimumSpeed && inputSprint;
            animator.SetBool("sprint", isSprinting);

        }

        // Jump animation
        if (animator != null)
            animator.SetBool("air", cc.isGrounded == false);

        // Handle can jump or not

        HeadHittingDetect();
        UpdateMovement();

    }


    // With the inputs and animations defined, FixedUpdate is responsible for applying movements and actions to the player
    private void UpdateMovement()
    {
        float velocityAdittion = 0f;

        if (isSprinting)
        {
            velocityAdittion = sprintAdittion;
        }

        if (isCrouching)
        {
            velocityAdittion = -(velocity * 0.50f);
        }

        float currentSpeed = velocity + velocityAdittion;

        // Camera-relative directions
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Build horizontal movement
        Vector3 horizontalDirection =
            forward * inputVertical +
            right * inputHorizontal;

        // Prevent diagonal movement from being faster
        if (horizontalDirection.sqrMagnitude > 1f)
        {
            horizontalDirection.Normalize();
        }

        horizontalDirection *= currentSpeed;

        if (cc.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }


        if (inputJump && cc.isGrounded)
        {
            verticalVelocity = jumpForce;
        }

        verticalVelocity -= gravity * Time.deltaTime;

        if (isAttacking)
        {
            inputHorizontal = 0f;
            inputVertical = 0f;
        }


        // Rotate toward movement direction
        if (horizontalDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(horizontalDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.deltaTime
            );
        }

        // Combine horizontal and vertical movement
        Vector3 movement = horizontalDirection;
        movement.y = verticalVelocity;

        // CharacterController.Move expects distance, so multiply once here
        cc.Move(movement * Time.deltaTime);
    }


    //This function makes the character end his jump if he hits his head on something
    void HeadHittingDetect()
    {
        float headHitDistance = 1.1f;
        Vector3 ccCenter = transform.TransformPoint(cc.center);
        float hitCalc = cc.height / 2f * headHitDistance;

        // Uncomment this line to see the Ray drawed in your characters head
        // Debug.DrawRay(ccCenter, Vector3.up * headHeight, Color.red);

        if (Physics.Raycast(ccCenter, Vector3.up, hitCalc))
        {

        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

}
