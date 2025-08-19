using System;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 6f;
    public float gravity = -20f;
    public float climbSpeed = 3f;
    public float climbCheckDistance = 1f;
    public LayerMask wallLayer;

    [Header("References")]
    public FloatingJoystick joystick;
    public Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 moveDir;
    private bool isJumping;
    private bool isMoving;
    private bool isClimbing;
    private bool isClimbingUp;
    private float buildingTopY;

    // Animation hashes
    private readonly int idleHash = Animator.StringToHash("Idle");
    private readonly int runHash = Animator.StringToHash("Running");
    private readonly int jumpHash = Animator.StringToHash("Jump");
    private readonly int climbHash = Animator.StringToHash("Climb");
    private readonly int climbUpHash = Animator.StringToHash("ClimbUp");
    private readonly int deathHash = Animator.StringToHash("Death");
    private readonly int winHash = Animator.StringToHash("Win");
    private int currentState;

    [SerializeField] private Transform cam;
    Vector3 camForward, camRight, move;
    [SerializeField] private Material playerMaterial;
    //[SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    private string floatPropertyName = "_EmissiveIntensity";
    [SerializeField] private Animation damageEffectAnim;
    private float currentHealth;
    public float maxHealth;
    [SerializeField] private Image fillBar;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Volume volume;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentState = idleHash;
        animator.Play(idleHash);
        playerMaterial.SetFloat(floatPropertyName,0f);
        currentHealth =  maxHealth;
        UpdateHealthUi();
        //playerMaterial = skinnedMeshRenderer.material;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            die = true;
            PlayAnim(winHash);
            CameraShake.instance.ChangeFov(40);
        }
        DetectClimbable();
        HandleMovement();
        HandleJumpAndGravity();
        HandleAnimation();
    }

    // --------------------------------------
    void DetectClimbable()
    {
        if (isClimbingUp || die) return; // prevent detection while climb-up animation is playing

        // Forward ray to detect climbable wall
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, climbCheckDistance, wallLayer))
        {
            isClimbing = true;
            velocity = Vector3.zero;
            buildingTopY = hit.collider.bounds.max.y;
            // Check if there's no wall above (i.e., top reached)
            if (!isClimbingUp && !Physics.Raycast(transform.position + Vector3.up * 1.75f, transform.forward, 0.5f))
            {
                TriggerClimbUp();
            }
            return;
        }
        isClimbing = false;
    }
    
    void TriggerClimbUp()
    {
        print("TriggerClimbUp");
        isClimbing = false;
        isClimbingUp = true;
        velocity = Vector3.zero;
        Vector3 toPos = new Vector3(transform.position.x, buildingTopY, transform.position.z);
        transform.DOJump(toPos + transform.forward * 0.5f, 0.3f, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            isClimbingUp = false;
        });
        PlayAnim(climbUpHash);
    }

    // --------------------------------------
    void HandleMovement()
    {
        if (isClimbingUp || die) return; // block input

        moveDir = new Vector3(joystick.Horizontal, 0f, joystick.Vertical).normalized;
        camForward = cam.forward;
        camForward.y = 0f;
        camForward.Normalize();
 
        camRight = cam.right;
        camRight.y = 0f;
        camRight.Normalize();
 
        //Find Movement Direction
        move = camForward * moveDir.z + camRight * moveDir.x;
        move = move.normalized;
        isMoving = move.magnitude > 0.1f;

        if (isClimbing)
        {
            float verticalInput = joystick.Vertical;
            if (verticalInput > 0.1f)
            {
                Vector3 climbMove = Vector3.up * climbSpeed * Time.deltaTime;
                controller.Move(climbMove);
            }
            return;
        }

        if (isMoving)
        {
            controller.Move(move * moveSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime * 100f
            );
        }
    }

    // --------------------------------------
    void HandleJumpAndGravity()
    {
        if (isClimbing || isClimbingUp) return; // disable gravity during climb or climb-up

        bool isGrounded = controller.isGrounded || (velocity.y < 0 && Physics.Raycast(transform.position, Vector3.down, 0.2f));

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !die)
        {
            velocity.y = jumpForce;
            isJumping = true;
            PlayAnim(jumpHash);
        }

        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f;
            isJumping = false;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    // --------------------------------------
    void HandleAnimation()
    {
        if (die)
        {
            return;
        }
        
        if (isClimbingUp)
        {
            PlayAnim(climbUpHash);
            return;
        }

        if (isClimbing)
        {
            PlayAnim(climbHash);
            return;
        }

        if (!isJumping)
        {
            if (isMoving)
                PlayAnim(runHash);
            else
                PlayAnim(idleHash);
        }
    }

    // --------------------------------------
    void PlayAnim(int targetHash)
    {
        if (currentState == targetHash) return;

        animator.CrossFadeInFixedTime(targetHash, 0.25f);
        currentState = targetHash;
    }

     // Assign the Global Volume Profile from Project Settings
    private ColorAdjustments colorAdjustments;
    private bool die;
    public void Damage(float damage)
    {
        if(die == true) return;
        DOTween.Kill(playerMaterial); // Prevent stacking animations
        damageEffectAnim.Play();
        currentHealth -= damage;
        UpdateHealthUi();
        
        if (currentHealth <= 0)
        {
            print("Die");
            gameObject.tag = "Untagged";
            die = true;
            PlayAnim(deathHash);
            GetComponent<Shooting>().enabled = false;
            if (volume != null && volume.profile.TryGet(out colorAdjustments))
            {
                colorAdjustments.saturation.value = -60f; // Initial value
            }
            else
            {
                Debug.LogWarning("Color Adjustments not found in the profile!");
            }
            CameraShake.instance.ChangeFov(90);
            Time.timeScale = 0.35f;
        }
        playerMaterial.DOFloat(1f, floatPropertyName, 0.05f)
            .OnComplete(() =>
            {
                playerMaterial.DOFloat(0f, floatPropertyName, 0.05f);
            });
    }
    

    void UpdateHealthUi()
    {
        fillBar.fillAmount = (float) currentHealth / maxHealth;
        if (currentHealth <= 0)
        {
            healthBar.SetActive(false);
            
        }
    }

    void LateUpdate()
    {
        healthBar.transform.LookAt(transform.position + cam.transform.forward);
    }
}
