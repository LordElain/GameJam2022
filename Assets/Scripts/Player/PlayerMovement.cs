using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    //Members
    public int m_PlayerDamage = 2;
    
    private float m_PlayerSpeed = 2;
    private float m_CurrentSpeed;
    private int m_PlayerHealth = 5;
    private int m_StartHealth = 1;
    private float m_PlayerAttackSpeed = 1;
    private bool m_CanShoot =  true;
    private bool m_CanDodge = true;
    private bool m_CanDie = true;
    private bool m_CanPlayAttackSound = true;
    private bool m_CheatsOn = false;


    private Rigidbody2D m_Rigidbody;
    private BulletFactory m_BulletFactory;
    private StageManager m_StageManager;
    private UIManager m_UIManager;
    private Slider m_HealthBar;

    public AK.Wwise.Bank m_MusicBank = null;
    public AK.Wwise.Event m_CatEvent_Shooting = null;
    public AK.Wwise.Event m_CatEvent_Dash = null;
    public AK.Wwise.Event m_CatEvent_Hurt = null;
    public AK.Wwise.Event m_CatEvent_Die = null;
    void Start()
    {
        //Get stats from level
        m_MusicBank.Load();   
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_BulletFactory = FindObjectOfType<BulletFactory>();
        m_StageManager = FindObjectOfType<StageManager>();
        m_UIManager = FindObjectOfType<UIManager>();
        m_CurrentSpeed = m_PlayerSpeed;
        m_HealthBar = GetComponentInChildren<Slider>();
        m_StartHealth = m_PlayerHealth;
        m_CanDie = true;
        Object.DontDestroyOnLoad(gameObject);

        StartCoroutine(nameof(DodgeDelay));
        StartCoroutine(nameof(ImmunityDelay));
    }
    
    void Update()
    {
        if (m_PlayerHealth <= 0)
        {
            if(m_CanDie)
            {
                m_CatEvent_Die.Post(gameObject);
                m_UIManager.GameOver();
                m_CanDie = false;
            }

        }
            
           

        if (m_CurrentSpeed != m_PlayerSpeed && m_CanDodge)
            m_CurrentSpeed = m_PlayerSpeed;
        Movement(m_CurrentSpeed);
        if (Input.GetMouseButton(0) && m_CanShoot)
        {
            m_BulletFactory.ShootBullet(gameObject, m_PlayerDamage);
            //
            if (m_CanPlayAttackSound)
            {
                m_CatEvent_Shooting.Post(gameObject);
                StartCoroutine(nameof(AudioDelay));
            }
            
            StartCoroutine(nameof(BulletDelay));
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && m_CanDodge)
        {
            m_CatEvent_Dash.Post(gameObject);
            StartCoroutine(nameof(DodgeDelay));
            StartCoroutine(nameof(ImmunityDelay));
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1
        {
            m_CheatsOn = true;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (m_CheatsOn)
            {
                RollStats();
            }
            
        }


        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up);

        m_HealthBar.maxValue = m_StartHealth;
        m_HealthBar.value = m_PlayerHealth;

    }
    //Public functions
    public void Movement(float speed)
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        m_Rigidbody.velocity = new Vector3(horizontal * speed, vertical * speed);
    }

    public void TakeDamage(int damage)
    {
        if(m_CanDie)
        {
            m_CatEvent_Hurt.Post(gameObject);
        }

        m_PlayerHealth -= damage;
    }

    public void RollStats()
    {
        Stats playerStats = m_StageManager.GetRollStats(true);
        m_UIManager.SetStats(playerStats);
        m_PlayerSpeed += playerStats.m_Stat_Speed;
        m_PlayerHealth += playerStats.m_Stat_HP;
        m_PlayerAttackSpeed *= 1.0f - playerStats.m_Stat_AttackRate;
        m_PlayerDamage += playerStats.m_Stat_DMG;
        if (m_PlayerHealth > m_StartHealth)
        {
            m_StartHealth = m_PlayerHealth;
        }
            
    }
    public void PhaseOut()
    {
        StartCoroutine(nameof(ImmunityDelay));
    }

    public IEnumerator BulletDelay()
    {
        m_CanShoot = false;
        yield return new WaitForSeconds(m_PlayerAttackSpeed);
        m_CanShoot = true;
    }

    public IEnumerator DodgeDelay()
    {
        m_CanDodge = false;
        yield return new WaitForSeconds(1f);
        m_CanDodge = true;
    }

    public IEnumerator ImmunityDelay()
    {
        gameObject.tag = "Enemy";
        m_CurrentSpeed *= 3;
        yield return new WaitForSeconds(0.3f);
        gameObject.tag = "Player";
        m_CurrentSpeed = m_PlayerSpeed;
    }

    public IEnumerator AudioDelay()
    {
        m_CanPlayAttackSound = false;
        yield return new WaitForSeconds(2.0f);
        m_CanPlayAttackSound = true;
    }

    public Stats GetPlayerStats()
    {
        var stats = new Stats();
        stats.m_Stat_Speed = m_PlayerSpeed;
        stats.m_Stat_DMG = m_PlayerDamage;
        stats.m_Stat_AttackRate = m_PlayerAttackSpeed;
        stats.m_Stat_HP = m_PlayerHealth;
        return stats;

    }
}
