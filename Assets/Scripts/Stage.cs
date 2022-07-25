using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Stage : MonoBehaviour
{
    // Variables
    public int m_StageType = 6; // increase by 1 (0 is re-roll level)
    public int m_PlayerBaseStat_HP = 0;
    public int m_PlayerBaseStat_DMG = 0;
    public float m_PlayerBaseStat_Speed = 0;
    public float m_PlayerBaseStat_AttackRate = 0;

    public int m_EnemyBaseStat_HP = 0;
    public int m_EnemyBaseStat_DMG = 0;
    public float m_EnemyBaseStat_Speed = 0;
    public float m_EnemyBaseStat_AttackRate = 0;

    public Transform[] PlayerSpawns;
    public int m_SelectedLevel = 0;
    public string m_StartingSceneName = " ";
    public string m_ActiveScene = " ";
    public bool m_StageCleared = false;

    public bool[] m_LevelsVisited;
    private bool m_IsSelectedLevelValid = false;

    private StageManager r_StageManager;

    // Functions
    private int GetNextLevelToLoad()
    {
        for (int i = 0; i < m_LevelsVisited.Length; i++)
        {
            Debug.Log("Visited " + i + ": " + m_LevelsVisited[i]);
        }

        m_IsSelectedLevelValid = false;
        while (m_IsSelectedLevelValid == false)
        {
             //m_SelectedLevel = (int)Mathf.Round(Mathf.Pow(Random.Range(0, m_StageType),2)/m_StageType); // x^2 / Max Level floored
             m_SelectedLevel = Mathf.RoundToInt(Random.Range(0, 7));
             if (m_LevelsVisited[m_SelectedLevel] == true)
             {
                    m_IsSelectedLevelValid = false;
                Debug.Log("Level " + m_SelectedLevel + " was already visited");
             }
             else
             {
                m_IsSelectedLevelValid = true;
                m_LevelsVisited[m_SelectedLevel] = true;
                Debug.Log("Level" + m_SelectedLevel + " was not visited yet");
                
             }
            
        }



        Debug.Log("NEXT LEVEL TO GO TO: " + m_SelectedLevel);
        return m_SelectedLevel;
    }

    public void LoadNextLevel(string Entrance)
    {
        if(m_SelectedLevel != m_StageType)
        {
            m_SelectedLevel = GetNextLevelToLoad();
            Debug.Log("LEVEL ABOUT TO BE LOADED: " + "D" + m_StageType + "_" + m_SelectedLevel);
            if(m_SelectedLevel != 6)
            SceneManager.LoadSceneAsync("D" + m_StageType + "_" + m_SelectedLevel);
            else
                SceneManager.LoadSceneAsync("D6_6");
            //SceneManager.UnloadSceneAsync(m_ActiveScene);



            Debug.Log("ActiveScene: " + SceneManager.GetActiveScene().name);

            switch(Entrance)
            {
                case "West":
                    r_StageManager.r_PlayerObject.GetComponent<Transform>().position = PlayerSpawns[1].localPosition;
                    break;
                case "South":
                    r_StageManager.r_PlayerObject.GetComponent<Transform>().position = PlayerSpawns[2].localPosition;
                    break;
                case "East":
                    r_StageManager.r_PlayerObject.GetComponent<Transform>().position = PlayerSpawns[3].localPosition;
                    break;
                case "North":
                    r_StageManager.r_PlayerObject.GetComponent<Transform>().position = PlayerSpawns[4].localPosition;
                    break;
            }
                
        }
        else
        {
            Debug.Log("Game Finished");
            SceneManager.LoadSceneAsync("FINAL");
        }

        
    }

    // Start is called before the first frame update
    private void Start()
    {
        Object.DontDestroyOnLoad(this.gameObject);
        m_LevelsVisited = new bool[m_StageType + 1];
        r_StageManager = FindObjectOfType<StageManager>().GetComponent<StageManager>();

    }

    private void Update()
    {
        m_ActiveScene = SceneManager.GetActiveScene().name;
    }

}
