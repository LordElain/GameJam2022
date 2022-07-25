using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public bool m_HasRolled;

    private GameObject m_PausePanel;
    private GameObject m_PlayerStats;
    private GameObject m_GameOverPanel;
    private PlayerMovement m_Player;
    private StageManager m_StageManager;
    private TextMeshProUGUI m_HPIncrease;
    private TextMeshProUGUI m_DMGIncrease;
    private TextMeshProUGUI m_MSIncrease;
    private TextMeshProUGUI m_ASIncrease;
    private Stats m_StatIncreases;

    private bool m_ShowRandom;
    private bool m_TextDone;
    private bool m_FirstCheck;
    private int m_BossHealthAll;
    private int m_BossHealth;
    void Start()
    {
        m_ShowRandom = false;
        m_HasRolled = false;
        m_TextDone = false;
        m_PausePanel = GameObject.Find("PausePanel");
        m_PlayerStats = GameObject.Find("PlayerStats");
        m_GameOverPanel = GameObject.Find("GameOverPanel");
        m_Player = FindObjectOfType<PlayerMovement>();
        if (m_PausePanel != null)
            m_PausePanel.SetActive(false);
        if (m_GameOverPanel)
            m_GameOverPanel.SetActive(false);
        SceneManager.sceneLoaded += NewSceneLoaded;
        if (m_PlayerStats)
        {
            m_DMGIncrease = m_PlayerStats.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
            m_MSIncrease = m_PlayerStats.transform.GetChild(5).GetComponent<TextMeshProUGUI>();
            m_ASIncrease = m_PlayerStats.transform.GetChild(6).GetComponent<TextMeshProUGUI>();
            m_HPIncrease = m_PlayerStats.transform.GetChild(7).GetComponent<TextMeshProUGUI>();
            m_HPIncrease.alpha = 0;
            m_DMGIncrease.alpha = 0;
            m_MSIncrease.alpha = 0;
            m_ASIncrease.alpha = 0;
        }
        m_StageManager = GetComponent<StageManager>();
    }

    void Update()
    {
        SetStatText();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseButton();
        }

        if (m_ShowRandom)
            SetNumberRandom();

        if (m_TextDone && m_ASIncrease.alpha >= 0)
        {
            m_HPIncrease.alpha -= 0.5f * Time.deltaTime;
            m_DMGIncrease.alpha -= 0.5f * Time.deltaTime;
            m_MSIncrease.alpha -= 0.5f * Time.deltaTime;
            m_ASIncrease.alpha -= 0.5f * Time.deltaTime;
        }
    }

    //Public functions
    public void PauseButton()
    {
        Time.timeScale = Time.timeScale == 1 ? 0 : 1;
        m_PausePanel.SetActive(!m_PausePanel.activeInHierarchy);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartScene()
    {
        m_StageManager.LoadStartScene();
    }

    public void SetStats(Stats stats)
    {
        m_StatIncreases = stats;
        StartCoroutine(nameof(ShowRandom));
    }

    public void GameOver()
    {
        m_GameOverPanel.SetActive(true);
        if (Time.timeScale > 0)
            Time.timeScale -= 2 * Time.deltaTime;
    }

    //Private functions
    private void NewSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        m_ShowRandom = false;
        m_PausePanel = GameObject.Find("PausePanel");
        m_PlayerStats = GameObject.Find("PlayerStats");
        m_Player = FindObjectOfType<PlayerMovement>();
        if (m_PausePanel != null)
            m_PausePanel.SetActive(false);
        m_GameOverPanel = GameObject.Find("GameOverPanel");
        if (m_GameOverPanel)
            m_GameOverPanel.SetActive(false);
        if (m_PlayerStats)
        {
            m_DMGIncrease = m_PlayerStats.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
            m_MSIncrease = m_PlayerStats.transform.GetChild(5).GetComponent<TextMeshProUGUI>();
            m_ASIncrease = m_PlayerStats.transform.GetChild(6).GetComponent<TextMeshProUGUI>();
            m_HPIncrease = m_PlayerStats.transform.GetChild(7).GetComponent<TextMeshProUGUI>();
            m_DMGIncrease.alpha = 0;
            m_MSIncrease.alpha = 0;
            m_ASIncrease.alpha = 0;
            m_HPIncrease.alpha = 0;
        }
    }

    private void SetStatText()
    {
        if (!m_Player || m_ShowRandom) return;
        var stats = m_Player.GetPlayerStats();
        m_PlayerStats.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"HP: {stats.m_Stat_HP}";
        m_PlayerStats.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"DMG: {stats.m_Stat_DMG}";
        m_PlayerStats.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"MS: {stats.m_Stat_Speed.ToString("n2")}";
        m_PlayerStats.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"AS: {stats.m_Stat_AttackRate.ToString("n2")}";
    }

    private void SetNumberRandom()
    {
        m_TextDone = false;
        m_DMGIncrease.alpha = 1;
        m_MSIncrease.alpha = 1;
        m_ASIncrease.alpha = 1;
        m_HPIncrease.alpha = 1;

        if (m_StatIncreases.m_Stat_HP == 0)
            m_HPIncrease.text = $"+ {Random.Range(0, m_StatIncreases.m_Stat_HP + 1 * 3)}";
        else
            m_HPIncrease.text = $"+ {Random.Range(0, m_StatIncreases.m_Stat_HP * 3)}";

        if (m_StatIncreases.m_Stat_DMG == 0)
            m_DMGIncrease.text = $"+ {Random.Range(0, m_StatIncreases.m_Stat_DMG + 1 * 3)}";
        else
            m_DMGIncrease.text = $"+ {Random.Range(0, m_StatIncreases.m_Stat_DMG * 3)}";

        if (m_StatIncreases.m_Stat_Speed == 0)
            m_MSIncrease.text = $"+ {Random.Range(0, m_StatIncreases.m_Stat_Speed + 0.1f * 3).ToString("n2")}";
        else
            m_MSIncrease.text = $"+ {Random.Range(0, m_StatIncreases.m_Stat_Speed * 3).ToString("n2")}";

        if (m_StatIncreases.m_Stat_AttackRate == 0)
            m_ASIncrease.text = $"- {Random.Range(0, m_StatIncreases.m_Stat_AttackRate + 0.1f * 3).ToString("n2")}";
        else
            m_ASIncrease.text = $"- {Random.Range(0, m_StatIncreases.m_Stat_AttackRate * 3).ToString("n2")}"; 
    }

    private IEnumerator ShowRandom()
    {
        m_ShowRandom = true;
        yield return new WaitForSeconds(1.5f);
        m_ShowRandom = false;
        m_HPIncrease.text = $"+ {m_StatIncreases.m_Stat_HP}";
        m_DMGIncrease.text = $"+ {m_StatIncreases.m_Stat_DMG}";
        m_ASIncrease.text = $"- {m_StatIncreases.m_Stat_AttackRate.ToString("n2")}";
        m_MSIncrease.text = $"+ {m_StatIncreases.m_Stat_Speed.ToString("n2")}";
        m_TextDone = true;
    }
}
