using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;
    private Transform cameraParent;
    private GUIStyle style;

    [SerializeField] private GameObject cameraRig;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject inventoryUI;

    private float speed;
    private float gravity = 15f;
    private float jumpForce = 7f;
    private Vector3 vertVelocity;
    private Vector3 direction, moveDirection;
    private float smoothRate = 0.1f;
    private float turnSmoothRef;
    private bool isDead;
        
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

        InitialiseCamera();
        InitialiseGUIStyle();
    }
        
    void Update()
    {
        UpdateAnimation();

        Kill();

        if (isDead)
            return;

        Movement();
    }
    
    void Kill()
    {
        if(Input.GetKeyDown(KeyCode.X) && !isDead)
        {
            isDead = true;
        }
    }

    void UpdateAnimation()
    {
        anim.SetFloat("Speed", direction.sqrMagnitude);
        anim.SetBool("IsDead", isDead);
    }

    void Movement()
    {
        //Jump Code
        if (IsGrounded())
        {
            if (Input.GetButtonDown("Jump"))
            {
                vertVelocity.y = jumpForce;
            }
        }
        else
        {
            vertVelocity.y -= gravity * Time.deltaTime;
        }

        controller.Move(vertVelocity * Time.deltaTime);

        //Directional code
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        direction = new Vector3(horizontal, 0f, vertical);

        if (direction.sqrMagnitude >= 0.1f)
        {
            float turnAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraParent.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, turnAngle, ref turnSmoothRef, smoothRate);
            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            if (direction.sqrMagnitude > 1)
                direction.Normalize();

            //Move direction takes rotation angle, multiplied by forward vector, multiplied by length of direction input
            //This allows for gradated analog movement
            moveDirection = (Quaternion.Euler(0, turnAngle, 0) * Vector3.forward) * direction.magnitude;

            //Movement speed is scaled with input magnitude - so less input, the character moves slower, helps
            //reinforce the animation and stop 'sliding' feet
            speed = Mathf.Lerp(0f, 7f, direction.magnitude);

            controller.Move(moveDirection * speed * Time.deltaTime);
        }                
    }

    void InitialiseCamera()
    {
        GameObject go = Instantiate(cameraRig, transform.position, transform.rotation);
        go.name = cameraRig.name;
        go.GetComponent<CameraController>().Player = transform;
        cameraParent = go.transform;
    }    

    void InitialiseGUIStyle()
    {
        style = new GUIStyle();
        style.normal.textColor = Color.black;
    }

    bool IsGrounded()
    {
        Ray ray = new Ray(new Vector3(controller.bounds.center.x, (controller.bounds.center.y - controller.bounds.extents.y), controller.bounds.center.z), Vector3.down);
        return Physics.Raycast(ray, 0.3f);
    }
}
