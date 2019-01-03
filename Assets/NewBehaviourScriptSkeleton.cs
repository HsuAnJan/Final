using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScriptSkeleton : MonoBehaviour
{
    private AnimatorStateInfo animStateInfo;
    private const string AttackState = "skeletonAttack";
    private const string HitState = "skeletonHit";

    private int a = 1;
    private int b = 0;
    private float speed = 0.1f;
    public int HP = 10;
    public GameObject playerUnit;
    private Animator animator;
    public float defendRadius;
    private enum MonsterState
    {
        STAND,
        WALK,
        CHASE,
        WARN,
        RETURN
    }
    private MonsterState currentState = MonsterState.STAND;
    public float actRestTme;
    private float lastActTime;

    private float diatanceToPlayer;
    public float[] actionWeight = { 3000, 4000 };


    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        RandomAction();
    }
    void RandomAction()
    {
        lastActTime = Time.time;
        float number = Random.Range(0, actionWeight[0] + actionWeight[1]);
        if (number <= actionWeight[0])
        {
            currentState = MonsterState.STAND;

        }
        if (actionWeight[0] < number && number <= actionWeight[0] + actionWeight[1])
        {
            currentState = MonsterState.WALK;
        }
    }
    void Update()
    {
        animator.SetBool("Attack", false);
        if (animStateInfo.IsName(HitState))
        {
            animator.SetBool("Hurt", false);
        }
        animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        switch (currentState)
        {
            case MonsterState.STAND:
                animator.SetTrigger("Stand");
                if (Time.time - lastActTime > actRestTme)
                {
                    RandomAction();
                }
                EnemyDistanceCheck();
                break;
            case MonsterState.WALK:
                if (!animStateInfo.IsName(AttackState))   //如果再攻擊就不要移動
                {
                    if (transform.position.x >= -27 && transform.position.x <= -25)
                        a = 1;
                    if (transform.position.x >= -47 && transform.position.x <= -45)
                        a = 0;
                    if (transform.position.x >= -7 && transform.position.x <= -5)
                        a = 2;
                    if (a == 1)
                    {
                        animator.SetTrigger("Walk");
                        if (b >= 0 && b < 1)
                        {
                            transform.Translate(-speed, 0, 0);
                            transform.localScale = new Vector3(-1, 1, 1);
                        }
                        else
                        {
                            transform.Translate(speed, 0, 0);
                            transform.localScale = new Vector3(1, 1, 1);
                        }
                    }
                    if (a == 0)
                    {
                        animator.SetTrigger("Walk");
                        transform.Translate(speed, 0, 0);
                        transform.localScale = new Vector3(1, 1, 1);
                        b = Random.Range(0, 2);
                    }
                    if (a == 2)
                    {
                        animator.SetTrigger("Walk");
                        transform.Translate(-speed, 0, 0);
                        transform.localScale = new Vector3(-1, 1, 1);
                        b = Random.Range(0, 2);
                    }
                }
                if (Time.time - lastActTime > actRestTme)
                {
                    RandomAction();
                }
                EnemyDistanceCheck();
                break;
            case MonsterState.CHASE:
                if (diatanceToPlayer <=1.5f)
                {
                    animator.SetBool("Attack",true);
                    animator.SetTrigger("Stand");
                    EnemyDistanceCheck();
                }
                else
                {
                    if (!animStateInfo.IsName(AttackState))   //如果再攻擊就不要移動
                    {
                        animator.SetTrigger("Walk");
                        if (playerUnit.transform.position.x < transform.position.x)
                        {
                            transform.Translate(-speed, 0, 0);
                            transform.localScale = new Vector3(-1, 1, 1);
                        }
                        if (playerUnit.transform.position.x > transform.position.x)
                        {
                            transform.Translate(speed, 0, 0);
                            transform.localScale = new Vector3(1, 1, 1);
                        }
                    }
                    EnemyDistanceCheck();
                    if (transform.position.x < -46 || transform.position.x > -6)
                    {
                        currentState = MonsterState.RETURN;
                    }
                }
                break;
            case MonsterState.RETURN:
                diatanceToPlayer = Vector2.Distance(playerUnit.transform.position, transform.position);
                if (!animStateInfo.IsName(AttackState))   //如果再攻擊就不要移動
                {
                    animator.SetTrigger("Walk");
                    if (transform.position.x > -26)
                    {                      
                        transform.Translate(-speed, 0, 0);
                        transform.localScale = new Vector3(-1, 1, 1);
                    }
                    if (transform.position.x < -26)
                    {
                        transform.Translate(speed, 0, 0);
                        transform.localScale = new Vector3(1, 1, 1);
                    }
                }
                if (transform.position.x <= -25 && transform.position.x >= -27)
                {
                    RandomAction();
                }
                if ((playerUnit.transform.position.x >= -46&&playerUnit.transform.position.x<=-6)&& diatanceToPlayer<defendRadius)
                {
                    currentState = MonsterState.CHASE;
                }
                break;
        }
    }
    void EnemyDistanceCheck()
    {

        diatanceToPlayer = Vector2.Distance(playerUnit.transform.position, transform.position);
        if (diatanceToPlayer < defendRadius)
        {
            currentState = MonsterState.CHASE;
        }
    }
    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.tag == "HeroAttack")
        {          
            animator.SetBool("Hurt",true);
            HP -= 3; //生命值-3
            if (HP <= 0)
            {
                animator.SetTrigger("Dead");
            }
        }
    }
    void Dead()
    {
        Destroy(gameObject);
    }
}
