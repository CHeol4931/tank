using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10.0f;
    public float endTIme = 2.0f;
    public float lifeTime = 0f;
    public int damage = 1;
    public Transform enemy = null;
    //적의 총알
    public bool isEnemy = false;
    PlayerControl playerControl = null;
    
    void Start()
    {
        playerControl = GameObject.Find("Player").GetComponent<PlayerControl>();
    }
    void Update()
    {
        lifeTime += Time.deltaTime;
        if(enemy == null)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        else
        {
            Vector3 direction = (enemy.position - this.transform.position).normalized;
            direction.y = 0;    //Y축을 고정 (위 아래 움직임 제거)

            this.transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
        if(lifeTime >= endTIme)
        {
            lifeTime = 0f;
            Destroy(this.gameObject);
        }
    }
    public void SetEnemy(Transform targetEnemy)
    {
        enemy = targetEnemy;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isEnemy)
        {
            if (other.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                enemy.SetHp(damage);
                Destroy(this.gameObject);
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                playerControl.SetHp(damage);
                Destroy(this.gameObject);
                  
            }
        }

    }
}
