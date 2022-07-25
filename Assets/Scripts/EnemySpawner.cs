using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private EnemyFactory m_EnemyFactory;
    void Start()
    {
        m_EnemyFactory = FindObjectOfType<EnemyFactory>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            m_EnemyFactory.CreateSpinner(transform.position, false);

        if (Input.GetKeyDown(KeyCode.Q))
            m_EnemyFactory.CreateTracker(transform.position, true);
    }

    public void SpawnEnemyOfTypeAtPosition(GameObject EnemyType, Transform EnemySpawnPosition)
    {
        m_EnemyFactory.CreateEnemyOfType(EnemyType, EnemySpawnPosition.position);
    }
}
