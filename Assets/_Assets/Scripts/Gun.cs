using UnityEngine;

public class Gun : MonoBehaviour
{
    public ParticleSystem bulletMuzzle;
    public Transform bulletSpawnPos;
    public float fireRate;

    public void Shoot(Transform target, int index = 0)
    {
        if (index == 0)
        {
            Bullet bullet1 = ObjectPooling.Instance.Spawn<Bullet>(bulletSpawnPos.position);
            bullet1.Launch(target.GetComponent<Enemy>());
        }
        if (index == 1)
        {
            Bullet bullet1 = ObjectPooling.Instance.Spawn<Bullet>(bulletSpawnPos.position);
            Bullet bullet2 = ObjectPooling.Instance.Spawn<Bullet>(bulletSpawnPos.position);
            Bullet bullet3 = ObjectPooling.Instance.Spawn<Bullet>(bulletSpawnPos.position);
            // bullet1.Launch(target.position);
            // bullet2.Launch( Quaternion.AngleAxis(1.5f, Vector3.up) * target.position);
            // bullet3.Launch( Quaternion.AngleAxis(-1.5f, Vector3.up) * target.position);
        }
        bulletMuzzle.Play();
    }
}
