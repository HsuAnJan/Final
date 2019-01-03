using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewBehaviourScript : MonoBehaviour
{
    private AnimatorStateInfo animStateInfo;
    private const string RunState = "run";
    private const string IdleState = "idle";
    private const string Attack1State = "attack1";
    private const string Attack2State = "attack2";
    private const string Attack3State = "attack3";
    private const string HitState = "hit";
    private int HitCount = 0;

    //血量
    public const int maxHealth = 40;
    public int currentHealth = maxHealth;
    public RectTransform HealthBar, Hurt;

    Rigidbody2D body2D;
    public Animator animator;
    public GameObject sprite;
    public GameObject attack1;
    public GameObject attack2;
    public GameObject attack3;

    [Header("水平速度")]
    public float speedX;

    [Header("水平方向")]
    public float horizontalDirection;//數值會在 -1~1之間

    const string HORIZONTAL = "Horizontal";

    [Header("水平推力")]
    [Range(0, 150)]
    public float Xforce;

    [Header("最大水平速度")]
    public float maxSpeedX;

    public float maxSpeedY;

    float speedY;  //目前垂直速度

    [Header("垂直向上推力")]
    public float Yforce;

    [Header("垂直方向")]
    public float verticalDirection;

    public bool Jumping = false;
    public bool ground = true;
    private int jump = 0;
    private bool dead = false;
    private int redead = 0;
    void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (ground||jump<1))
        {
            body2D.AddForce(Vector2.up * Yforce);
            jump++;
        }
    }

    public void ControlSpeed()
    {
        speedX = body2D.velocity.x;
        speedY = body2D.velocity.y;
        float newSpeedX = Mathf.Clamp(speedX, -maxSpeedX, maxSpeedX);
        float newSpeedY = Mathf.Clamp(speedY, -maxSpeedY, maxSpeedY);
        body2D.velocity = new Vector2(newSpeedX, newSpeedY);
    }


    void Start()
    {
        body2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Movement()
    {

        speedY = body2D.velocity.y;
        horizontalDirection = Input.GetAxis("Horizontal");
        verticalDirection = Input.GetAxis("Vertical");
        if (Input.GetAxis("Horizontal") > 0)
        {
            sprite.transform.localScale = new Vector3(10, 10, 1);
            attack1.transform.localScale = new Vector3(1, 1, 1);
            attack2.transform.localScale = new Vector3(1, 1, 1);
            attack3.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            sprite.transform.localScale = new Vector3(-10, 10, 1);
            attack1.transform.localScale = new Vector3(-1, 1, 1);
            attack2.transform.localScale = new Vector3(-1, 1, 1);
            attack3.transform.localScale = new Vector3(-1, 1, 1);
        }
        if (ground == true)
            body2D.velocity = new Vector2(Xforce * horizontalDirection, speedY);
    }

    void Statechange()
    {
        animator.SetFloat("speed", Mathf.Abs(body2D.velocity.x));
        animator.SetBool("ground", ground);
        animator.SetBool("jumping", Jumping);
    }
    void JumpState()
    {
        if (jump == 1)
        {
            animator.SetTrigger("DJ");
            jump++;
        }
        if (body2D.velocity.y > 0.1)
        {
            Jumping = true;
            ground = false;
        }
        else if (Jumping)
        {
            Jumping = false;
        }
        if (body2D.velocity.y != 0)
        {
            ground = false;
        }
        else
        {
            ground = true;
            jump = 0;
        }
    }

    void attacktest()
    {
        if ((animStateInfo.IsName(IdleState)||animStateInfo.IsName(RunState)) && HitCount == 0 && animStateInfo.normalizedTime > 0.1f)
        {
            animator.SetInteger("ActionID", 1);
            HitCount = 1;
        }
        if (animStateInfo.IsName(Attack1State) && HitCount == 1 && animStateInfo.normalizedTime > 0f)
        {
            animator.SetInteger("ActionID", 2);
            HitCount = 2;
        }
        if (animStateInfo.IsName(Attack2State) && HitCount == 2 && animStateInfo.normalizedTime > 0f)
        {
            animator.SetInteger("ActionID", 3);
            HitCount = 3;
        }
    }

        void Update()
    {
        if (animStateInfo.IsName(HitState))
            {
                animator.SetBool("Hit", false);
            }
        animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            //血量控制
        HealthBar.sizeDelta = new Vector2(currentHealth, HealthBar.sizeDelta.y);
            //呈現傷害量
        if (Hurt.sizeDelta.x > HealthBar.sizeDelta.x)
            {
                //讓傷害量(紅色血條)逐漸追上當前血量
                Hurt.sizeDelta += new Vector2(-1, 0) * Time.deltaTime * 10;
            }
        if ((animStateInfo.IsName(IdleState) && animStateInfo.normalizedTime > 0f) || (animStateInfo.IsName(RunState) && animStateInfo.normalizedTime > 0f))
            {
                animator.SetInteger("ActionID", 0);
                HitCount = 0;
            }
        if (!animStateInfo.IsName(Attack1State) && !animStateInfo.IsName(Attack2State) && !animStateInfo.IsName(Attack3State)&&!dead)
            {
                Movement();
            }
        ControlSpeed();
        if (!dead)
        {
            TryJump();
        }
        Statechange();
        JumpState();
        if (Input.GetKeyDown(KeyCode.X))
            {
                attacktest();
            } 

    }
    void OnTriggerEnter2D(Collider2D obj)
    {
        if (!dead)
        {
            if (obj.tag == "Boss")
            {
                animator.SetBool("Hit", true);
                currentHealth = currentHealth - 10; //生命值-4
            }
            if (obj.tag == "Skeleton")
            {
                animator.SetBool("Hit", true);
                currentHealth = currentHealth - 2; //生命值-2        
            }
            if (obj.tag == "Verit")
            {
                animator.SetBool("Hit", true);
                currentHealth = currentHealth - 1; ; //生命值-1        
            }
        }
        if (currentHealth <= 0&&redead==0)
        {
            animator.SetTrigger("Dead");
            dead = true;
            redead++;
        }
    }
    void Dead()
    {
        Destroy(gameObject);
    }
}
