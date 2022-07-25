using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable] //needed to make ScriptableObject out of this class
public class GatePositionElement
{
    public enum GatePositions { West, South, East, North, Start };
    public GatePositions GatePosition;
    //public string DialogueText;
}


public class NextLevelGateScript : MonoBehaviour
{
    public enum GatePositions { West, South, East, North };
    public GatePositions GatePosition;
    private string GatePositionText;
    public SpriteRenderer m_GateSpriteRenderer;

    public bool m_GateActive = false;

    private StageManager r_StageManager;
    private PlayerMovement r_PlayerReference;
    BoxCollider2D m_CollisionBox;

    // Start is called before the first frame update
    void Start()
    {
        m_CollisionBox = GetComponent<BoxCollider2D>();
        r_PlayerReference = FindObjectOfType<PlayerMovement>();
        r_StageManager = GameObject.Find("GameManager").GetComponent<StageManager>();
        GatePositionText = GatePosition.ToString();
        //Debug.Log(GatePosition);

        m_GateSpriteRenderer = this.GetComponent<SpriteRenderer>();
        //m_GateActive = false;
        m_GateSpriteRenderer.color = new Color(0, 255, 0, 0);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (m_GateActive)
        {
            if(!r_StageManager.m_ActiveStage.m_StageCleared)
            {
                m_GateSpriteRenderer.color = new Color(0, 255, 0, 1.0f + Mathf.Sin(Time.time * 5));
            }
            else
            {
                m_GateSpriteRenderer.color = new Color(255, 0, 0, 1.0f + Mathf.Sin(Time.time * 5));
            }
            
        }
        else
        {
            m_GateSpriteRenderer.color = new Color(0, 0, 0, 0);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(m_GateActive)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player Entered Portal");
                if (!r_StageManager.m_ActiveStage.m_StageCleared)
                {
                    Debug.Log("Loading Next Level");
                    r_StageManager.LoadNextLevel(GatePositionText);
                    r_PlayerReference.RollStats(); // !
                    r_PlayerReference.PhaseOut();
                }
                else
                {
                    //Debug.Log("Loading Next Stage");
                    //r_StageManager.LoadNextStage(r_StageManager.m_NextStageName);
                    ////r_PlayerReference.RollStats(); // !
                    //SceneManager.LoadScene("FINAL"); 
                }

            }
        }

    }

    public void ActivateGate()
    {
        m_GateActive = true;
    }
}
