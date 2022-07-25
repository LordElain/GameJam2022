using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public bool m_InfiniteShoot = false;

    public bool m_IsBoss = false;
    public int m_EnemyHealth = 1;
    public int m_StartHealth;

    //Members
    private float m_EnemySpeed = 3;
    private float m_EnemyAttackSpeed = 1f;
    private float m_BossRotateSpeed = 50;
    private float m_EnemyDamage = 1;
    private float m_EnemyBulletSpeed;

    private bool m_CanShoot;
    private bool m_CanDie;
    private bool m_ParentIsAlive = true;

    private GameObject[] m_GoalPositions;
    private Transform m_CurrentGoal;
    private BulletFactory m_BulletFactory;
    private EnemyFactory m_EnemyFactory;
    private TextMeshProUGUI m_StatsGainedText;
    private StageManager m_StageManager;
    private Slider m_HealthBar;
    public AK.Wwise.Bank m_MusicBank = null;
    public AK.Wwise.Event m_EnemyEvent_Shooting = null;
    public AK.Wwise.Event m_EnemyEvent_Hurt = null;
    public AK.Wwise.Event m_EnemyEvent_Die = null;
    public AK.Wwise.Event m_BossEvent_Shooting = null;
    public AK.Wwise.Event m_BossEvent_Hurt = null;
    public AK.Wwise.Event m_BossEvent_Die = null;
    void Start()
    {
        m_MusicBank.Load();
        m_ParentIsAlive = true;
        m_BulletFactory = FindObjectOfType<BulletFactory>();
        m_EnemyFactory = FindObjectOfType<EnemyFactory>();
        m_CanShoot = true;
        m_CanDie = true;
        m_StageManager = FindObjectOfType<StageManager>();
        m_StatsGainedText = GetComponentInChildren<TextMeshProUGUI>();
        m_HealthBar = GetComponentInChildren<Slider>();
        IncreaseStats(m_StageManager.GetRollStats(false));
        if (m_IsBoss && GameObject.FindObjectsOfType<Enemy>().Length == 5)
        {
            m_EnemyHealth += 40;
        }
        m_StartHealth = m_EnemyHealth;
        m_GoalPositions = GameObject.FindGameObjectsWithTag("Goal");
        foreach (Enemy enemy in GetComponentsInChildren<Enemy>())
        {
            if (enemy != this)
            {
                enemy.m_InfiniteShoot = true;
                enemy.m_IsBoss = false;
            }
        }
    }

    void Update()
    {
        if (m_EnemyHealth <= 0)
        {
            //gameObject.SetActive(false);
            foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
            {
                Destroy(child.gameObject);
            }

            if (m_IsBoss == true)
            {
                if (m_CanDie)
                    m_BossEvent_Die.Post(gameObject);
            }
            else
            {
                if (m_CanDie)
                    m_EnemyEvent_Die.Post(gameObject);
            }

            Destroy(gameObject);
        }
        if (m_HealthBar)
        {
            m_HealthBar.maxValue = m_StartHealth;
            m_HealthBar.value = m_EnemyHealth;
        }

        Movement(m_EnemySpeed);
    }
    //Public functions
    public void Movement(float speed)
    {
        if (m_StatsGainedText && m_StatsGainedText.alpha >= 0)
            m_StatsGainedText.alpha -= 0.5f * Time.deltaTime;

        if (m_IsBoss)
        {
            if (m_EnemyHealth <= m_StartHealth * 0.7f && m_EnemyHealth > 7)
            {
                m_EnemyFactory.CreateSpinner(m_GoalPositions[(Random.Range(0, m_GoalPositions.Length))].transform.position, true);
                m_EnemyFactory.CreateSpinner(m_GoalPositions[(Random.Range(0, m_GoalPositions.Length))].transform.position, true);
                foreach (Enemy enemy in GameObject.FindObjectsOfType<Enemy>())
                {
                    if (enemy.m_IsBoss)
                    {
                        enemy.m_StartHealth = enemy.m_EnemyHealth = m_EnemyHealth;
                    }

                }
                foreach (Enemy enemy in GetComponentsInChildren<Enemy>())
                    enemy.m_ParentIsAlive = false;
                Destroy(gameObject);
            }

            if (m_EnemyHealth >= 30)
            {
                transform.localScale = new Vector3(0.1f, 0.1f, 0);
                m_EnemyBulletSpeed = 10f;
                m_EnemyAttackSpeed = 3;
            }
            else if (m_EnemyHealth >= 10)
            {
                transform.localScale = new Vector3(0.075f, 0.075f, 0);
                m_EnemyBulletSpeed = 15f;
            }
        }

        if (!m_InfiniteShoot)
        {
            if (m_CurrentGoal == null || transform.position == m_CurrentGoal.position)
                m_CurrentGoal = m_GoalPositions[(Random.Range(0, m_GoalPositions.Length))].transform;
            else
            {
                transform.Rotate(new Vector3(0, 0, m_BossRotateSpeed * Time.deltaTime));
                transform.position = Vector3.MoveTowards(transform.position, m_CurrentGoal.position, speed * Time.deltaTime);
            }
        }

        if (m_InfiniteShoot && m_CanShoot && m_ParentIsAlive)
        {
            if (m_IsBoss == true)
            {
                m_BossEvent_Shooting.Post(gameObject);
            }
            else
            {
                m_EnemyEvent_Shooting.Post(gameObject);
            }
            m_BulletFactory.ShootBullet(gameObject, 1, 3);
            StartCoroutine(nameof(ShootDelay));
        }
    }

    public void TakeDamage(int damage)
    {
        if (m_EnemyHealth - damage <= 0)
        {
            foreach (Enemy enemy in GetComponentsInChildren<Enemy>())
                enemy.m_ParentIsAlive = false;
        }
        if (m_IsBoss == true)
        {
            m_BossEvent_Hurt.Post(gameObject);
        }
        else
        {
            m_EnemyEvent_Hurt.Post(gameObject);
        }
        m_EnemyHealth -= damage;
    }

    public IEnumerator ShootDelay()
    {
        m_CanShoot = false;
        yield return new WaitForSeconds(m_EnemyAttackSpeed);
        m_CanShoot = true;
    }

    public void SetBaseStats(Stats baseStats, float rotateSpeed = 50f)
    {
        m_EnemyHealth = baseStats.m_Stat_HP;
        m_EnemySpeed = baseStats.m_Stat_Speed;
        m_EnemyDamage = baseStats.m_Stat_DMG;
        m_EnemyAttackSpeed = baseStats.m_Stat_AttackRate;
        m_BossRotateSpeed = rotateSpeed;
    }

    public void IncreaseStats(Stats stats)
    {
        m_EnemyHealth += stats.m_Stat_HP;
        m_EnemySpeed += stats.m_Stat_Speed;
        m_EnemyAttackSpeed += stats.m_Stat_AttackRate;
        m_EnemyDamage += stats.m_Stat_DMG;
        if (m_StatsGainedText)
            m_StatsGainedText.text = $"{m_EnemyHealth + (m_EnemySpeed * 10) + (m_EnemyAttackSpeed * 10) + m_EnemyDamage}";
    }
}
