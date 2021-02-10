using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool isGrounded = false;
    bool wasGrounded = false;

    [SerializeField] float thrust = 200f;
    [SerializeField] float speed = 20f;
    [SerializeField] float rotationSpeed = 20f;

    float translation;

    Rigidbody rb;
    Animator animator;

    [SerializeField] GameObject pulseParticleObj;
    [SerializeField] GameObject jumpParticlesObj;

    ParticleSystem pulseParticles;
    ParticleSystem jumpParticles;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        pulseParticles = pulseParticleObj.GetComponent<ParticleSystem>();
        jumpParticles = jumpParticlesObj.GetComponent<ParticleSystem>();

    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Pulsate");
            pulseParticles.Play();
        }

        if (isGrounded && !wasGrounded)
        {
            //If player just landed
            animator.SetTrigger("Land");
        }

        wasGrounded = isGrounded;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            if (Vector3.up == gameObject.transform.up || Vector3.up == -gameObject.transform.up)
            {
                animator.SetTrigger("Jump");
            }
            else
            {
                animator.SetTrigger("JumpAlt");
            }
            //Debug.Log("Boing!");
            rb.AddForce(Vector3.up * thrust);
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            rb.AddForce(Input.GetAxis("Horizontal") * speed, 0, 0);
        }

        //Debug.Log(rb.velocity.y);
        if (rb.velocity.y > 2f || rb.velocity.y < -2f)
        {
            //Debug.Log("Not grounded");
            isGrounded = false;
        }
        else
        {
            isGrounded = true;
        }
    }

}
