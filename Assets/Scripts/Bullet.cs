using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Members
    public float m_BulletSpeed = 10;
    public int m_BulletDamage = 1;
    public Vector3 m_MousePos;
    public GameObject m_Owner;

    private Rigidbody2D m_BulletBody;
    private Vector3 m_Direction;
    private Vector3 m_OwnerPos;
    private string m_OwnerTag;
    void Start()
    {
        m_BulletBody = GetComponent<Rigidbody2D>();
        m_Direction = m_MousePos - m_Owner.transform.position;
        m_OwnerTag = m_Owner.tag;
        m_OwnerPos = m_Owner.transform.right;
    }

    void Update()
    {
        if (m_OwnerTag == "Player")
            m_BulletBody.velocity = new Vector3(m_Direction.x, m_Direction.y, 0).normalized * m_BulletSpeed;
        else
            m_BulletBody.velocity = new Vector3(m_OwnerPos.x, m_OwnerPos.y, 0) * m_BulletSpeed;
    }
    //Private functions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(m_OwnerTag) || collision.gameObject.CompareTag("Bullet")) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(m_BulletDamage);
            Destroy(gameObject);
            return;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponent<Enemy>())
                collision.gameObject.GetComponent<Enemy>().TakeDamage(m_BulletDamage);
            if (collision.gameObject.GetComponent<EnemyTest01>())
                collision.gameObject.GetComponent<EnemyTest01>().TakeDamage(m_BulletDamage);
            Destroy(gameObject);
            return;
        }
        Destroy(gameObject);
    }
}
