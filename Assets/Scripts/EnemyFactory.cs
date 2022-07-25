using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyFactory : MonoBehaviour
{
    public Enemy m_Spinner;
    public EnemyTest01 m_Tracker;
    // add other enemy types once implemented

    private StageManager m_StageManager;
    void Start()
    {
        m_StageManager = FindObjectOfType<StageManager>();
    }

    public void CreateSpinner(Vector3 enemyPos, bool isBoss)
    {
        var stats = m_StageManager.GetBaseStats(false);
        var enemy = Instantiate(m_Spinner, enemyPos, Quaternion.identity);
        enemy.SetBaseStats(stats);
        enemy.m_IsBoss = isBoss;
    }

    public void CreateTracker(Vector3 enemyPos, bool isBoss)
    {
        var stats = m_StageManager.GetBaseStats(false);                     //
        var enemy = Instantiate(m_Tracker, enemyPos, Quaternion.identity);
        enemy.m_EnemyStats = m_StageManager.GetBaseStats(false);            // add same function as in Enemy
    }

    public void CreateEnemyOfType(GameObject EnemyType, Vector3 enemyPos)
    {
        //Debug.Log("CreateEnemyCalled");
        //Stats stats = m_StageManager.GetBaseStats(false);
        if (EnemyType.name == "Spinner")
        {
            Stats stats = m_StageManager.GetBaseStats(false);
            Enemy enemy = Instantiate(m_Spinner, enemyPos, Quaternion.identity);
            enemy.SetBaseStats(stats);
        }
        else if(EnemyType.name == "Enemy_Test01")
        {
            
            EnemyTest01 enemy = Instantiate(m_Tracker, enemyPos, Quaternion.identity);
            enemy.m_EnemyStats = m_StageManager.GetBaseStats(false);
            enemy.m_IsBoss = true;
            

        }
        // add other Enemy Types once implemented
        
    }
}
