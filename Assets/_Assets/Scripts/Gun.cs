using UnityEngine;

public class Gun : MonoBehaviour
{
    public ParticleSystem bulletMuzzle;
    public Transform bulletSpawnPos;
    public float fireRate;

    private int[] spreadAngles = new[] {-4,-2,-1,0, 1,2,4};

    public void Shoot(Transform target, int index = 0)
    {
        if (index == 0)
        {
            Bullet bullet = ObjectPooling.Instance.Spawn<Bullet>(PoolType.BulletGreen,bulletSpawnPos.position);
            if (bullet != null)
            {
                bullet.Launch(target);
            }
        }
        if (index == 1)
        {
            for (int i = 0; i < 7; i++)
            {
                Bullet bullet = ObjectPooling.Instance.Spawn<Bullet>(PoolType.BulletRed,bulletSpawnPos.position);
                if (bullet != null)
                {
                    bullet.Launch(Quaternion.AngleAxis(spreadAngles[i],Vector3.up) * target.position);
                }
            }
        }
        if (index == 2)
        {
            Bullet bullet = ObjectPooling.Instance.Spawn<Bullet>(PoolType.missile,bulletSpawnPos.position);
            if (bullet != null)
            {
                bullet.Launch(target);
            }
        }
        bulletMuzzle.Play();
    }
}
