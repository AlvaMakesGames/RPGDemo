using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class FirstPersonPlayer : MonoBehaviour
{  
    [SerializeField] private ParticleSystem smoke, flash;
    [SerializeField] private Transform playerCam;
    [SerializeField] private AudioSource sound;
    [SerializeField] private Light flashLight;

    private CharacterController controller;
    private Animation anim;

    private float speed = 5f;
    private float rotSpeed = 180f;
    private float gravity = 9.81f;
    private float jumpForce = 2.5f;
    private float mouseSens = 3f;
    private float fireRate = 0.7f;
    private float vertVel, mouseX, mouseY, shootCoolDown;
    private bool canShoot = true;
    private bool isCrouching;
    private Item pickUp;
    private bool foundInArray, freeSlotFound;
    private int indexToAdd, amtToAdd;
    private bool toggle;
    
    void Start ()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animation>();
        Cursor.lockState = CursorLockMode.Locked;
	}

    void Update()
    {
        ToggleInventory();

        if (SingletonManager.Instance.GamePause)
            return;

        Movement();
        Crouch();
        Shoot();
        TraceForItem();
    }

    void Movement()
    {
        Vector3 movement = Vector3.zero;
        Vector3 rotation = Vector3.zero;

        //Calculate X Movement
        movement.x = Input.GetAxis("Horizontal");

        //Calculate Y Movement
        if (IsGrounded())
        {
            if (Input.GetKeyDown(KeyCode.Space))
                vertVel = jumpForce;
        }
        else
        {
            vertVel -= gravity * Time.deltaTime;
        }

        movement.y = vertVel;

        //Calculate Z Movement
        movement.z = Input.GetAxis("Vertical");

        //Calculate Rotation
        float pitch = -Input.GetAxis("Mouse Y") * mouseSens;
        rotation.y = Input.GetAxis("Mouse X") * mouseSens;
        pitch = Mathf.Clamp(pitch, -60f, 60f);

        playerCam.Rotate(new Vector3(pitch, 0f, 0f));

        movement = transform.TransformDirection(movement);

        //Apply transforms
        transform.Rotate(rotation * rotSpeed * Time.deltaTime);
        controller.Move(movement * speed * Time.deltaTime);
    }

    void Crouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!isCrouching)
                StartCoroutine(Crouch(2, 1));
            else
                StartCoroutine(Crouch(1, 2));
        }
    }

    bool IsGrounded()
    {
        Ray ray = new Ray(new Vector3(controller.bounds.center.x, (controller.bounds.center.y - controller.bounds.extents.y), controller.bounds.center.z), Vector3.down);
        return (Physics.Raycast(ray, 0.3f));
    }

    #region Inventory Methods
    void ToggleInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            toggle = !toggle;

            SingletonManager.Instance.ToggleInventory(toggle);
        }
    }

    void TraceForItem()
    {
        if (Input.GetButtonDown("Interact"))
        {
            float checkDistance = 3f;
            RaycastHit hit;

            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out hit, checkDistance))
            {
                if (hit.collider.CompareTag("Pickup"))
                {
                    pickUp = hit.collider.GetComponent<Item>();
                    amtToAdd = pickUp.AmtToAdd;
                    AddToInventory();
                }
                else
                {
                    pickUp = null;
                    amtToAdd = 0;
                }
            }
        }
    }

    void SetArrayElement(int index, ItemData data, int amtToAdd)
    {
        SingletonManager.Instance.Inventory[index].Type = data.Type;
        SingletonManager.Instance.Inventory[index].Name = data.Name;
        SingletonManager.Instance.Inventory[index].Action = data.Action;
        SingletonManager.Instance.Inventory[index].Image = data.Image;
        SingletonManager.Instance.Inventory[index].CurrentQuantity = amtToAdd;
        SingletonManager.Instance.Inventory[index].MaxQuantity = data.MaxQuantity;
        SingletonManager.Instance.Inventory[index].CanStack = data.CanStack;
    }

    void ModifyArrayElement(int index, int amtToAdd)
    {
        SingletonManager.Instance.Inventory[index].CurrentQuantity += amtToAdd;
    }

    void FillStack(Item item, int amt, int index, bool newSlot)
    {
        if(newSlot)
        {
            int amtToFillStack = item.Data.MaxQuantity;

            if(amt > amtToFillStack)
            {
                SetArrayElement(index, item.Data, amtToFillStack);
                int remainder = amt - amtToFillStack;
                amtToAdd = remainder;

                AddToInventory();                
            }
            else
            {
                SetArrayElement(index, item.Data, amt);
                Destroy(item.gameObject);
            }
        }
        else
        {
            int amtToFillStack = SingletonManager.Instance.Inventory[index].MaxQuantity - SingletonManager.Instance.Inventory[index].CurrentQuantity;

            if(amt > amtToFillStack)
            {
                ModifyArrayElement(index, amtToFillStack);
                int remainder = amt - amtToFillStack;
                amtToAdd = remainder;

                AddToInventory();
            }
            else
            {
                ModifyArrayElement(index, amt);
                Destroy(item.gameObject);
            }
        }
    }

    void AddToInventory()
    {
        if(pickUp.Data.CanStack)
        {
            if (CheckForExistingStack())
                FillStack(pickUp, amtToAdd, indexToAdd, false);
            else if (CheckForFreeSlot())
                FillStack(pickUp, amtToAdd, indexToAdd, true);
            else
                print("No free slot available.");
        }
        else
        {
            if (CheckForFreeSlot())
                FillStack(pickUp, amtToAdd, indexToAdd, true);
            else
                print("No free slot available.");
        }
    }

    bool CheckForExistingStack()
    {
        foundInArray = false;
        int index = 0;

        foreach(ItemData slot in SingletonManager.Instance.Inventory)
        {
            if (slot.Type == pickUp.Data.Type && slot.CurrentQuantity < slot.MaxQuantity)
            {
                foundInArray = true;
                indexToAdd = index;
                break;
            }

            index++;
        }

        return foundInArray;
    }

    bool CheckForFreeSlot()
    {
        freeSlotFound = false;
        int index = 0;

        foreach(ItemData slot in SingletonManager.Instance.Inventory)
        {
            if(slot.Type == ItemType.None)
            {
                freeSlotFound = true;
                indexToAdd = index;
                break;
            }

            index++;
        }

        return freeSlotFound;
    }
    #endregion

    #region Shooting
    void Shoot()
    {
        if(Input.GetMouseButton(0))
        {
            if(SingletonManager.Instance.Ammo > 0)
            {
                shootCoolDown += Time.deltaTime;

                if(shootCoolDown > 0.2f)
                {
                    sound.Play();
                    anim.Play();
                    smoke.Play();
                    flash.Play();
                    StartCoroutine(LightIntensity());
                    ShootCheck();

                    shootCoolDown = 0f;
                }
            }
        }
        

        canShoot = false;
    }

    void ShootCheck()
    {
        if(Physics.Raycast(sound.gameObject.transform.position, sound.gameObject.transform.right, out RaycastHit hit, Mathf.Infinity))
        {
            if(hit.collider.CompareTag("Enemy"))
            {
                print("Hit!");
                hit.collider.GetComponent<AIController>().Health--;
            }
        }
    }
    #endregion

    IEnumerator Crouch(float input, float output)
    {
        if (isCrouching)
            isCrouching = false;
        else
            isCrouching = true;

        yield return controller.height = Mathf.Lerp(input, output, 1f * Time.deltaTime);
    }

    IEnumerator LightIntensity()
    {
        flashLight.intensity = 10f;
        yield return new WaitForSeconds(0.2f);
        flashLight.intensity = 0f;
    }
}
