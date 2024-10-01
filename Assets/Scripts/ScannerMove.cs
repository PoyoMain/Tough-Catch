using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerMove : MonoBehaviour
{
    Rigidbody rb;
    float horizontalInput;
    float verticalInput;

    [SerializeField] float speed;
    [Header("Bounds")]
    [SerializeField] float xMax;
    [SerializeField] float xMin;
    [SerializeField] float yMax;
    [SerializeField] float yMin;

    public bool moving;
    bool paused;

    Vector3 move;

    private static ScannerMove instance;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        instance = this;

        paused = false;
    }

    
    void Update()
    {
        if (paused) return;

        //Handle Input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        

        //Update moving bool when there is input
        if (Mathf.Abs(verticalInput) > 0 || Mathf.Abs(horizontalInput) > 0)
        {
            moving = true;
        }
        else moving = false;

        //Check bounds
        Vector3 pos = transform.position;

        if (pos.y > yMax) pos.y = yMax;
        if (pos.y < yMin) pos.y = yMin;
        if (pos.x > xMax) pos.x = xMax;
        if (pos.x < xMin) pos.x = xMin;

        transform.position = pos;
    }

    //Physics handled in fixed update
    private void FixedUpdate()
    {
        if (paused) return;

        move = Vector3.zero;

        //Create vector from input
        move.x = horizontalInput;
        move.y = verticalInput;

        //Normalize the vector to get direction and multiply by speed to get velocity
        rb.velocity = move.normalized * speed;
    }


    /// <summary>
    /// Pauses the movement of the scanner
    /// </summary>
    public void Pause()
    {
        rb.velocity = Vector3.zero;
        paused = true;
    }

    /// <summary>
    /// Unpauses the movement of the scanner
    /// </summary>
    public void Unpause()
    {
        paused = false;
    }

    /// <summary>
    /// Public static method to retrieve the scanner singleton
    /// </summary>
    /// <returns></returns>
    public static ScannerMove GET_SCANNER()
    {
        return instance;
    }
}
