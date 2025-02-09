using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public int level = 0;
    public int maxlevel = 6;
    public int maxEx = 0;       // 레벨업에 따라 체력, 마력, 공격력
    public int curEx = 0;
    Rigidbody rb;
    private List<int> ExNum = new List<int>()
    {
        0,100,200,300,500,800,1200
    };
    // 체력
    public int MaxHp = 100;         // 0,100,200,300,500,800,1200
    public int HP = 100;
    // 보호막 체력
    public int bohoHp = 0;
    public int MaxBohoHp = 200;
    public float bohoTime = 0;
    // ㅁㅏ력
    public int MaxMP = 50;
    public int MP = 50;
    // 기본 이동 속도
    public float basemoveSpeed = 5.0f;
    // 현재 이동 속도
    public float moveSpeed = 5.0f;
    // 회전 속도
    public float rotationSpeed = 90.0f;
    // 공격 가능 거리
    public float attackRange = 10.0f;
    // 공격 딜레이
    public float attackDelay = 1.0f;
    public float attackTimer = 0f;
    // 현재 기본 공격력
    public int attackDamage = 3;
    // 탱크 발사대
    public Transform turret;
    public Transform firePoint;
    public float turretTurSpeed = 50.0f;
    public float turretPitchSpeed = 30.0f;
    // 총알 프리펩
    public GameObject bulletPrefab;
    // 기관포 프리펩
    public GameObject cannonPrefab;
    // 추적탄 프리펩
    public GameObject tracerPrefab;
    // 적탐지 거리
    public float detectionRange = 10.0f;
    // 적 레이어
    public LayerMask enemyLayer;
    // 회전 각도 상에 들어왔을때 인식 범위(오차)
    public float minRot = 5.0f;
    // 타겟 인식 했는지 확인
    public bool isTarget = false;
    // 타겟
    [SerializeField]
    private Transform targetEnemy = null;
    //각 스킬별 사용 여부
    private List<bool> isSkill = new List<bool>()
    {
        false, false, false, false
    };
    //각 스킬 이름
    private enum SkillName
    {
        cannon = 0,         // 가까운 범위의 적을 연속으로 공격
        tracer = 1,
        fast = 2,
        shield = 3,
    }
    // 스킬 딜레이
    List<float> skillDelay = new List<float>()
    {
        0.2f,10.0f,5.0f,3.0f
    };
    // 스킬 마력 소비량
    List<int> skillConsume = new List<int>()
    {
        1,10,5,20
    };
    // 스킬 레벨
    public List<int> skillLevel = new List<int>()
    {
        0,0,0,0
    };
    public MessageBox messageBox;
    bool isStop = false;
    public MainControl mainControl;

    void Start()
    {
        mainControl = GameObject.Find("MainControl").GetComponent<MainControl>();
        rb = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        TurnMove();
        Attack();
        if(bohoTime > 0)
        {
            bohoTime -= Time.deltaTime;
        }
    }

    void TurnMove()
    {
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float turn = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        rb.MovePosition(rb.position + (this.transform.forward.normalized * move));
        this.transform.Rotate(0, turn, 0);
        // 목표 찾기
        FindTarget();
        if(targetEnemy != null)
        {
            RotateTurret();
        }
    }

    void FindTarget()
    {
        Collider[] hitcolliders =
            Physics.OverlapSphere(transform.position,
            detectionRange, enemyLayer);
        if(hitcolliders.Length > 0)
        {
            float saveDistance = Mathf.Infinity;
            foreach(var hit in hitcolliders)
            {
                float distnace = Vector3.Distance(
                    this.transform.position, hit.transform.position);
                if(distnace < saveDistance)
                {
                    saveDistance = distnace;
                    targetEnemy = hit.transform;
                }
            }
        }
        else
        {
            targetEnemy = null;
        }
    }
    void RotateTurret()
    {
        Vector3 dir = targetEnemy.position - turret.position;
        dir.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(dir);

        turret.rotation = Quaternion.Lerp(turret.rotation, targetRot, Time.deltaTime * rotationSpeed);

        if(Vector3.Angle(turret.forward, dir) < minRot)
        {
            isTarget = true;
            if(attackTimer >= attackDelay)
            {
                attackTimer = 0;
                Shoot();
            }
        }
        else
        {
            isTarget = false;
        }
    }

    void Attack()
    {
        if (isTarget)
        {
            attackTimer += Time.deltaTime;
        }
    }

    void Shoot()
    {
        Vector3 dir = new Vector3(targetEnemy.position.x, 0, targetEnemy.position.z) - new Vector3(firePoint.position.x, 0, firePoint.position.z);
        Quaternion targetRot = Quaternion.LookRotation(dir);
        Bullet bullet = Instantiate(bulletPrefab, firePoint.position, targetRot).GetComponent<Bullet>();
        bullet.damage = attackDamage;
    }

    public void UseSkill(int skillName)
    {
        if (skillLevel[skillName] > 0)
        {
            if (!isSkill[skillName])
            {
                if(MP >= skillConsume[skillName])
                {
                    isSkill[skillName] = true;
                    switch (skillName)
                    {
                        case (int)SkillName.cannon:
                            StartCoroutine("Cannon");
                            break;
                        case (int)SkillName.tracer:
                            StartCoroutine("tracer");
                            break;
                        case (int)SkillName.fast:
                            StartCoroutine("fast");
                            break;
                        case (int)SkillName.shield:
                            StartCoroutine("shield");
                            break;
                    }
                }
                else
                {
                    messageBox.ShowMessageBox("마나가 부족합니다.");
                }
            }
            else
            {
                messageBox.ShowMessageBox("스킬을 아직 사용할 수 없습니다.");
            }
        }
        else
        {
            messageBox.ShowMessageBox("스킬을 아직 획득하지 않았습니다.");
        }
    }

    IEnumerator Cannon()
    {
        int skillNum = (int)SkillName.cannon;

        //발사
        if (!targetEnemy)
        {
            messageBox.ShowMessageBox("공격할 수 있는 적이 없습니다.");
        }
        else
        {
            for(int i = 0; i < 3; i++)
            {
                // 반복적으로 총알 발사
                yield return new WaitForSeconds(0.2f);
                // 발사
                Vector3 dir = new Vector3(targetEnemy.position.x, 0, targetEnemy.position.z);
                Quaternion targetRot = Quaternion.LookRotation(dir);
                Bullet bullet = Instantiate(cannonPrefab, firePoint.position, targetRot).GetComponent<Bullet>();
                bullet.damage = attackDamage / 3;
                MP = MP - skillConsume[skillNum];
                mainControl.MPBar.value = (float)MP / MaxMP;
            }
        }
        // 스킬이 다시 사용될 수 있는 시간
        yield return new WaitForSeconds(skillDelay[skillNum]);
        isSkill[skillNum] = false;
    }

    IEnumerator Fast()
    {
        int skillNum = (int)SkillName.fast;
        // 속도 증가
        moveSpeed += 1.3f;
        MP = MP - skillConsume[skillNum];
        mainControl.MPBar.value = (float)MP / MaxMP;
        // 스킬이 다시 사용될 수 있는 시간
        yield return new WaitForSeconds(skillDelay[skillNum]);
        // 속도 원래대로 변경
        moveSpeed = basemoveSpeed;
        isSkill[skillNum] = false;

    }
    IEnumerator Shield()
    {
        int skillNum = (int)SkillName.shield;
        // 보호막 생성(보호막체력 더하기)
        bohoHp = MaxHp;
        bohoTime = 5.0f;
        MP = MP - skillConsume[skillNum];
        mainControl.MPBar.value = (float)MP / MaxMP;
        // 스킬이 다시 사용될 수 있는 시간
        yield return new WaitForSeconds(skillDelay[skillNum]);
        // 원래대로 변경
        isSkill[skillNum] = false;
        yield return new WaitUntil(() => bohoTime <= 0);
        // 보호막 제거(보호막 체력 빼기 단 0보다 적다면 0)
        bohoHp = 0;
    }
    IEnumerator Tracer()
    {
        int skillNum = (int)SkillName.tracer;

        //발사
        if (!targetEnemy)
        {
            messageBox.ShowMessageBox("공격할 수 있는 적이 없습니다");
        }
        else
        {
            #region
            Bullet bullet = Instantiate(tracerPrefab, firePoint.position,
            transform.rotation).GetComponent<Bullet>();
            bullet.damage = attackDamage;
            bullet.SetEnemy(targetEnemy);
            MP = MP - skillConsume[skillNum];
            mainControl.MPBar.value = (float)MP / MaxMP;
            #endregion
        }
        //스킬이 다시 사용딜 수 있는 시간
        yield return new WaitForSeconds(skillDelay[skillNum]);
        isSkill[skillNum] = false;
    }
    public void SetHp(int damage)
    {
        if (!isStop)
        {
            if(bohoHp > 0)
            {
                bohoHp -= damage;
            }
            else
            {
                HP -= damage;
                mainControl.HPBar.value = (float) HP / MaxHp;

                if(HP <= 0)
                {
                    mainControl.HPBar.value = 0;
                    HP = 0;
                    Time.timeScale = 0;
                    mainControl.OpenMenuUI();
                }
            }
        }
    }
    public void SetEx(int ex)
    {
        curEx += ex;
        mainControl.EXBar.value = (float)curEx / maxEx;
        if(curEx > maxEx)
        {
            if(level < ExNum.Count)
            {
                level++;
                curEx = 0;
                maxEx = ExNum[level];
                Time.timeScale = 0;
                MaxHp = (int)(MaxHp + (MaxHp * 0.1f));
                HP = MaxHp;
                MaxHp = (int)(MaxHp + (MaxMP * 0.1f));
                MP = MaxMP;
                attackDamage = (int)(attackDamage + (attackDamage * 0.1f));

                mainControl.LvText.text = "Lv." + level;
                mainControl.StopLvText.text = "Lv." + level;
                mainControl.HPBar.value = 1;
                mainControl.MPBar.value = 1;
                mainControl.OpenStopUI();
            }
        }
    }
}
