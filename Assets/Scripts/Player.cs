using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{

    public CharacterController controller;
    public Transform thirdPersonCamera;
    public TextMeshProUGUI scoreUI;
    public int score = 0;

    //Movement
    public float speed = 6.0f;
    public float diveSpeed = 20f;
    public float jumpHeight = 3f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    Vector3 velocity;
    bool isDiving = false;

    //Gravity
    public float gravity = -24f;//-9.81f;
    public Transform groundCheck;//sphere collider
    public float groundDistance = 0.4f;//sphere radius
    public LayerMask groundMask;//used to control what objects the collider checks for
    bool isGrounded;

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        PlayerMovement();

        //Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        //Dive
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!isDiving)
            {
                isDiving = true;
                StartCoroutine(DiveWait());
            }
        }

        velocity.y += gravity * Time.deltaTime;//Add gravity to player
        controller.Move(velocity * Time.deltaTime);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Pickup")
        {
            if (!collision.gameObject.GetComponent<Pickup>().IsCollected)
            {
                score++;
                scoreUI.text = ("SCORE: ") + score.ToString();
                collision.gameObject.GetComponent<Pickup>().GetPickedUp();
            }
        }

        if (collision.gameObject.name == "Finish Zone")
        {
            if (collision.gameObject.GetComponent<FinishZone>().finishZoneOpen)
            {
                Debug.Log("You Win");
            }
        }
    }

    void PlayerMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;//normalising stops faster movement on the diagonal 

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + thirdPersonCamera.eulerAngles.y;//Returns angle on the x axis based off the y //* by radians to convert into degrees
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);//allows for smoother rotation
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }
    }
    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //creates sphere

        if (isGrounded && velocity.y < 2)
        {
            velocity.y = -2f;//forces player on ground
        }
    }

    IEnumerator DiveWait()
    {
        float baseSpeed = speed;
        speed = diveSpeed;
        yield return new WaitForSeconds(0.1f);
        speed = baseSpeed;
        yield return new WaitForSeconds(0.5f);
        isDiving = false;
    }
}
