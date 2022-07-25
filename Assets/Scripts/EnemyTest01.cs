using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTest01 : MonoBehaviour // 1.1
{
    public Transform m_RayCastSource;
    public Transform r_PlayerTransform;

    public Stats m_EnemyStats;

    public int m_NumberOfRaycasts = 0; 
    public float AngleRange;
    public float m_MoveSpeed;
    public float m_TurnSpeed;
    public bool m_IsBoss;
    public int m_StartHealth;

    public Sprite[] m_Sprites;

    private float m_DistanceToRayCastHit;
    private Vector2 m_RayDirection;
    private Vector2 m_EnemyDirection;
    private Vector2 m_VectorToPlayer;
    private Vector2 m_TargetVector;
    private Vector2 m_RayCastOrigin;
    private float AngleDiff;
    private float[] LineTraceResults;
    private Vector2[] LineTraceDirections;

    private bool m_CanShoot = true;

    private BulletFactory r_EnemyBulletFactory;
    private EnemyFactory r_EnemyFactory;
    private StageManager r_StageManager;
    private TextMeshProUGUI m_StatsGainedText;
    private SpriteRenderer m_FroschImage;
    private Slider m_HealthBar;


    public void TakeDamage(int damage)
    {
        m_EnemyStats.m_Stat_HP -= damage;
    }
    private void GenerateLineTraceResults()
    {
        for (int i = 0; i < m_NumberOfRaycasts; i++)
        {
            AngleDiff = Random.Range(0, AngleRange*2) - AngleRange;
            m_RayDirection = Quaternion.Euler(0f, 0f, AngleDiff) * m_EnemyDirection;
            Debug.DrawRay(m_RayCastOrigin, m_RayDirection, Color.red, 0.05f);
            RaycastHit2D raycasthit = Physics2D.Raycast(m_RayCastOrigin, m_RayDirection);
            if (raycasthit.collider != null)
            {
                m_DistanceToRayCastHit = Vector2.Distance(raycasthit.point, m_RayCastSource.transform.position);
                //Debug.Log("Distance: " + m_DistanceToRayCastHit);
                LineTraceResults[i] = m_DistanceToRayCastHit;
                LineTraceDirections[i] = m_RayDirection;
            }
            else
                LineTraceResults[i] = Mathf.Infinity;

        }

    }

    private void CalculateMotionVector()
    {
        int HighestResultIterator = 0;
        float HighestResult = 0.0f;
        for (int i = 0; i < LineTraceResults.Length; i++)
        {
            if (LineTraceResults[i] > HighestResult)
            {
                HighestResult = LineTraceResults[i];
                HighestResultIterator = i;
            }

        }
        m_TargetVector = LineTraceDirections[HighestResultIterator];
        m_TargetVector = Vector2.Lerp(m_EnemyDirection, m_TargetVector, 0.1f);
        m_TargetVector = Vector2.Lerp(m_TargetVector, m_VectorToPlayer, 0.5f);

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90.0f) * m_TargetVector;

        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

        this.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, m_TurnSpeed * Time.deltaTime);
        transform.Translate(Vector3.right * m_MoveSpeed * Time.deltaTime, Space.Self);
    }

    private void IncreaseStats(Stats stats)
    {
        m_EnemyStats.m_Stat_DMG += stats.m_Stat_DMG;
        m_EnemyStats.m_Stat_HP += stats.m_Stat_HP;
        m_EnemyStats.m_Stat_Speed += stats.m_Stat_Speed;
        m_EnemyStats.m_Stat_AttackRate += stats.m_Stat_AttackRate;
    }

    private IEnumerator DelayShots()
    {
        m_CanShoot = false;
        yield return new WaitForSeconds(m_EnemyStats.m_Stat_AttackRate);
        m_CanShoot = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_RayCastSource = transform.GetChild(0);
        LineTraceResults = new float[m_NumberOfRaycasts];
        LineTraceDirections = new Vector2[m_NumberOfRaycasts];
        r_PlayerTransform = FindObjectOfType<PlayerMovement>().GetComponent<Transform>();
        r_StageManager = FindObjectOfType<StageManager>();
        r_EnemyBulletFactory = r_StageManager.GetComponent<BulletFactory>();
        r_EnemyFactory = r_StageManager.GetComponent<EnemyFactory>();
        m_StatsGainedText = GetComponentInChildren<TextMeshProUGUI>();
        IncreaseStats(r_StageManager.GetRollStats(false));
        m_FroschImage = GetComponentInChildren<SpriteRenderer>();
        m_HealthBar = GetComponentInChildren<Slider>();

        if (!m_IsBoss)
            m_EnemyStats = r_StageManager.GetBaseStats(false);
        if (m_StatsGainedText)
            m_StatsGainedText.text = $"{m_EnemyStats.m_Stat_HP + (m_EnemyStats.m_Stat_Speed * 10)+ (m_EnemyStats.m_Stat_AttackRate * 10) + m_EnemyStats.m_Stat_DMG}";
        // LOAD BASE STATS AND ROLL STATS (from Stage Manager)

        if (m_IsBoss && GameObject.FindObjectsOfType<EnemyTest01>().Length == 1)
        {
            m_EnemyStats = r_StageManager.GetBaseStats(false);
            m_EnemyStats.m_Stat_HP += 40;
        }
        m_StartHealth = m_EnemyStats.m_Stat_HP;
    }

    // Update is called once per frame
    void Update()
    {
        m_RayCastOrigin = m_RayCastSource.position;
        m_EnemyDirection = Vector2.up;
        m_EnemyDirection = m_RayCastSource.position - transform.position;
        m_VectorToPlayer = r_PlayerTransform.position - transform.position;

        GenerateLineTraceResults();
        CalculateMotionVector();

        // SHOOT PROJECTILES every t = Attack Rate 
        if(m_CanShoot)
        {
            r_EnemyBulletFactory.ShootBullet(gameObject, m_EnemyStats.m_Stat_DMG, 10.0f);
            StartCoroutine(nameof(DelayShots));
        }

        if (m_StatsGainedText && m_StatsGainedText.alpha >= 0)
            m_StatsGainedText.alpha -= 0.5f * Time.deltaTime;

        if (m_EnemyStats.m_Stat_HP <= 0)
            Destroy(gameObject);

        if (transform.rotation.z <= 0.1f || transform.rotation.z >= 0.9f)
            m_FroschImage.sprite = m_Sprites[0];
        else
            m_FroschImage.sprite = m_Sprites[1];

        if (r_PlayerTransform.position.x > transform.position.x)
        {
            m_FroschImage.flipX = false;
            m_FroschImage.flipY = false;
        }
        else
        {
            m_FroschImage.flipX = true;
            m_FroschImage.flipY = true;
        }

        if (m_IsBoss)
        {
            if (m_EnemyStats.m_Stat_HP <= m_StartHealth * 0.7f && m_EnemyStats.m_Stat_HP > 10)
            {
                r_EnemyFactory.CreateTracker(new Vector3(transform.position.x + Random.Range(-7, 7), transform.position.y + Random.Range(1, 2), 0), true);
                r_EnemyFactory.CreateTracker(new Vector3(transform.position.x + Random.Range(-7, 7), transform.position.y + Random.Range(1, 2), 0), true);
                foreach (EnemyTest01 enemy in GameObject.FindObjectsOfType<EnemyTest01>())
                {
                    if (enemy.m_IsBoss && enemy.m_EnemyStats.m_Stat_HP <= m_EnemyStats.m_Stat_HP)
                    {
                        enemy.m_EnemyStats.m_Stat_HP = m_EnemyStats.m_Stat_HP;
                    }

                }
                Destroy(gameObject);
            }

            if (m_EnemyStats.m_Stat_HP >= 30)
            {
                transform.localScale = new Vector3(0.1f, 0.1f, 0);
                m_EnemyStats.m_Stat_AttackRate = 3;
            }
            else if (m_EnemyStats.m_Stat_HP >= 10)
            {
                transform.localScale = new Vector3(0.075f, 0.075f, 0);
            }
        }

        m_HealthBar.maxValue = m_StartHealth;
        m_HealthBar.value = m_EnemyStats.m_Stat_HP;
    }       
}
