using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class player : MonoBehaviour
{

    private Rigidbody2D rigi;

    private Animator MyAnimu;

    [SerializeField]
    private float movementSpeed;

    private bool facingRight;

    private bool attack;

    [SerializeField]
    private Transform[] groundPoints;

    [SerializeField]
    private float groundRadius;

    [SerializeField]
    private LayerMask whatIsGround;

    private bool isGrounded;

    private bool jump;

    private bool jumpAttack;

    [SerializeField]
    private bool aircontrol;

    [SerializeField]
    private float jumpForce;

    public int count;
    public Text countText;

    [SerializeField]
    private Collider2D attackTrigger;

    [SerializeField]
    private Collider2D attackTriggerSlide;

    public float max_health = 100;
    public float cur_health = 0f;
    public GameObject healthBar;

    private SpriteRenderer sprite;

    [SerializeField]
    private GameObject HpDown;

    private bool slide;

    public bool immortal;

    // [SerializeField]
    private float immortalTime = 200f;

    // Use this for initialization
    void Start()
    {

        facingRight = true;
        rigi = GetComponent<Rigidbody2D>();
        MyAnimu = GetComponent<Animator>();

        sprite = GetComponent<SpriteRenderer>();
        cur_health = max_health;

        //InvokeRepeating("decreasehealth", 1f, 1f);
    }

    void Update()
    {
        handleInput();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");

        isGrounded = IsGrounded();

        HandleMovement(horizontal);
        Flip(horizontal);

        handleAttacks();
        Handlelayer();

        resetvalues();
        AfterAttack();

        hpStore();

       
    }


    public void decreasehealth()
    {
        if (count >= 15)
        {
            cur_health -= 1.5f;
        }

        if (count >= 25)
        {
            cur_health -= 1.5f;
        }
        cur_health -= 10f;
        float calc_health = cur_health / max_health;
        SetHealthBar(calc_health);

        if (cur_health <= 0)
        {
            Application.LoadLevel("gameOver");
        }
    }

    public void SetHealthBar(float myHealth)
    {
        healthBar.transform.localScale = new Vector2(myHealth, healthBar.transform.localScale.y);
    }

    private void HandleMovement(float horizontal)
    {
        if (rigi.velocity.y < 0)
        {
            MyAnimu.SetBool("land", true);
        }

        if (!this.MyAnimu.GetBool("slide") && !this.MyAnimu.GetCurrentAnimatorStateInfo(0).IsTag("attack") && (isGrounded || aircontrol))
        {
            rigi.velocity = new Vector2(horizontal * movementSpeed, rigi.velocity.y);
        }

        MyAnimu.SetFloat("speed", Mathf.Abs(horizontal));

        if (isGrounded && jump)
        {
            isGrounded = false;
            rigi.AddForce(new Vector2(0, jumpForce));
            MyAnimu.SetTrigger("jump");
            //if (rigi.position.y > 3)
            //{
            // rigi.gravityScale = 1.3f;
            //}
            attackTrigger.enabled = true;

        }
        if(isGrounded && !jump && this.MyAnimu.GetCurrentAnimatorStateInfo(1).IsTag("landsu") &&
            this.MyAnimu.GetCurrentAnimatorStateInfo(0).IsName("idle_animation") || isGrounded && !jump && this.MyAnimu.GetCurrentAnimatorStateInfo(0).IsName("Run_animation")
             && this.MyAnimu.GetCurrentAnimatorStateInfo(1).IsTag("landsu"))
        {
            attackTrigger.enabled = false;
        }

        if (this.MyAnimu.GetCurrentAnimatorStateInfo(1).IsTag("landsu"))
        {
            rigi.gravityScale = 2f;
            // rigi.mass = 1.5f;
        }
        else
        {
           // rigi.gravityScale = 1.3f;
            //rigi.mass = 1;
        }

        if(slide && !this.MyAnimu.GetCurrentAnimatorStateInfo(0).IsName("slide"))
        {
            MyAnimu.SetBool("slide", true);
            attackTriggerSlide.enabled = true;
        }
        else if (!this.MyAnimu.GetCurrentAnimatorStateInfo(0).IsName("slide"))
        {
            MyAnimu.SetBool("slide", false);
            //attackTriggerSlide.enabled = false;
        }

        if (!slide && !this.MyAnimu.GetCurrentAnimatorStateInfo(0).IsName("slide") && 
            this.MyAnimu.GetCurrentAnimatorStateInfo(0).IsName("idle_animation") || this.MyAnimu.GetCurrentAnimatorStateInfo(0).IsName("Run_animation") 
            && !slide && !this.MyAnimu.GetCurrentAnimatorStateInfo(0).IsName("slide"))
        { attackTriggerSlide.enabled = false; }

    }

    private void handleAttacks()
    {
        if (attack && isGrounded && !this.MyAnimu.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
        {
            MyAnimu.SetTrigger("attack");
            rigi.velocity = Vector2.zero;
            //attackTrigger.enabled = true;


        }
        if (!attack && isGrounded && !this.MyAnimu.GetCurrentAnimatorStateInfo(0).IsTag("attack") ||
        !jumpAttack && !isGrounded && !this.MyAnimu.GetCurrentAnimatorStateInfo(1).IsName("jumpAttack"))
        {
            //attackTrigger.enabled = false;
        }



        if (jumpAttack && !isGrounded && !this.MyAnimu.GetCurrentAnimatorStateInfo(1).IsName("jumpAttack"))
        {
            MyAnimu.SetBool("jumpAttack", true);
            attackTrigger.enabled = true;
        }

        if (!jumpAttack && !this.MyAnimu.GetCurrentAnimatorStateInfo(1).IsName("jumpAttack"))
        {
            MyAnimu.SetBool("jumpAttack", false);
        }

    }

    private void handleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            jump = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            attack = true;
            jumpAttack = true;

        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            slide = true;
        }

    }

    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;

            Vector3 TheScale = transform.localScale;

            TheScale.x *= -1;

            transform.localScale = TheScale;
        }
    }

    private void resetvalues()
    {
        attack = false;
        jump = false;
        jumpAttack = false;
        slide = false;
    }

    private bool IsGrounded()
    {
        if (rigi.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != gameObject)
                    {
                        MyAnimu.ResetTrigger("jump");
                        MyAnimu.SetBool("land", false);
                        return true;
                    }
                }

            }
        }
        return false;
    }

    private void Handlelayer()
    {
        if (!isGrounded)
        {
            MyAnimu.SetLayerWeight(1, 1);
        }
        else
        {
            MyAnimu.SetLayerWeight(1, 0);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.CompareTag("Coins"))
        //{
        //    other.gameObject.SetActive(false);
        //    count = count + 1;
        //    SetCountText();
        //    //Destroy(other.gameObject);
        //}

        if (other.CompareTag("Enemy"))
        {
            if (attackTrigger.enabled == false)
            {

                if (!immortal)
                {
                    decreasehealth();
                    immortal = true;

                }
            }

        }

        //if (other.CompareTag("dead"))
        //{
        //    rigi.transform.position = new Vector2(3.2f, -0.36f);
            
        //        decreasehealth();
        //        immortal = true;
            
        //}

        //if (other.CompareTag("dead2"))
        //{
        //    rigi.transform.position = new Vector2(76.84f, -0.36f);
           
        //        decreasehealth();
        //        immortal = true;
            
        //}

        //if (other.CompareTag("dead3"))
        //{
        //    rigi.transform.position = new Vector2(0f, -0.96f);

        //    decreasehealth();
        //    immortal = true;

        //}

        //if (other.CompareTag("dead4"))
        //{
        //    rigi.transform.position = new Vector2(105.54f, -0.96f);

        //    decreasehealth();
        //    immortal = true;

        //}

        //if (other.CompareTag("fly"))
        //{
        //    rigi.AddForce(new Vector2(0, jumpForce * 2.5f));
        //}


    }
    //void OnCollisionEnter2D(Collision2D other)
    //{
    //    if (other.transform.tag == "MovingPlatform")
    //    { transform.parent = other.transform; }
    //}

    //void OnCollisionExit2D(Collision2D other)
    //{
    //    if (other.transform.tag == "MovingPlatform")
    //    { transform.parent = null; }
    //}

    public void AfterAttack()
    {
        if (immortal == true)
        {
            immortalTime--;
            StartCoroutine(takedamage());
        }

        if (immortalTime == 0)
        {
            immortal = false;
            immortalTime = 200f;
        }
    }

    IEnumerator takedamage()
    {
        if (immortal == true)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(.1f);
            sprite.enabled = true;
            yield return new WaitForSeconds(.1f);
        }

    }

    public void SetCountText()
    {
        countText.text = "Points: " + count.ToString();
    }

    public void hpStore()
    {
        if (cur_health < 100 && count >= 5)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                cur_health += 10;
                float calc_health = cur_health / max_health;
                SetHealthBar(calc_health);
                count -= 5;
            }
        }

        if (cur_health > 100)
        {
            cur_health = 100;
        }

        if (cur_health < 100 && count >= 5)
        {
            HpDown.SetActive(true);
        }
        else
        {
            HpDown.SetActive(false);
        }
    }

}
