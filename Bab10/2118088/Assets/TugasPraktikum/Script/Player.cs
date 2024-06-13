using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    public Animator animator;
    public GameObject bullet;
    public Transform firePoint;

    Rigidbody2D rb;
    [SerializeField] Transform groundcheckCollider;
    [SerializeField] LayerMask groundLayer; 

    const float groundCheckRadius = 0.2f; // +
    [SerializeField] float speed = 1;
    [SerializeField] float jumpPower = 100;  

    float horizontalValue;

    [SerializeField] bool isGrounded; // +
    bool facingRight;
    bool jump;
    bool menyerang;

    public int nyawa;
    [SerializeField] Vector3 respawn_loc;
    public bool play_again;

  private void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
    respawn_loc = transform.position;
  }

  void playagain()
  {
    if(play_again == true)
    {
      nyawa = 3;
      transform.position = respawn_loc;
      play_again = false;
    }
  }

    void Update () 
    {
        horizontalValue = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")){
            animator.SetBool("Jumping", true);
            jump = true;
        }
        else if (Input.GetButtonUp("Jump"))
        jump = false;
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(Attack());
        }
        if (Input.GetKeyDown(KeyCode.C) && !menyerang)
        {
          StartCoroutine(Attack());
        }
        if(nyawa < 0)
        {
          playagain();
        }
        if(transform.position.y < -10)
        {
          play_again = true;
          playagain();
        } 
    }
  
    void FixedUpdate()
    {
        GroundCheck();
        Move(horizontalValue, jump, menyerang);
        animator.SetFloat("Blend", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("Blend Jump",rb.velocity.y);
    }
    IEnumerator Attack()
    {
      
      animator.SetTrigger("Attack");
      menyerang = true;
      animator.SetBool("menyerang", true);
      yield return new WaitForSeconds(0.25f);
      
      float direction = facingRight ? 1f : -1f;
      
      GameObject fireball = Instantiate(bullet, firePoint.position, Quaternion.identity);
      fireball.GetComponent<Rigidbody2D>().velocity = new Vector2(direction * 10f, 0);

      animator.SetBool("menyerang", false);
      menyerang = false;
      
      Destroy(fireball, 2f);
    }

    void GroundCheck()
    {
        isGrounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundcheckCollider.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0){
            isGrounded = true;
        }
        animator.SetBool("Jumping", !isGrounded);
    }
  
  void Move(float dir, bool jumpflag, bool serangFlag)
  {
    float xVal = dir * speed * 100 * Time.fixedDeltaTime;
    Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
    rb.velocity = targetVelocity;
    menyerang = serangFlag;

    if(isGrounded && serangFlag)
    {
      serangFlag = true;
    }

    if(isGrounded && jumpflag)
    {
      isGrounded = false;
      jumpflag = false;
      rb.AddForce(new Vector2(0f, jumpPower));
    }
    if (facingRight && dir < 0)
    {
      // ukuran player
      transform.localScale = new Vector3(-0.15f, 0.15f, 0.15f);
      facingRight = false;
    }

    else if (!facingRight && dir > 0)
    {
      // ukuran player
      transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
      facingRight = true;
    }
  }


}
