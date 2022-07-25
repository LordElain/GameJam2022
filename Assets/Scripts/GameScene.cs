using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    public int LevelNumber = 0;
    public Transform[] m_EnemyTransforms;
    public GameObject m_EnemyType;
    public GameObject[] m_Gates;

    public bool m_IsBossLevel = false;
    public bool m_BossIsDead = false;
    private EnemySpawner r_EnemySpawner;
    private StageManager r_StageManager;
    private int m_NumberOfEnemiesAlive;

    // Start is called before the first frame update
    void Start()
    {
        r_StageManager = FindObjectOfType<StageManager>().GetComponent<StageManager>();
        if (LevelNumber == r_StageManager.m_ActiveStage.m_StageType)
            m_IsBossLevel = true;

        r_EnemySpawner = FindObjectOfType<EnemySpawner>();
        for(int i = 0; i < m_EnemyTransforms.Length; i++)
        {
            r_EnemySpawner.SpawnEnemyOfTypeAtPosition(m_EnemyType, m_EnemyTransforms[i]);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        m_NumberOfEnemiesAlive = FindObjectsOfType<Enemy>().Length;
        m_NumberOfEnemiesAlive += FindObjectsOfType<EnemyTest01>().Length;
        //Debug.Log(m_NumberOfEnemiesAlive);
        if (m_NumberOfEnemiesAlive == 0)
        {
            ActivateAllGates();
        }
    }

    public void ActivateAllGates()
    {
        for (int i = 0; i < m_Gates.Length; i++)
        {
            m_Gates[i].GetComponent<NextLevelGateScript>().m_GateActive = true;

        }
    }
}
