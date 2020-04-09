using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending}
    State state = State.Alive;

    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 250f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip explodingSound;
    [SerializeField] AudioClip coolSound;

    // Start is called before the first frame update
    void Start()
    {
        if(state == State.Alive)
        {
            rigidBody = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        RespondToThrustInput();
        RespondToRotateInput();
    }

    void RespondToRotateInput()
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

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
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
                StartNextLevelSequence();
                break;
            default:
                //Die            
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(explodingSound);
        Invoke("LoadFirstLevel", 1f);
    }

    private void StartNextLevelSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(coolSound);
        Invoke("LoadNextScene", 1f);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
}
