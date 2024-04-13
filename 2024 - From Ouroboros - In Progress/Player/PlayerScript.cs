using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody rb;

    public float groundDist = 0.6f;
    public int jumpVal = 5;
    public int moveMag = 10;

    private bool jump;
    private Vector3 moveDir;
    private bool rotating;
    private bool held = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        EventModule.i.onGravityChange += RightPlayer;
    }

    private void Update()
    {
        if (!rotating)
        {
            RotWithCam();
        }
    }

    void FixedUpdate()
    {
        // jump code
        if (jump)
        {
            rb.AddForce(rb.transform.up * jumpVal, ForceMode.Impulse);
            jump = false;
        }

        // player move code
        // this SHOULD work with diff grav // as long as players y is up against grav
        if (!rotating)
        {
            rb.AddForce(
                transform.TransformDirection(moveDir) * Time.fixedDeltaTime * moveMag,
                ForceMode.Impulse);
        }

        // should cap move velocity // or should i?
    }

    void OnJump()
    {
        if (isGrounded())
        {
            jump = true;
        }
    }

    void OnMove(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        moveDir = new Vector3(dir.x, 0, dir.y);
    }

    void OnInteract()
    {
        // if player was holding, don't interact
        if (held == true){
            held = false;
            return;
        }

        // regular interaction
        RaycastHit hit;
        Transform camTran = Camera.main.transform;

        if (Physics.Raycast(camTran.position, camTran.forward, out hit, 10f))
        {
            hit.collider.SendMessage("Interact", SendMessageOptions.DontRequireReceiver);
            Debug.Log(hit.collider.name);
        }

    }

    void OnHold()
    {
        held = true;
    }

    void OnTake()
    {

    }

    void OnTUp()
    {
        EventModule.i.OnTUp();
    }

    void OnTDown()
    {
        EventModule.i.OnTDown();
    }

    private bool isGrounded()
    {
        RaycastHit hit;
        Vector3 dir = rb.transform.up * -1;

        if (Physics.Raycast(transform.position, dir, out hit, groundDist))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // rotate player with camera // y axis only
    private void RotWithCam()
    {
        transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            Camera.main.transform.localEulerAngles.y,
            transform.localEulerAngles.z);
    }

    // allows player to rotate with grav
    private void RightPlayer()
    {
        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        rotating = true;
        Quaternion to = Quaternion.FromToRotation(transform.up, -Physics.gravity.normalized);
        Quaternion from = transform.rotation;
        float timeCount = 0f;
        float rate = 0.8f;

        while (transform.rotation != to)
        {
            transform.rotation = Quaternion.Slerp(from, to, timeCount);
            timeCount += Time.fixedDeltaTime * rate;

            yield return new WaitForFixedUpdate();
        }
        rotating = false;
    }
}