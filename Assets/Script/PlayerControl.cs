using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public int level = 0;
    public int maxlevel = 6;
    public int maxEx = 0;       // �������� ���� ü��, ����, ���ݷ�
    public int curEx = 0;
    Rigidbody rb;
    private List<int> ExNum = new List<int>()
    {
        0,100,200,300,500,800,1200
    };
    // ü��
    public int MaxHp = 100;         // 0,100,200,300,500,800,1200
    public int HP = 100;
    // ��ȣ�� ü��
    public int bohoHp = 0;
    public int MaxBohoHp = 200;
    public float bohoTime = 0;
    // ������
    public int MaxMP = 50;
    public int MP = 50;
    // �⺻ �̵� �ӵ�
    public float basemoveSpeed = 5.0f;
    // ���� �̵� �ӵ�
    public float moveSpeed = 5.0f;
    // ȸ�� �ӵ�
    public float rotationSpeed = 90.0f;
    // ���� ���� �Ÿ�
    public float attackRange = 10.0f;
    // ���� ������
    public float attackDelay = 1.0f;
    public float attackTimer = 0f;
    // ���� �⺻ ���ݷ�
    public int attackDamage = 3;
    // ��ũ �߻��
    public Transform turret;
    public Transform firePoint;
    public float turretTurSpeed = 50.0f;
    public float turretPitchSpeed = 30.0f;
    // �Ѿ� ������
    public GameObject bulletPrefab;
    // ����� ������
    public GameObject cannonPrefab;
    // ����ź ������
    public GameObject tracerPrefab;
    // ��Ž�� �Ÿ�
    public float detectionRange = 10.0f;
    // �� ���̾�
    public LayerMask enemyLayer;
    // ȸ�� ���� �� �������� �ν� ����(����)
    public float minRot = 5.0f;
    // Ÿ�� �ν� �ߴ��� Ȯ��
    public bool isTarget = false;
    // Ÿ��
    [SerializeField]
    private Transform targetEnemy = null;
    //�� ��ų�� ��� ����
    private List<bool> isSkill = new List<bool>()
    {
        false, false, false, false
    };
    //�� ��ų �̸�
    private enum SkillName
    {
        cannon = 0,         // ����� ������ ���� �������� ����
        tracer = 1,
        fast = 2,
        shield = 3,
    }
    // ��ų ������
    List<float> skillDelay = new List<float>()
    {
        0.2f,10.0f,5.0f,3.0f
    };
    // ��ų ���� �Һ�
    List<int> skillConsume = new List<int>()
    {
        1,10,5,20
    };
    // ��ų ����
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
        // ��ǥ ã��
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
                    messageBox.ShowMessageBox("������ �����մϴ�.");
                }
            }
            else
            {
                messageBox.ShowMessageBox("��ų�� ���� ����� �� �����ϴ�.");
            }
        }
        else
        {
            messageBox.ShowMessageBox("��ų�� ���� ȹ������ �ʾҽ��ϴ�.");
        }
    }

    IEnumerator Cannon()
    {
        int skillNum = (int)SkillName.cannon;

        //�߻�
        if (!targetEnemy)
        {
            messageBox.ShowMessageBox("������ �� �ִ� ���� �����ϴ�.");
        }
        else
        {
            for(int i = 0; i < 3; i++)
            {
                // �ݺ������� �Ѿ� �߻�
                yield return new WaitForSeconds(0.2f);
                // �߻�
                Vector3 dir = new Vector3(targetEnemy.position.x, 0, targetEnemy.position.z);
                Quaternion targetRot = Quaternion.LookRotation(dir);
                Bullet bullet = Instantiate(cannonPrefab, firePoint.position, targetRot).GetComponent<Bullet>();
                bullet.damage = attackDamage / 3;
                MP = MP - skillConsume[skillNum];
                mainControl.MPBar.value = (float)MP / MaxMP;
            }
        }
        // ��ų�� �ٽ� ���� �� �ִ� �ð�
        yield return new WaitForSeconds(skillDelay[skillNum]);
        isSkill[skillNum] = false;
    }

    IEnumerator Fast()
    {
        int skillNum = (int)SkillName.fast;
        // �ӵ� ����
        moveSpeed += 1.3f;
        MP = MP - skillConsume[skillNum];
        mainControl.MPBar.value = (float)MP / MaxMP;
        // ��ų�� �ٽ� ���� �� �ִ� �ð�
        yield return new WaitForSeconds(skillDelay[skillNum]);
        // �ӵ� ������� ����
        moveSpeed = basemoveSpeed;
        isSkill[skillNum] = false;

    }
    IEnumerator Shield()
    {
        int skillNum = (int)SkillName.shield;
        // ��ȣ�� ����(��ȣ��ü�� ���ϱ�)
        bohoHp = MaxHp;
        bohoTime = 5.0f;
        MP = MP - skillConsume[skillNum];
        mainControl.MPBar.value = (float)MP / MaxMP;
        // ��ų�� �ٽ� ���� �� �ִ� �ð�
        yield return new WaitForSeconds(skillDelay[skillNum]);
        // ������� ����
        isSkill[skillNum] = false;
        yield return new WaitUntil(() => bohoTime <= 0);
        // ��ȣ�� ����(��ȣ�� ü�� ���� �� 0���� ���ٸ� 0)
        bohoHp = 0;
    }
    IEnumerator Tracer()
    {
        int skillNum = (int)SkillName.tracer;

        //�߻�
        if (!targetEnemy)
        {
            messageBox.ShowMessageBox("������ �� �ִ� ���� �����ϴ�");
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
        //��ų�� �ٽ� ���� �� �ִ� �ð�
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
