using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Animator playerAnimator;

    [Header("Movement")]
    public float moveSpeed = 12f;
    public float turnSpeed = 15f;
    public float gravity = -19.62f;

    [Header("Slide")]
    public float slideSpeed = 18f;
    public float slideDuration = 0.55f;
    public float slideCooldown = 0.6f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    public Vector3 moveDir { get; private set; }

    private Vector3 currentVelocity;
    private bool isGrounded;
    private bool isSliding;
    private float nextSlideTime;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (controller == null) controller = GetComponent<CharacterController>();
        if (playerAnimator == null) playerAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (playerAnimator != null) playerAnimator.SetBool("isGrounded", isGrounded);

        if (isGrounded && currentVelocity.y < 0)
            currentVelocity.y = -2f;

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(x, 0f, z).normalized;

        if (Camera.main != null && inputDir.magnitude > 0.1f)
        {
            Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;
            moveDir = (camRight * inputDir.x + camForward * inputDir.z).normalized;
        }

        if (!isSliding)
        {
            if (inputDir.magnitude > 0.1f)
            {
                controller.Move(moveDir * moveSpeed * Time.deltaTime);

                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            }

            if (playerAnimator != null)
                playerAnimator.SetFloat("Speed", inputDir.magnitude, 0.1f, Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !isSliding && Time.time >= nextSlideTime)
        {
            StartCoroutine(ExecuteSlide());
        }

        currentVelocity.y += gravity * Time.deltaTime;
        controller.Move(currentVelocity * Time.deltaTime);
    }

    IEnumerator ExecuteSlide()
    {
        isSliding = true;

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isSliding", true);
            playerAnimator.SetTrigger("Slide");
        }

        Vector3 slideDirection = moveDir != Vector3.zero ? moveDir : transform.forward;
        float timer = 0f;

        while (timer < slideDuration)
        {
            float currentSlideSpeed = Mathf.Lerp(slideSpeed, moveSpeed, timer / slideDuration);

            controller.Move(slideDirection * currentSlideSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        if (playerAnimator != null)
            playerAnimator.SetBool("isSliding", false);

        isSliding = false;
        nextSlideTime = Time.time + slideCooldown;
    }
}