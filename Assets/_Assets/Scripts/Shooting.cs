using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Animations.Rigging;

public class Shooting : MonoBehaviour
{
    [SerializeField] private MultiAimConstraint[] rightHandConstraint,leftHandConstraint;
    [SerializeField] private Transform rightHandTarget, leftHandTarget;
    [SerializeField] private float aimRange = 15f;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private float rightOffset, leftOffset;
    private float currentRightWeight, currentLeftWieght;
    private Transform rightTarget, leftTarget;
    [SerializeField] private GunSet[] gunSets;
    private int currentSelectedGunSet;
    [SerializeField] Bullet bulletPrefab;

    [Serializable]
    public class GunSet
    {
        public Gun[] Guns;

        public void EnableGuns(bool enable)
        {
            foreach (var gun in Guns)
            {
                gun.gameObject.SetActive(enable);
            }
        }
    }

    private void Start()
    {
        SelectGunSet(0);
    }

    private void Update()
    {
        SwitchGun();
        DetectRightSide();
        DetectLeftSide();
        //DetectEnemies();
        HandelAim();
        CheckForShoot();
    }

    void DetectRightSide()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position + transform.right * rightOffset, aimRange, enemyLayerMask);
        rightTarget = enemies.Length > 0
            ? enemies.Select(c => c.transform)
                .OrderBy(t => Vector3.Distance(transform.position, t.position))
                .FirstOrDefault()
            : null;

        if (rightTarget != null)
        {
            rightHandTarget.position = Vector3.Lerp(rightHandTarget.position, rightTarget.position + new Vector3(0,0.5f,0), Time.deltaTime * 10);
        }
    }
    
    void DetectLeftSide()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position + transform.right * leftOffset, aimRange, enemyLayerMask);
        leftTarget = enemies.Length > 0
            ? enemies.Select(c => c.transform)
                .OrderBy(t => Vector3.Distance(transform.position, t.position))
                .FirstOrDefault()
            : null;

        if (leftTarget != null)
        {
            leftHandTarget.position = Vector3.Lerp(leftHandTarget.position, leftTarget.position + new Vector3(0,0.5f,0), Time.deltaTime * 10);
        }
    }



    /*void DetectEnemies()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, aimRange, enemyLayerMask);
         targetsInRange = enemies
            .Select(c => c.transform)
             .OrderBy(t => Vector3.Distance(transform.position, t.position))
             .ToList();
        oneInRange = enemies.Length > 0;
        moreThanOneInRange = enemies.Length > 1;
        if (oneInRange)
        {
            rightHandTarget.position = Vector3.Lerp(rightHandTarget.position,targetsInRange[0].position, Time.deltaTime * 10);
            if (moreThanOneInRange)
            {
                leftHandTarget.position = Vector3.Lerp(leftHandTarget.position,targetsInRange[1].position, Time.deltaTime * 10);
            }
        }
    }*/

    private float fireDelay;
    private float elaspedTime;
    void CheckForShoot()
    {
        elaspedTime += Time.deltaTime;
        if (elaspedTime >= fireDelay)
        {
            elaspedTime = 0;
            Shoot();
        }
    }
    
    void Shoot()
    {
        if (leftTarget != null)
        {
            gunSets[currentSelectedGunSet].Guns[0].Shoot(leftTarget,currentSelectedGunSet);
        }
        if (rightTarget != null)
        {
            gunSets[currentSelectedGunSet].Guns[1].Shoot(rightTarget,currentSelectedGunSet);
        }
    }
    

    void HandelAim()
    {
        SetRightHandWeight();
        SetLeftHandWeight();
    }

    void SetRightHandWeight()
    {
        float targetWeight1 = rightTarget != null ? 1f : 0f;
        currentRightWeight = Mathf.Lerp(currentRightWeight, targetWeight1, Time.deltaTime * 10f);
        for (int i = 0; i < rightHandConstraint.Length; i++)
        {
            rightHandConstraint[i].weight = currentRightWeight;
        }
    }
    
    void SetLeftHandWeight()
    {
        float targetWeight2 = leftTarget != null ? 1f : 0f;
        currentLeftWieght = Mathf.Lerp(currentLeftWieght, targetWeight2, Time.deltaTime * 10f);
        for (int i = 0; i < leftHandConstraint.Length; i++)
        {
            leftHandConstraint[i].weight = currentLeftWieght;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + transform.right * rightOffset, aimRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + transform.right *leftOffset, aimRange);
    }

    void SwitchGun()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SelectGunSet(0);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SelectGunSet(1);
        }
    }

    public void SelectGunSet(int gunIndex)
    {
        currentSelectedGunSet = gunIndex;
        fireDelay = gunSets[gunIndex].Guns[0].fireRate;
        for (int i = 0; i < gunSets.Length; i++)
        {
            gunSets[i].EnableGuns(false);
        }
        gunSets[currentSelectedGunSet].EnableGuns(true);
    }
}
