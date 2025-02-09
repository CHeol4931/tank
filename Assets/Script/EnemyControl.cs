using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyControl;

public class EnemyControl : MonoBehaviour
{
    public enum EnemyName
    {
        Bomb,
        Shoot,
        fighter,
        Boss1,
        Boss2,
        Boss3
    }

    public enum EnemyScore
    {
        Bomb = 10,
        Shoot = 10,
        Fighter = 20,
        Boss1= 500,
        Boss2 = 1000,
        Bosse = 2000

    }
    public enum EnemyEx
    {
        Bomb = 10,
        Shoot = 20,
        Fighter = 30,
        Boss1 = 500,
        Boss2 = 500,
        Bosse = 900
    }
    public enum StageEnmeyNum
    {
        stage01 = 50,
        stage02 = 100,
        stage03 = 150
    }
    public MainControl mainControl;
    public GameObject enemyPrefab;
    public int addEnemyNum = 0;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void RepeatRespawn()
    {
        InvokeRepeating("Respawn", 3.0f, 3.0f);
    }
    void Respawn()
    {
        //x축 기준 -80~80, 50,-80
        switch (mainControl.stageNum)
        {
            case 0:     //맴 상단 기준 적 생성
                if(addEnemyNum < (int)StageEnmeyNum.stage01){
                    Instantiate(enemyPrefab, new Vector3(Random.Range(-80, 80), 0, 50), enemyPrefab.transform.rotation);
                    addEnemyNum++;
                }
                else
                {
                    CancelInvoke("Respawn");
                }
                break;
            case 1:     // 스테이지2 // 맴 상단 좌/우 기준 생성
                if(addEnemyNum < (int)StageEnmeyNum.stage02)
                {
                    float tempX = Random.Range(-80, 80);
                    int enemyNum = Random.Range(0, 2);
                    if(tempX > -50 && tempX < 50)
                    {
                        Enemy enemy = Instantiate(enemyPrefab, new Vector3(tempX, 0, 50), enemyPrefab.transform.rotation).GetComponent<Enemy>();
                        enemy.SetEnemy((EnemyName)enemyNum);
                    }
                    else
                    {
                        Enemy enemy = Instantiate(enemyPrefab, new Vector3(tempX, 0, Random.Range(-80, 80)), enemyPrefab.transform.rotation).GetComponent<Enemy>();
                        enemy.SetEnemy((EnemyName)enemyNum);
                    }
                    addEnemyNum++;
                }
                else
                {
                    CancelInvoke("Respawn");

                }
                break;
            case 2:     // 스테이지3 // 맴 상/하  좌/우
                if (addEnemyNum < (int)StageEnmeyNum.stage03)
                {
                    float tempX = Random.Range(-80, 80);
                    int enemyNum = Random.Range(0, 3);
                    if (tempX > -50 && tempX < 50)
                    {
                        int minus = 1;
                        if(Random.Range(0, 2) == 1)
                        {
                            minus = -1;
                        }
                        Enemy enemy = Instantiate(enemyPrefab, new Vector3(tempX, 0, 50), enemyPrefab.transform.rotation).GetComponent<Enemy>();
                        enemy.SetEnemy((EnemyName)enemyNum);
                    }
                    else
                    {
                        Enemy enemy = Instantiate(enemyPrefab, new Vector3(tempX, 0, Random.Range(-80, 80)), enemyPrefab.transform.rotation).GetComponent<Enemy>();
                        enemy.SetEnemy((EnemyName)enemyNum);
                    }
                    addEnemyNum++;
                }
                else
                {
                    CancelInvoke("Respawn");
                }
                break;
        }
    }
}
