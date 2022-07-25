using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulletFactory : MonoBehaviour
{
    //Members
    public Bullet Bullet;
    private Camera m_MainCam;
    
    private void Start()
    {
        m_MainCam = Camera.main;
        SceneManager.sceneLoaded += NewSceneLoaded;
    }
    //Public functions
    public void ShootBullet(GameObject shooter, int damage, float bulletspeed = 10f)
    {
        var shotBullet = Instantiate(Bullet, shooter.transform.position, Quaternion.identity);
        shotBullet.m_Owner = shooter;
        shotBullet.m_BulletDamage = damage;
        shotBullet.m_BulletSpeed = bulletspeed;
        shotBullet.m_MousePos = m_MainCam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void NewSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        m_MainCam = Camera.main;
    }
}

