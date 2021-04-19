using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using System.Windows.Input;

public class ShipController : MonoBehaviour
{

    float rotationSpeed = 100.0f;
    float thrustForce = 3f;

    public AudioClip crash;
    public AudioClip shoot;

    public GameObject bullet;


    private GameController gameController;

    private FMOD.Studio.EventInstance instance;
    private string paramName="Thrust";
    public float value = 0.0f;



void Start()
    {
        // Get a reference to the game controller object and the script
        GameObject gameControllerObject =
            GameObject.FindWithTag("GameController");

        gameController =
            gameControllerObject.GetComponent<GameController>();


        // Using the event path set in Unity editor
        instance = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Thruster");
    }

    private void Update()
    {
        // Has a bullet been fired
        if (Input.GetKeyDown("space"))
            ShootBullet();

        if (Input.GetKeyDown(KeyCode.W))
        {
            instance.start();

        }

        if (Input.GetKey(KeyCode.W))
        {
            value += Time.deltaTime;
            Debug.Log(value);
            if (value > 5.0f)
            {
                value = 5.0f;
                instance.setParameterByName(paramName, value);
                
            }
            
        }
        
        if(Input.GetKeyUp(KeyCode.W))
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            value = 0.0f;
        }
    }

    void FixedUpdate()
    {

        // Rotate the ship if necessary
        transform.Rotate(0, 0, -Input.GetAxis("Horizontal") *
            rotationSpeed * Time.deltaTime);

        // Thrust the ship if necessary
        GetComponent<Rigidbody2D>().
            AddForce(transform.up * thrustForce *
                Input.GetAxis("Vertical"));
       



    }

    void OnTriggerEnter2D(Collider2D c)
    {

        // Anything except a bullet is an asteroid
        if (c.gameObject.tag != "Bullet")
        {

            //                                          AudioSource.PlayClipAtPoint
            //                                              (crash, Camera.main.transform.position);

            // Move the ship to the centre of the screen
            transform.position = new Vector3(0, 0, 0);

            // Remove all velocity from the ship
            GetComponent<Rigidbody2D>().
                velocity = new Vector3(0, 0, 0);
            value = 0.0f;

            gameController.DecrementLives();
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Explode");
        }
    }

    void ShootBullet()
    {

        // Spawn a bullet
        Instantiate(bullet,
            new Vector3(transform.position.x, transform.position.y, 0),
            transform.rotation);

        // Play a shoot sound
        //                              AudioSource.PlayClipAtPoint(shoot, Camera.main.transform.position);
    }
}
