using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyControl;

public class Enemy : MonoBehaviour
{
    EnemyControl enemyControl;
    public bool isStop = false;
    // 체려ㄱ
    public int MaxHealth = 0;
    public int curHealth = 0;
    // 속도 
    public float speed = 0.1f;
    public LayerMask targetlayer;
    EnemyControl.EnemyName enemyName;
    // 공격 가능 거리
    public float attackRange = 10.0f;
    public float rotateSpeed = 2.0f;
    // 공격간 딜ㄹㅔㅇㅣ 
    public float attackDelay = 1.0f;
    public float attackTimer = 0f;
    // 공격 대미지 
    public int damage = 0;
    // 경험치
    public int ex = 0;
    public int score = 0;
    public PlayerControl playerControl;
    public MainControl mainControl;
    public GameObject bulletPrefab;

    void Start()
    {
        mainControl = GameObject.Find("MainControl").GetComponent<MainControl>();
    }


    void Update()
    {
        if (playerControl == null)
        {
            playerControl = GameObject.Find("Player").GetComponent<PlayerControl>();
            return;
        }
        Move();
        attackTimer += Time.deltaTime;
    }
    private void Move()
    {
        if (Vector3.Distance(this.transform.position, playerControl.transform.position) > attackRange)
        {
            // 플레이어로 이동할 방향 설정
            Vector3 direction = (playerControl.transform.position - this.transform.position).normalized;
            direction.y = 0;    // Y축을 고정(위아래 움직임 제거)
            // 이동
            this.transform.Translate(direction * speed * Time.deltaTime, Space.World);      //월드 기준으로 움직이라는 것 space.world
        }
        else
        {
            AttackEnemy();
        }
    }
    public void AttackEnemy()
    {
        if (attackTimer >= attackDelay)
        {
            attackTimer = 0;
            switch (enemyName)
            {
                case EnemyControl.EnemyName.Bomb:
                    {
                        playerControl.SetHp(damage);
                        Destroy(this.gameObject);
                    }
                    break;
                case EnemyControl.EnemyName.Shoot:
                    {
                        Vector3 dir = new Vector3(playerControl.transform.position.x, 0, playerControl.transform.position.z) - new Vector3(this.transform.position.x, 0, this.transform.position.z);
                        Quaternion targetRot = Quaternion.LookRotation(dir);
                        Bullet bullet = Instantiate(bulletPrefab, this.transform.position, targetRot).GetComponent<Bullet>();
                        bullet.isEnemy = true;
                        bullet.damage = damage;
                    }
                    break;
                case EnemyControl.EnemyName.fighter:
                    {
                        Vector3 dir = new Vector3(playerControl.transform.position.x, 0, playerControl.transform.position.z) - new Vector3(this.transform.position.x, 0, this.transform.position.z);
                        Quaternion targetRot = Quaternion.LookRotation(dir);
                        Bullet bullet = Instantiate(bulletPrefab, this.transform.position, targetRot).GetComponent<Bullet>();
                        bullet.isEnemy = true;
                        bullet.damage = damage;
                        bullet.transform.GetChild(0).gameObject.SetActive(false);
                    }
                    break;
                case EnemyControl.EnemyName.Boss1:
                    {
                        StartCoroutine("Cannon");
                    }
                    break;
                case EnemyControl.EnemyName.Boss2:
                    {
                        StartCoroutine("");
                    }
                    break;
                case EnemyControl.EnemyName.Boss3:
                    {
                        StartCoroutine("");
                    }
                    break;
            }
        }
    }
    IEnumerator Cannon()
    {

        for (int i = 0; i < 3; i++)
        {
            // 반복적으로 총알 발사
            yield return new WaitForSeconds(0.01f);
            // 발사
            Vector3 dir = new Vector3(playerControl.transform.position.x, 0, playerControl.transform.position.z);
            Quaternion targetRot = Quaternion.LookRotation(dir);
            Bullet bullet = Instantiate(bulletPrefab, this.transform.position, targetRot).GetComponent<Bullet>();
            bullet.isEnemy = true;
            bullet.damage = damage / 10;
        }

    }
    public void SetEnemy(EnemyControl.EnemyName en)
    {
        enemyName = en;
        switch (enemyName)
        {
            case EnemyControl.EnemyName.Bomb:
                {
                    MaxHealth = 20;
                    curHealth = MaxHealth; ;
                    speed = 0.2f;
                    attackRange = 2.0f;
                    damage = 20;
                    ex = (int)EnemyControl.EnemyEx.Bomb;
                    score = (int)EnemyControl.EnemyScore.Bomb;
                }
                break;
            case EnemyControl.EnemyName.Shoot:
                {
                    MaxHealth = 40;
                    curHealth = MaxHealth; ;
                    speed = 0.2f;
                    attackRange = 3.0f;
                    damage = 5;
                    ex = (int)EnemyControl.EnemyEx.Shoot;
                    score = (int)EnemyControl.EnemyScore.Shoot;
                }
                break;
            case EnemyControl.EnemyName.fighter:
                {
                    MaxHealth = 200;
                    curHealth = MaxHealth; ;
                    speed = 0.2f;
                    attackRange = 2.0f;
                    damage = 10;
                    ex = (int)EnemyControl.EnemyEx.Fighter;
                    score = (int)EnemyControl.EnemyScore.Fighter;
                }
                break;
        }
    }
    public void SetHp(int damage)
    {
        if (!isStop)
        {
            curHealth -= damage;
            if (curHealth <= 0)
            {
                curHealth = 0;
                playerControl.SetEx(ex);
                mainControl.SetScore(score);
                Destroy(this.gameObject);
            }
        }
    }
}
