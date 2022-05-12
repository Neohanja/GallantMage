using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : Movement
{
    public bool inverseLookUp = true;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 50f;
    bool fpsCam;
    GameObject thirdPersonCam;
    GameObject firstPersonCam;
    Quaternion prevRotation;
    MeshRenderer model;
    protected int partIndex = 0;
    float rotX;

    [Header("Resources")]
    public int woodCount = 0;

    protected override void Initialize()
    {
        base.Initialize();
        rotX = 0f;
        model = GetComponentInChildren<MeshRenderer>();

        if (CamControl.MainCam != null)
            thirdPersonCam = CamControl.MainCam.gameObject;
        firstPersonCam = GetComponentInChildren<Camera>().gameObject;

        if(firstPersonCam != null && !Application.isEditor)
        {
            thirdPersonCam.SetActive(false);
            fpsCam = true;
            prevRotation = transform.rotation;
            model.enabled = false;
            if (UIManager.ActiveUI != null)
                UIManager.ActiveUI.MouseActive(false);
        }
        else
        {
            if (firstPersonCam != null)
                firstPersonCam.SetActive(false);
            fpsCam = false;
            if (UIManager.ActiveUI != null)
                UIManager.ActiveUI.MouseActive(true);
        }
    }

    protected override void GetMovement()
    {

        float side = Input.GetAxis("Horizontal");
        float forward = Input.GetAxis("Vertical");

        if(Input.GetKeyDown(KeyCode.F3) && Application.isEditor)
        {
            if(fpsCam)
            {
                if (thirdPersonCam != null)
                {
                    fpsCam = false;
                    thirdPersonCam.SetActive(true);
                    firstPersonCam.SetActive(false);
                    prevRotation = transform.rotation;
                    transform.rotation = Quaternion.identity; 
                    CamControl.MainCam.SetTarget(gameObject);
                    model.enabled = true;
                    if (UIManager.ActiveUI != null)
                        UIManager.ActiveUI.MouseActive(true);
                }
            }
            else
            {
                if(firstPersonCam != null)
                {
                    fpsCam = true; 
                    thirdPersonCam.SetActive(false);
                    firstPersonCam.SetActive(true);
                    transform.rotation = prevRotation;
                    model.enabled = false;
                    if (UIManager.ActiveUI != null)
                        UIManager.ActiveUI.MouseActive(false);
                }
            }
        }

        if(fpsCam)
        {
            float mouseSide = Input.GetAxis("Mouse X") * mouseSensitivity;
            rotX += Input.GetAxisRaw("Mouse Y") * (inverseLookUp ? -1 : 1) * mouseSensitivity;
            rotX = Mathf.Clamp(rotX, -maxLookAngle, maxLookAngle);
            transform.Rotate(new Vector3(0, mouseSide, 0));
            firstPersonCam.transform.localRotation = Quaternion.identity;
            firstPersonCam.transform.Rotate(new Vector3(rotX, 0f, 0f));
        }

        if(Input.GetMouseButtonDown(0))
        {
            if (meleeRange && meleeTarget != null)
            {
                int given = meleeTarget.GetComponent<ClutterData>().GiveResources(3, transform);
                woodCount += given;
                if (given < 3)
                {
                    MeleeRange(null);
                }
            }
            else meleeRange = false;
        }

        if (Input.GetKey(KeyCode.LeftShift)) running = true;
        else running = false;

        momentum = transform.forward * forward + transform.right * side;
    }
}
