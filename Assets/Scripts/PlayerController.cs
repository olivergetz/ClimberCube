using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool isGrounded = false;
    bool wasGrounded = false;
    bool canJump = true;
    bool jumpAddForce = false;

    [SerializeField] float thrust = 200f;
    [SerializeField] float speed = 20f;
    [SerializeField] float rotationSpeed = 20f;
    [SerializeField] float groundedThreshold = 5f;
    [SerializeField] float lightFlashBase = 5f;
    [SerializeField] float lightFlashMax = 5f;
    [SerializeField] [Range(0f, 1f)] float playerGlitchMax = .3f;
    [SerializeField] [Range(0f, 1f)] float glitchResetTime = .5f;

    float translation;
    float playerDistance;
    float distanceToGoal;
    float normalizedDistance = 0f;
    float distancePercent = 0f;

    readonly uint jumpSoundID = 3580720154U;
    readonly uint landSoundID = 3580720152U;
    readonly uint musicStartID = 1193902085U;
    readonly uint pulseSoundID = 553658201U;
    readonly uint playerProgressRTPCID = 3056980031U;
    readonly uint MusicSwitchGroupID = 3508446603U;
    readonly uint MusicLvlSwitch00ID = 987635872U;
    readonly uint MusicWinGameSwitchID = 3258849071U;

    [SerializeField] GameObject pulseParticleObj;
    [SerializeField] GameObject jumpParticlesObj;
    [SerializeField] GameObject lightObj;
    [SerializeField] GameObject goal;

    Color baseColor;
    [SerializeField] Color glitchColor;

    public CameraMovement cameraShake;

    ParticleSystem pulseParticles;
    ParticleSystem jumpParticles;

    Rigidbody rb;
    Animator animator;

    GlitchImageFX glitchFX;

    Light light;

    // Start is called before the first frame update
    void Start()
    {
        //Start Music
        AkSoundEngine.PostEvent(musicStartID, gameObject, 0x0100, MusicCallback, null);
        //AkSoundEngine.PostEvent(musicStartID, gameObject);
        AkSoundEngine.SetSwitch(MusicSwitchGroupID, MusicLvlSwitch00ID, gameObject);

        //Snap Glitch
        GlitchFade(0f,0f);

        //Set references
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        pulseParticles = pulseParticleObj.GetComponent<ParticleSystem>();
        jumpParticles = jumpParticlesObj.GetComponent<ParticleSystem>();

        light = lightObj.GetComponent<Light>();
        baseColor = light.color;

        glitchFX = new GlitchImageFX();

        //Initalize Variables
        light.range = lightFlashBase;
    }

    void Update()
    {
        //Calculate distance to goal
        distanceToGoal = gameObject.transform.position.y - goal.transform.position.y;

        //If the distance is negative, make it positive.
        if (distanceToGoal < 0)
        {
            distanceToGoal = distanceToGoal * -1;
        }

        //Convert distanceToGoal to a range between 100 and 0, where goal position = 100% and playerPosition = 0%.
        normalizedDistance = Mathf.InverseLerp(goal.transform.position.y, 0f, distanceToGoal);
        //Convert normalizedDistance to percentage.
        distancePercent = normalizedDistance * 100;

        //Set RTCP, which in turn changes switches for music development in Wwise.
        AkSoundEngine.SetRTPCValue(playerProgressRTPCID, distancePercent, gameObject);

        if (Input.GetButtonDown("Fire1"))
        {
            /*
             * Note: When posting triggers, the track triggering the stinger 
             * must reference the same gameobject. 
            */
            AkSoundEngine.PostEvent(pulseSoundID, gameObject);
            
            animator.SetTrigger("Pulsate");
            pulseParticles.Play();

            StartCoroutine(Flash());
        }

        if (isGrounded && !wasGrounded)
        {
            //If player just landed
            AkSoundEngine.PostEvent(landSoundID, gameObject);
            animator.SetTrigger("Land");
        }

        wasGrounded = isGrounded;

        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            if (Vector3.up == gameObject.transform.up || Vector3.up == -gameObject.transform.up)
            {
                animator.SetTrigger("Jump");
            }
            else
            {
                animator.SetTrigger("JumpAlt");
            }
            AkSoundEngine.PostEvent(jumpSoundID, gameObject);
            jumpParticles.Play();
            jumpAddForce = true;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (jumpAddForce)
        {
            rb.AddForce(Vector3.up * thrust * Time.deltaTime, ForceMode.Impulse);
            jumpAddForce = false;
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            rb.AddForce(Input.GetAxis("Horizontal") * speed, 0, 0);
        }

        //Is the player grounded?

        /*METHOD 1: VELOCITY
         * Doesn't work well if I want to do something on the first grounded frame, since velocity 
         * will also be low during the turning point in the air. Use Raycast.
        */

        /*
        //Debug.Log(rb.velocity.y); //For threshold calibration
        if (rb.velocity.y > 2f || rb.velocity.y < -2f)
        {
            //Debug.Log("Not grounded");
            isGrounded = false;
        }
        else
        {
            isGrounded = true;
        }
        */

        /*METHOD 2: RAYCAST*/
        if (Physics.Raycast(transform.position, Vector3.down, groundedThreshold))
        {
            //Debug.Log("Grounded");
            isGrounded = true;
            canJump = true;
        }
        else
        {
            //Debug.Log("Not grounded");
            isGrounded = false;
            canJump = false;
        }
    }

    void MusicCallback(object in_cookie, AkCallbackType in_type, object in_info)
    {
        //Snare Drum Sound
        //Debug.Log("PSCHT");
        //Debug.Log(normalizedDistance);
        if (normalizedDistance > .5)
        {
            //Debug.Log("DONK!");
            StartCoroutine(cameraShake.CameraShake(.5f, Mathf.Lerp(0f, 1f, normalizedDistance)));
        }
        StartCoroutine(GlitchFade(Mathf.Lerp(0f, playerGlitchMax / 10, normalizedDistance), glitchResetTime / 10));
    }

    IEnumerator GlitchFade(float strength, float resetTime)
    {
        glitchFX.Glitch(strength, GlitchImageFX.material);
        for (float f = 1.0f; f > 0f; f = f - .05f)
        {   
            glitchFX.Glitch(Mathf.Lerp(0f, strength, f), GlitchImageFX.material);
            if (normalizedDistance > .45)
            {
                light.color = Color.Lerp(baseColor, Color.Lerp(baseColor, glitchColor, f), normalizedDistance);
            }
            else light.color = baseColor;

            yield return new WaitForSeconds(resetTime);
        }
        yield return null;
    }

    IEnumerator Flash()
    {
        light.range = lightFlashMax;
        for (float f = 1.0f; f > 0f; f = f - .05f)
        {
            light.range = Mathf.Lerp(lightFlashBase, lightFlashMax, f);
            yield return new WaitForSeconds(0.025f);
        }
    }

}
