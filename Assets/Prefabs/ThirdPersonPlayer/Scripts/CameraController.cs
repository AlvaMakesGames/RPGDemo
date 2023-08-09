using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Player { get; set; }
    private float mouseX, mouseY;
    private bool controllerDetected;
    [SerializeField] private float rAnalogSens = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        controllerDetected = DetectController();        

        if (controllerDetected)
        {
            mouseX -= Input.GetAxis("RightHorizontal") * rAnalogSens;
            mouseY -= Input.GetAxis("RightVertical") * rAnalogSens;
        }
        else
        {
            mouseX += Input.GetAxis("Mouse X");
            mouseY -= Input.GetAxis("Mouse Y");
        }

        mouseY = Mathf.Clamp(mouseY, -60f, 60f);

        transform.rotation = Quaternion.Euler(mouseY, mouseX, 0f);
    }

    private bool DetectController()
    {
        if (Input.GetJoystickNames().Length != 0)
            return true;
        else
            return false;
    }

    private void LateUpdate()
    {
        transform.position = Player.position;
    }
}
