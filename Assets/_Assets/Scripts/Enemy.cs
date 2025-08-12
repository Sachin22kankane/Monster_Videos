using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : PoolableObject
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 6f;
    public float gravity = -20f;
    public float climbSpeed = 3f;
    public float climbCheckDistance = 1f;
    public LayerMask wallLayer;
    
    public bool isDead;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform target;
    
    private readonly int idleHash = Animator.StringToHash("Idle");
    private readonly int runHash = Animator.StringToHash("Running");
    private readonly int jumpHash = Animator.StringToHash("Jump");
    private readonly int climbHash = Animator.StringToHash("Climb");
    private readonly int climbUpHash = Animator.StringToHash("ClimbUp");
    private readonly int deathHash = Animator.StringToHash("Death");
    private int currentState;
    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 moveDir;
    private Vector3 offset;
    private bool isJumping;
    private bool isMoving;
    private bool isClimbing;
    private bool isClimbingUp;
    private float buildingTopY;
    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        target = PlayerController.instance.transform;
        currentState = idleHash;
        animator.Play(idleHash);
        offset = new Vector3(Random.Range(-3,3),0,Random.Range(-1,1));
        moveSpeed = Random.Range(3.5f, 5.5f);
    }

    void Update()
    {
        if(isDead) return;
        DetectClimbable();
        HandleMovement();
        HandleJumpAndGravity();
        HandleAnimation();
    }

    // --------------------------------------
    void DetectClimbable()
    {
        if (isClimbingUp) return; // prevent detection while climb-up animation is playing

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
        if (isClimbingUp) return; // block input
        if (isClimbing)
        {
            float verticalInput = 1f;
            if (verticalInput > 0.1f)
            {
                Vector3 climbMove = Vector3.up * climbSpeed * Time.deltaTime;
                controller.Move(climbMove);
            }
            return;
        }
        
        moveDir = target.position - transform.position + offset;
        if (Vector3.Distance(target.position, transform.position) < 2f)
        {
            moveDir = Vector3.zero;
        }
        moveDir.y = 0;
        moveDir = moveDir.normalized;
        isMoving = moveDir.magnitude > 0.1f;

        if (isMoving)
        {
            controller.Move(moveDir * moveSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
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

    public void Dead()
    {
        BloodParticle bloodParticle = ObjectPooling.Instance.Spawn<BloodParticle>(transform.position + new Vector3(0,1,0));
        bloodParticle.Play(-transform.forward);
        GetComponent<Collider>().enabled = false;
        isDead = true;
        PlayAnim(deathHash);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            gameObject.SetActive(false);
        });
        //pool.Release(this);
    }

    public void Active(Vector3 spawnPos)
    {
        transform.position = spawnPos;
        gameObject.SetActive(true);
    }
    
    
}
