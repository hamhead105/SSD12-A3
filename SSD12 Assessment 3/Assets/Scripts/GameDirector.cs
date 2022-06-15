using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * An AI which manages spawning entities into the game as well as controlling the difficulty of the game
 */
public class GameDirector : MonoBehaviour
{
    private float cycleTime = 2f;
    private int difficulty = 1;
    [SerializeField] private int baseSpawnRate = 30; // lower rates result in more enemies being spawned min value 1
    [SerializeField] private int spawnRateDecrement = 1; // for each unsuccessful spawn cycle, the spawnRate decreases so that it is more likely next cycle that enemies will spawn
    [SerializeField] private int spawnRate;

    [SerializeField] private int maxEnemies;

    // Start is called before the first frame update
    void Start()
    {
        spawnRate = baseSpawnRate;
        InvokeRepeating("DecideSpawning", cycleTime, cycleTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * Called every cycle and randomly decides whether to spawn enemies
     */
    public void DecideSpawning()
    {
        int numberOfEnemies = FindObjectsOfType<Enemy>().Length;
        if (numberOfEnemies < maxEnemies)
        {
            if (Random.Range(1, spawnRate) <= 1)
            {
                CallEnemySpawns();
            } 
            else
            {
                spawnRate -= spawnRateDecrement;
            }
        }
    }

    /*
     * Find the closest spawns to the player and call the spawners to instantiate enemies
     */
    public void CallEnemySpawns()
    {
        spawnRate = baseSpawnRate;
        foreach (Spawner spawner in FindObjectsOfType<Spawner>())
        {
            if (Vector3.Distance(spawner.transform.position, FindObjectOfType<Player>().transform.position) < spawner.MaxDistanceToPlayer)
            {
                spawner.SpawnItems();
            }
        }
    }
}
