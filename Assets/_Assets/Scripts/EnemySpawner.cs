using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform player; // Player to spawn around
    public int enemiesPerWave = 5; // How many enemies per spawn
    public float spawnRadius = 10f; // Distance from player to spawn
    public float spawnInterval = 5f; // Time between spawns

    [Header("Obstacle Avoidance")] public LayerMask obstacleLayer; // Layer of buildings or things to avoid
    public float checkRadius = 1f; // Size of check to avoid overlapping buildings

    private float timer = 0f;

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemies();
            timer = 0f;
        }
    }

    void SpawnEnemies()
    {
        int spawned = 0;
        int attempts = 0;

        while (spawned < enemiesPerWave && attempts < enemiesPerWave * 2)
        {
            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection.y = 0;
            randomDirection.Normalize();

            Vector3 spawnPos = player.position + randomDirection * spawnRadius;

            // Check for obstacles (like buildings)
            if (!Physics.CheckSphere(spawnPos, checkRadius, obstacleLayer))
            {
                //Boid boid = ObjectPooling.Instance.Spawn<Boid>(spawnPos);
                //boid.ActiveEnemy(player);
                spawned++;
            }

            attempts++;
        }
    }
}

