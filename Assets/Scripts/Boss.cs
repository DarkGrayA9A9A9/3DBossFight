using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class Boss : MonoBehaviour
{
    public float moveSpeed;
    public float rotSpeed;
    public float chaseMinDistance;
    public float chaseMaxDistance;

    public float lastPattern;
    public float patternTime;
    public float lastHit;

    public float attack;
    public float jumpAttack;

    public float currentHealth;
    public float maxHealth;

    public int moveChangeCnt;

    public bool attacking;
    public bool jumpAttacking;
    public bool die;

    Vector3 playerDir;

    public GameObject target;

    public Slider healthSlider;
    public GameObject healthSliderObj;

    Rigidbody rigid;
    CapsuleCollider collider;
    Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        GameObject hit = transform.GetChild(3).gameObject;
        hit.GetComponent<ParticleSystem>().enableEmission = false;

        GameObject slash = transform.GetChild(4).gameObject;
        slash.GetComponent<ParticleSystem>().enableEmission = false;

        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (die)
            return;

        Approach();
    }

    void LateUpdate()
    {
        if (die)
            return;

        lastHit += Time.deltaTime;

        if (playerDir.magnitude < chaseMaxDistance)
            healthSliderObj.SetActive(true);

        healthSlider.value = currentHealth / maxHealth;

        if (currentHealth <= 0f)
            Die();
    }

    void FixedUpdate()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void Approach()
    {
        playerDir = target.transform.position - this.transform.position;

        if (playerDir.magnitude < chaseMaxDistance)
            lastPattern += Time.deltaTime;

        if (!attacking && !jumpAttacking && playerDir.magnitude < chaseMaxDistance && lastPattern < patternTime)
        {
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(playerDir), Time.deltaTime * rotSpeed);

            if (playerDir.magnitude > chaseMinDistance && moveChangeCnt < 3 && !anim.GetBool("Move"))
            {
                anim.SetBool("Move", true);
                moveChangeCnt++;
            }
                
            
            if (playerDir.magnitude < chaseMinDistance && anim.GetBool("Move"))
            {
                anim.SetBool("Move", false);
                moveChangeCnt++;
            }
                
        }

        if (!attacking && !jumpAttacking && playerDir.magnitude < chaseMaxDistance && lastPattern > patternTime)
        {
            if (playerDir.magnitude < chaseMinDistance * 2f)
            {
                StartCoroutine(Attack());
            }
            else
            {
                StartCoroutine(JumpAttack());
            }
        }
    }

    IEnumerator Attack()
    {
        attacking = true;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);

        SoundEffect.instance.BossSlash();

        yield return new WaitForSeconds(0.2f);

        GameObject atk = transform.GetChild(2).gameObject;
        atk.GetComponent<BoxCollider>().enabled = true;

        GameObject slash = transform.GetChild(4).gameObject;
        slash.GetComponent<ParticleSystem>().enableEmission = true;
        slash.GetComponent<ParticleSystem>().Play();

        yield return new WaitForSeconds(0.2f);

        atk.GetComponent<BoxCollider>().enabled = false;

        yield return new WaitForSeconds(0.3f);

        slash.GetComponent<ParticleSystem>().enableEmission = false;
        slash.GetComponent<ParticleSystem>().Stop();

        yield return new WaitForSeconds(0.9f);

        float random = Random.Range(1f, 2f);
        patternTime = random;

        lastPattern = 0f;
        moveChangeCnt = 0;
        attacking = false;

        yield return null;
    }

    IEnumerator JumpAttack()
    {
        jumpAttacking = true;
        anim.SetTrigger("JumpAttack");

        yield return new WaitForSeconds(1.6f);

        SoundEffect.instance.BossJumpAttack();

        if (playerDir.magnitude < chaseMinDistance * 10f && PlayerController.instance.landing && !PlayerController.instance.sliding && !PlayerController.instance.die && !PlayerController.instance.hit)
        {
            StartCoroutine(PlayerController.instance.Hit());
            PlayerStatus.instance.Hit(jumpAttack);
        }

        yield return new WaitForSeconds(2.4f);

        float random = Random.Range(1f, 2f);
        patternTime = random;

        lastPattern = 0f;
        moveChangeCnt = 0;
        jumpAttacking = false;

        yield return null;
    }

    IEnumerator Hit()
    {
        float random = Random.Range(0.5f, 1.5f);
        currentHealth -= PlayerStatus.instance.attack * random;

        GameObject hit = transform.GetChild(3).gameObject;
        hit.GetComponent<ParticleSystem>().enableEmission = true;
        hit.GetComponent<ParticleSystem>().Play();

        yield return new WaitForSeconds(0.5f);

        hit.GetComponent<ParticleSystem>().enableEmission = false;
        hit.GetComponent<ParticleSystem>().Stop();

        yield return null;
    }

    void Die()
    {
        die = true;
        GameManager.instance.clear = true;
        anim.SetTrigger("Die");
        collider.direction = 2;
        collider.radius = 0.25f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Attack") && lastHit > 0.3f)
        {
            lastHit = 0;
            SoundEffect.instance.AttackSound();
            StartCoroutine(Hit());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && jumpAttacking && !PlayerController.instance.hit && !PlayerController.instance.sliding && !PlayerController.instance.die)
        {
            StartCoroutine(PlayerController.instance.Hit());
            PlayerStatus.instance.Hit(jumpAttack);
        }
    }
}
