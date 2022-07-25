using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wwise_Audio : MonoBehaviour
{
    public AK.Wwise.Bank m_MusicBank = null;
    public AK.Wwise.Switch m_Switch_Fight = null;
    public AK.Wwise.Switch m_Switch_Boss = null;
    public AK.Wwise.Switch m_Switch_Boss_Dead = null;
    public AK.Wwise.Event m_MusicEvent;

    private GameScene m_GameScene;
    private bool m_ActiveStageName;
    private bool m_ActiveBossState;
    void Awake()
    {
        m_ActiveBossState = false;
        m_MusicBank.Load();
    }

    // Start is called before the first frame update
    void Start() 
    {   
        m_GameScene = GetComponent<GameScene>();
        m_Switch_Fight.SetValue(gameObject);
        m_MusicEvent.Post(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void MusicChoice () {
        m_ActiveStageName = m_GameScene.m_IsBossLevel;
        m_ActiveBossState = m_GameScene.m_BossIsDead;
        if (m_ActiveStageName == true)
        {
            m_Switch_Boss.SetValue(gameObject);
        }
        else if (m_ActiveBossState == true)
        {
            m_Switch_Boss_Dead.SetValue(gameObject);
        }
        else
        {
            m_Switch_Fight.SetValue(gameObject);
        }
    }
}
