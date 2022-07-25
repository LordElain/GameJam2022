using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stats
{
    public int m_Stat_Total = 0;
    public int m_Stat_HP = 0;
    public int m_Stat_DMG = 0;
    public float m_Stat_Speed = 0;
    public float m_Stat_AttackRate = 0;

    public Stats()
    {

    }
}


public class StageManager : MonoBehaviour
{
    // member variables
    public string m_StartingStageName;      // persistent Stage Object with respective name required!
    public string m_NextStageName;          // persistent Stage Object with respective name required!
    public GameObject m_ActiveStageObject;
    public Stage m_ActiveStage;
    public PlayerMovement r_PlayerObject;

    public int m_StandardPlayerHPIncrease = 1;
    public int m_StandardPlayerDMGIncrease = 1;
    public float m_StandardPlayerSpeedIncrease = 0.1f;
    public float m_StandardPlayerAttackRateIncrease = 0.1f;

    public int m_StandardEnemyHPIncrease = 1;
    public int m_StandardEnemyDMGIncrease = 1;
    public float m_StandardEnemySpeedIncrease = 0.1f;
    public float m_StandardEnemyAttackRateIncrease = 0.1f;

    // member functions
    public Stats GetRollStats(bool isPlayer)
    {
        int[] StatDistribution = new int[4];
        int numberOfStatPoints = Random.Range(1, m_ActiveStage.m_SelectedLevel);
        while(numberOfStatPoints > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                if(Random.Range(0,100) > 75)
                {
                    StatDistribution[i]++;
                    numberOfStatPoints--;
                }
            }
        }
        Stats NewStats = new Stats();
        if (isPlayer) // Player needs to call this every time they enter a new level (call right after LoadNextLevel)
        {
            //Debug.Log("Called by Player");
            NewStats.m_Stat_HP = m_StandardPlayerHPIncrease * StatDistribution[0];
            NewStats.m_Stat_DMG = m_StandardPlayerDMGIncrease * StatDistribution[1];
            NewStats.m_Stat_Speed = m_StandardPlayerSpeedIncrease * StatDistribution[2];
            NewStats.m_Stat_AttackRate = m_StandardPlayerAttackRateIncrease * StatDistribution[3];
        }
        else // Enemies need to call this on Start and add the results to their stats
        {
            //Debug.Log("Called by Enemy");
            NewStats.m_Stat_HP = m_StandardEnemyHPIncrease * StatDistribution[0];
            NewStats.m_Stat_DMG = m_StandardEnemyDMGIncrease * StatDistribution[1];
            NewStats.m_Stat_Speed = m_StandardEnemySpeedIncrease * StatDistribution[2];
            NewStats.m_Stat_AttackRate = m_StandardEnemyAttackRateIncrease * StatDistribution[3];
        }

        return NewStats;

    }

    public Stats GetBaseStats(bool IsPlayer) // retrieves Base stats for current stage. Has to be called by player on LoadNextStage. Player needs to overwrite his stats with these
    {
        Stats NewStats = new Stats();
        if (IsPlayer)   // PLAYER BASE STATS
        {
            NewStats.m_Stat_HP = m_ActiveStage.m_PlayerBaseStat_HP;                   // Health Points
            NewStats.m_Stat_DMG = m_ActiveStage.m_PlayerBaseStat_DMG;                 // Damage Points
            NewStats.m_Stat_Speed = m_ActiveStage.m_PlayerBaseStat_Speed;             // Speed
            NewStats.m_Stat_AttackRate = m_ActiveStage.m_PlayerBaseStat_AttackRate;   // Attack Rate
        }
        else            // ENEMY BASE STATS
        {
            NewStats.m_Stat_HP = m_ActiveStage.m_EnemyBaseStat_HP;                   // Health Points
            NewStats.m_Stat_DMG = m_ActiveStage.m_EnemyBaseStat_DMG;                 // Damage Points
            NewStats.m_Stat_Speed = m_ActiveStage.m_EnemyBaseStat_Speed;             // Speed
            NewStats.m_Stat_AttackRate = m_ActiveStage.m_EnemyBaseStat_AttackRate;   // Attack Rate
        }


        return NewStats;
    }


    public bool LoadNextStage(string StageName) // should be called by the Portal prefab to the next Stage
    {
        m_ActiveStageObject = GameObject.Find(StageName);
        m_ActiveStage = m_ActiveStageObject.GetComponent<Stage>();
        SceneManager.LoadScene(m_ActiveStage.m_StartingSceneName);
        return true;
    }

    public void LoadNextLevel(string Exit) // should be called by the portal prefab to the next randomized level
    {
        string Entrance;
        switch(Exit)
        {
            case "West":
                Entrance = "East";
                break;
            case "South":
                Entrance = "North";
                break;
            case "East":
                Entrance = "West";
                break;
            case "North":
                Entrance = "South";
                break;
            case "Start":
                Entrance = "Start";
                break;
            default:
                Entrance = "";
                break;
        }

        m_ActiveStage.LoadNextLevel(Entrance); 
    }

    public void LoadStartScene()
    {
        SceneManager.LoadSceneAsync(m_ActiveStage.m_StartingSceneName);     // Load Starting Scene of the Stage
        this.r_PlayerObject.GetComponent<Transform>().position = m_ActiveStage.PlayerSpawns[0].localPosition;
        this.m_ActiveStage.m_LevelsVisited[1] = true;
    }


    // Start is called before the first frame update
    void Start()
    {
        Object.DontDestroyOnLoad(this.gameObject);                          // ensure persistence
        m_ActiveStageObject = GameObject.Find(m_StartingStageName);         //
        m_ActiveStage = m_ActiveStageObject.GetComponent<Stage>();          // Get Starting Stage       
        m_ActiveStage.m_ActiveScene = SceneManager.GetActiveScene().name;   // Set Active Scene Reference
        r_PlayerObject = FindObjectOfType<PlayerMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

