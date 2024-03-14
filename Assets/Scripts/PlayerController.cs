using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("# Move Speed")]
    public float speed;
    public float runSpeed;
    public float finalSpeed;

    public bool run;

    [Header("# Jump")]
    public float jumpPower;
    public float lastJump;
    public float jumpDelay;

    public bool landing;
    public bool jumping;

    [Header("# Slide")]
    public float slidePower;

    public bool sliding;

    [Header("# Attack")]
    public float lastAttack;
    public float attackInit;

    public bool attacking;
    public bool attackCombo;

    [Header("# Idle Motion")]
    public float waitTime;

    public bool waitingMotion;

    [Header("# Check")]
    public bool wallCheck;
    public bool hit;
    public bool die;
    public bool clear;

    public float clearTime;

    [Header("# ")]
    public float smoothness;

    [Header("# Move Vector")]
    public Vector3 forward;
    public Vector3 right;
    public Vector3 moveDirection;

    Animator anim;
    Camera camera;
    Rigidbody rigid;

    public static PlayerController instance;

    void Awake()
    {
        anim = GetComponent<Animator>();
        camera = Camera.main;
        rigid = GetComponent<Rigidbody>();

        if (PlayerController.instance == null)
            PlayerController.instance = this;
    }

    void Update()
    {
        if (GameManager.instance.clear)
        {
            clearTime += Time.deltaTime;

            if (!clear && clearTime > 2f)
            {
                clear = true;
                StartCoroutine(GameManager.instance.Clear());
                anim.ResetTrigger("Attack0");
                anim.ResetTrigger("Attack1");
                anim.ResetTrigger("Slide");
                anim.ResetTrigger("Hit");
                anim.ResetTrigger("Die");
                anim.SetTrigger("Win");
            }

            return;
        }

        if (hit || die || GameManager.instance.paused)
            return;

        InputMovement();
        InputManager();
        Landing();

        if (sliding)
            Physics.IgnoreLayerCollision(6, 7, true);
        else
            Physics.IgnoreLayerCollision(6, 7, false);
    }

    void LateUpdate()
    {
        if (GameManager.instance.clear)
            return;

        if (PlayerStatus.instance.currentHealth <= 0f && !die)
            Die();

        if (hit || die || GameManager.instance.paused)
            return;

        InputRotation();
        IdleMotion();
        DelayCheck();
    }

    void FixedUpdate()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void InputMovement()
    {
        if (attacking || sliding)
            return;

        finalSpeed = (run) ? runSpeed : speed;

        forward = camera.transform.TransformDirection(Vector3.forward);
        right = camera.transform.TransformDirection(Vector3.right);

        moveDirection = (forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal"));
        moveDirection.y = 0;

        if (!wallCheck)
            rigid.transform.position += moveDirection.normalized * finalSpeed * Time.deltaTime;

        float percent = ((run) ? 1 : 0.5f) * moveDirection.normalized.magnitude;
        anim.SetFloat("Move", percent, 0.1f, Time.deltaTime);
    }

    void InputRotation()
    {
        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            waitTime = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z)), Time.deltaTime * smoothness);
        }
        else
        {
            waitTime += Time.deltaTime;
        }
    }

    void InputManager()
    {
        // Run
        if (Input.GetKey(KeyCode.LeftShift) && PlayerStatus.instance.currentStamina > 0f)
            run = true;
        else
            run = false;

        // Jump Animation
        if (jumping)
            anim.SetBool("Jump", true);
        else
            anim.SetBool("Jump", false);

        // Attack Combo Init
        if (lastAttack > attackInit)
            attackCombo = false;

        if (Input.GetButtonDown("Jump") && lastJump > jumpDelay && landing && !sliding && !attacking)
            Jump();

        if (Input.GetKeyDown(KeyCode.Mouse0) && !attacking && landing && !sliding)
            StartCoroutine(Attack());

        if (Input.GetKeyDown(KeyCode.Mouse1) && !sliding && PlayerStatus.instance.currentStamina > PlayerStatus.instance.slideCost && !jumping && landing)
            StartCoroutine(Slide());
    }

    void DelayCheck()
    {
        lastJump += Time.deltaTime;
        lastAttack += Time.deltaTime;
    }

    void Landing()
    {
        if (Raycast.instance.raycasting && lastJump > jumpDelay)
        {
            jumping = false;
            landing = true;
        }
        else
        {
            landing = false;
        }
    }

    void Jump()
    {
        SoundEffect.instance.JumpSound();
        jumping = true;
        lastJump = 0;
        waitTime = 0;
        rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

    IEnumerator Slide()
    {
        sliding = true;
        PlayerStatus.instance.Slide();
        anim.SetTrigger("Slide");
        rigid.velocity = Vector3.zero;
        rigid.AddForce(transform.TransformDirection(Vector3.forward) * slidePower * 0.5f, ForceMode.Impulse);

        yield return new WaitForSeconds(0.2f);

        SoundEffect.instance.SlideSound();
        rigid.velocity = Vector3.zero;
        rigid.AddForce(transform.TransformDirection(Vector3.forward) * slidePower, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);

        rigid.velocity = Vector3.zero;
        sliding = false;

        yield return null;
    }

    IEnumerator Attack()
    {
        lastAttack = 0;
        waitTime = 0;
        attacking = true;
        rigid.velocity = Vector3.zero;

        if (!attackCombo)
        {
            attackCombo = true;
            anim.SetTrigger("Attack0");
        }
        else
        {
            attackCombo = false;
            anim.SetTrigger("Attack1");
        }

        yield return new WaitForSeconds(0.2f);

        SoundEffect.instance.SlashSound();

        yield return new WaitForSeconds(0.6f);

        anim.ResetTrigger("Attack0");
        anim.ResetTrigger("Attack1");
        attacking = false;

        yield return null;
    }

    void IdleMotion()
    {
        if (waitTime > 10f)
        {
            int random = Random.Range(0, 4);

            if (!waitingMotion)
            {
                waitingMotion = true;
                StartCoroutine(WaitExit(random));
            }

            switch (random)
            {
                case 0:
                    anim.SetBool("Wait0", true);
                    break;
                case 1:
                    anim.SetBool("Wait1", true);
                    break;
                case 2:
                    anim.SetBool("Wait2", true);
                    break;
                case 3:
                    anim.SetBool("Wait3", true);
                    break;
            }
        }
        else
        {
            StopCoroutine(WaitExit(-1));
            anim.SetBool("Wait0", false);
            anim.SetBool("Wait1", false);
            anim.SetBool("Wait2", false);
            anim.SetBool("Wait3", false);
        }
    }

    IEnumerator WaitExit(int num)
    {
        switch (num)
        {
            case 0:
                yield return new WaitForSeconds(6.6f);
                break;
            case 1:
                yield return new WaitForSeconds(8.3f);
                break;
            case 2:
                yield return new WaitForSeconds(5.167f);
                break;
            case 3:
                yield return new WaitForSeconds(5.167f);
                break;
        }

        waitTime = 0;
        waitingMotion = false;
        yield return null;
    }

    public IEnumerator Hit()
    {
        SoundEffect.instance.HitSound();
        rigid.velocity = Vector3.zero;
        waitTime = 0;
        hit = true;
        run = false;
        attacking = false;
        attackCombo = false;
        sliding = false;
        anim.SetTrigger("Hit");

        yield return new WaitForSeconds(3.7f);

        hit = false;

        yield return null;
    }

    void Die()
    {
        die = true;
        StartCoroutine(GameManager.instance.GameOver());
        anim.ResetTrigger("Hit");
        anim.SetTrigger("Die");
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyAttack") && !hit && !sliding && !die)
        {
            SoundEffect.instance.BossAttack();
            PlayerStatus.instance.Hit(15f);
            StartCoroutine(Hit());
        }
    }
}
