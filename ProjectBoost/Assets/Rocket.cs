using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource thrusterSound;

    enum State { Alive, Dying, Transcending}
    State state = State.Alive;

    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 250f;

    // Start is called before the first frame update
    void Start()
    {
        if(state == State.Alive)
        {
            rigidBody = GetComponent<Rigidbody>();
            thrusterSound = GetComponent<AudioSource>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Thrust();
        Rotate();
    }

    void Rotate()
    {
        rigidBody.freezeRotation = true;

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust);
            if (!thrusterSound.isPlaying)
            {
                thrusterSound.Play();
            }
        }
        else
        {
            thrusterSound.Stop();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) {return;}

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //Do nothing
                break;
            case "Finish":
                state = State.Transcending;
                Invoke("LoadNextScene", 1f);       
                break;
            default:
                //Die
                state = State.Dying;
                Invoke("LoadFirstLevel", 1f);
                break;
        }
    }

    private static void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
}
