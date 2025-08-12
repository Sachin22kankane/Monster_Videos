using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Boid : PoolableObject
{
    [SerializeField] private float m_Speed = 4f;
    [SerializeField] private float m_RotationSpeed = 8f;
    [SerializeField] private float m_StopDistance = 1.5f;
    public float climbSpeed = 3f;
    public float climbCheckDistance = 1f;
    private float m_VerticalVelocity = 0f;
    [SerializeField] private float m_Gravity = -9.81f;
    public LayerMask wallLayer;// Distance at which the enemy stops following

    [Header("Boids")]
    [SerializeField]
    private float m_DetectionDistance = 1f;
    [SerializeField] private float m_SeparationWeight = 2f;
    [SerializeField] private float m_AlignmentWeight = 1.0f;
    [SerializeField] private float m_CohesionWeight = 1.0f;

    public Animator animator;
    public Transform m_Target;

    private Vector3 m_Direction = Vector3.zero;
    private Quaternion m_TargetRotation;
    private float m_MovementSpeedBlend;
    private Vector3 m_SeparationForce;
    
    private readonly int idleHash = Animator.StringToHash("Idle");
    private readonly int runHash = Animator.StringToHash("Running");
    private readonly int jumpHash = Animator.StringToHash("Jump");
    private readonly int climbHash = Animator.StringToHash("Climb");
    private readonly int climbUpHash = Animator.StringToHash("ClimbUp");
    private readonly int deathHash = Animator.StringToHash("Death");
    private int currentState;
    public bool isDead = false;
    
    private bool isClimbing;
    private bool isClimbingUp;
    private float buildingTopY;

    private void OnEnable()
    {
        currentState = idleHash;
        animator.Play(idleHash);
    }

    void Update()
    {
        if (m_Target != null && isDead == false)
        {
            FollowTarget();
        }
    }

    private void FixedUpdate()
    {
        DetectClimbable();
    }

    bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 1f; // Slightly above feet
        return Physics.Raycast(origin, Vector3.down, 1.1f);
    }
    
    void DetectClimbable()
    {
        if (isClimbingUp) return;
        
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, climbCheckDistance, wallLayer))
        {
            isClimbing = true;
            buildingTopY = hit.collider.bounds.max.y;
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
        isClimbing = false;
        isClimbingUp = true;
        //velocity = Vector3.zero;
        Vector3 toPos = new Vector3(transform.position.x, buildingTopY, transform.position.z);
        DOTween.Kill(transform);
        transform.DOJump(toPos + transform.forward * 0.5f, 0.3f, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            isClimbingUp = false;
        });
        PlayAnim(climbUpHash);
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
    }
    

    private void FollowTarget()
    {
        if (isClimbingUp) return; // block input
        if (isClimbing)
        {
            Vector3 climbMove = Vector3.up * climbSpeed * Time.deltaTime;
            transform.position += climbMove;
            return;
        }
        
        m_SeparationForce = Vector3.zero;
        m_Direction = (m_Target.position - transform.position);
        float distance = m_Direction.magnitude;
        m_Direction = m_Direction.WithNewY(0);
        m_Direction = m_Direction.normalized;

        var neigbours = GetNeighbours();

        if (neigbours.Length > 0)
        {
            CalculateSeparationForce(neigbours);
            ApplyAllignment(neigbours);
            ApplyCohesion(neigbours);
        }

        ApplyGravity();
        if (distance > m_StopDistance)
        {
            MoveTowardsTarget();
        }
        else
        {
            StopMove();
        }

        RotateTowardsTarget();
    }

    // 1. Define Neighbor Detection
    private Collider[] GetNeighbours()
    {
        var enemyMask = LayerMask.GetMask("enemy");
        return Physics.OverlapSphere(transform.position, m_DetectionDistance, enemyMask);
    }

    // 2. Separation Rule
    private void CalculateSeparationForce(Collider[] neighbours)
    {
        foreach (var neighbour in neighbours)
        {
            var dir = neighbour.transform.position - transform.position;
            var distance = dir.magnitude;
            var away = -dir.normalized;

            if (distance > 0)
            {
                m_SeparationForce += (away / distance) * m_SeparationWeight;
            }
        }
    }

    private void ApplyAllignment(Collider[] neighbours)
    {
        Vector3 neighboursForward = Vector3.zero;

        foreach (var neighbour in neighbours)
        {
            neighboursForward += neighbour.transform.forward;
        }

        if (neighboursForward != Vector3.zero)
        {
            neighboursForward.Normalize();
        }

        m_SeparationForce += neighboursForward * m_AlignmentWeight;
    }

    // Step 4
    // The purpose of this rule is to keep the enemies moving toward the center of the group,
    // creating a sense of unity in their movement.
    private void ApplyCohesion(Collider[] neighbours)
    {
        Vector3 averagePosition = Vector3.zero;

        foreach (var neighbour in neighbours)
        {
            averagePosition += neighbour.transform.position;
        }

        averagePosition /= neighbours.Length;

        Vector3 cohesionDir = (averagePosition - transform.position).normalized;
        m_SeparationForce += cohesionDir * m_CohesionWeight;
    }

    private void MoveTowardsTarget()
    {
        var combinedDirection = (m_Direction + m_SeparationForce).normalized;
        Vector3 movement = combinedDirection * m_Speed * Time.deltaTime;
        movement.y += m_VerticalVelocity * Time.deltaTime;
        transform.position += movement;
        PlayAnim(runHash);
    }

    private void StopMove()
    {
        PlayAnim(idleHash);
        //m_Animator.SetFloat("Speed", m_MovementSpeedBlend);
    }

    void ApplyGravity()
    {
        if (!IsGrounded())
        {
            print("ApplyGravity" + gameObject.name);
            m_VerticalVelocity = m_Gravity;
        }
        else
        {
            m_VerticalVelocity = 0f;
        }
    }

    private void RotateTowardsTarget()
    {
        m_TargetRotation = Quaternion.LookRotation(m_Direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRotation, Time.deltaTime * m_RotationSpeed);
    }
    
    void PlayAnim(int targetHash)
    {
        if (currentState == targetHash) return;

        animator.CrossFadeInFixedTime(targetHash, 0.25f);
        currentState = targetHash;
    }
}