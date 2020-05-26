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
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip explodingSound;
    [SerializeField] AudioClip coolSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem explodingSoundParticles;
    [SerializeField] ParticleSystem coolSoundParticles;

    bool isCollisionDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
            rigidBody = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild)
        {
            RespondDebugKeys();
        }  
    }

    private void RespondDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartNextLevelSequence();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            isCollisionDisabled = !isCollisionDisabled;
        }
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
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust *Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !isCollisionDisabled) {return;}

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
        explodingSoundParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void StartNextLevelSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(coolSound);
        coolSoundParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
         
        SceneManager.LoadScene(nextSceneIndex);
    }
}
