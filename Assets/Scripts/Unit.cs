using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour
{
    Animator animator;
    bool isMove;
    public float curHp;
    public float curMp;
    public float findDis;
    public bool isDie;
    public bool isAttack;
    public GameObject target;
    public Status status_original;
    private Status status;
    public GameObject Leader;
    public UnitType unitType;
    public UnitInfo unitInfo;
    public PlayerUnitType playerUnitType;
    public bool isLeader;
    public float speed;
    NavMeshAgent nav;
    public CharacterController cr;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        status = ScriptableObject.Instantiate(status_original);
    }
    // Start is called before the first frame update
    void Start()
    {
        curHp = status.MaxHp;
        curMp = status.MaxMp;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Unit unit in GameManager.Instance.unitsList)
        {
            if (this.gameObject.GetComponent<CharacterController>() != null && unitType == UnitType.Player)
            {
                Leader = null;
            }
            else if (unitType == UnitType.Player)
            {
                Leader = GameManager.Instance.leader;
            }
        }

        Move();
        RotateLeader();
        FollowLeader();
        FindUnit();


        if (target != null && !isLeader)
        {
            if (target != null && Vector3.Distance(target.transform.position, this.transform.position) <= status.attackDistance)
            {
                nav.isStopped = true;
                unitInfo = UnitInfo.attack;
                if (isAttack)
                {
                    StartCoroutine(Attack());
                }
            }
            else
            {
                nav.isStopped = false;
                unitInfo = UnitInfo.chase;
                nav.SetDestination(target.transform.position);
            }
        }
        else if (!isLeader)
        {
            unitInfo = UnitInfo.follow;
        }
        else
        {
            unitInfo = UnitInfo.none;
        }
    }

    IEnumerator Attack() {
        if (target == null)
        {
            yield return null;
        }
        if (isAttack)
        {
            isAttack = false;
            target.GetComponent<Unit>().SetHp(status.attack);
            animator.SetTrigger("Attack");
           GetExp(status.attack);
            yield return new WaitForSeconds(1);
            isAttack = true;
        }
    }
    

    void GetExp(float damage)
    {
        if (target.GetComponent<Unit>().curHp - status.attack > 0)
        {
            status.curExp += target.GetComponent<Unit>().status.curExp * damage/ target.GetComponent<Unit>().status.MaxHp;
        }
        else
        {
            status.curExp = target.GetComponent<Unit>().status.curExp * target.GetComponent<Unit>().curHp / target.GetComponent<Unit>().status.MaxHp;
        }

        if(status.curExp >= status.maxExp)
        {
            LevelUp();
        }
    }


    void SetHp(float damage)
    {
        curHp -= damage;
        if (curHp <= 0)
        {
            curHp = 0;
            isDie = true;
            if (unitType == UnitType.Enemy)
            {
                foreach (Unit type in GameManager.Instance.unitsList)
                {
                    type.status.curExp += target.GetComponent<Unit>().status.curExp / 10;
                }
            }
        }

    }

    void LevelUp()
    {
        if(unitType== UnitType.Player)
        {
            if(status.curExp >= status.maxExp)
            {
                status.curExp -= status.maxExp;
                status.level += 1;
                status.maxExp += 500;
                switch(playerUnitType)
                {
                    case PlayerUnitType.warrior:
                        status.MaxHp += 20;
                        status.attack += 10;
                        break;
                    case PlayerUnitType.archor:
                        status.MaxHp += 15;
                        status.attack += 8;
                        break;
                    case PlayerUnitType.mage:
                        status.MaxHp += 10;
                        status.attack += 10;
                        break;
                }

            }
        }
    }
    void FollowLeader()
    {
        if(unitInfo == UnitInfo.follow && !isLeader && unitType == UnitType.Player)
        {
            if(Vector3.Distance(GameManager.Instance.leader.transform.position,this.transform.position) > 5)
            {
                nav.stoppingDistance = 3;
                nav.SetDestination(GameManager.Instance.leader.transform.position);
            }
            
        }
    }

    void Move()
    {
        if (GameManager.Instance.leader == this.gameObject)
        {
            isMove = false;
            if (Input.GetKey(KeyCode.W))
            {
                cr.Move(transform.forward.normalized * speed * Time.deltaTime);
                isMove = true;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                cr.Move(-transform.forward.normalized * speed * Time.deltaTime);
                isMove = true;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                cr.Move(transform.right.normalized * speed * Time.deltaTime);
                isMove = true;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                cr.Move(-transform.right.normalized * speed * Time.deltaTime);
                isMove = true;
            }
           

            if(isMove)
            {
                animator.SetBool("isMove", true);
            }
            if (!isMove)
            {
                animator.SetBool("isMove", false);
            }
        }
    }

    void FindUnit()
    {
        Collider[] collider = Physics.OverlapSphere(this.gameObject.transform.position, findDis, LayerMask.GetMask("Unit"));
        switch (unitType)
        {
            case UnitType.Player:
                foreach (Collider collider2 in collider)
                {
                    if(collider2.GetComponent<Unit>().unitType == UnitType.Enemy && !collider2.GetComponent<Unit>().isDie)
                    {
                       target = collider2.gameObject;
                        if(Vector3.Distance(this.transform.position, target.transform.position) >= Vector3.Distance(this.transform.position, collider2.transform.position))
                        {
                            target = collider2.gameObject;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                

                break;
            case UnitType.Enemy:
                foreach (Collider collider2 in collider)
                {
                    if (collider2.GetComponent<Unit>().unitType == UnitType.Player && !collider2.GetComponent<Unit>().isDie)
                    {
                        target = collider2.gameObject;
                        if (Vector3.Distance(this.transform.position, target.transform.position) >= Vector3.Distance(this.transform.position, collider2.transform.position))
                        {
                            target = collider2.gameObject;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
               
                break;
        }
       
        
        
    }

    void RotateLeader()
    {
       if(GameManager.Instance.leader == this.gameObject)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Plane plane = new Plane(Vector3.up, Vector3.zero);
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                Vector3 mouseDis = ray.GetPoint(distance);

                transform.LookAt(new Vector3(mouseDis.x, transform.position.y, mouseDis.z));
            }
        }
    }
}
