using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FootstepSound : MonoBehaviour
{
    public FootstepSounds rockSounds;
    private AudioSource audioSource;

    public float walkPeriod = 0.5f;
    public float runPeriod = 0.3f;
    public float crouchPeriod = 0.1f;

    private TerrainSetting ts;

    public bool isMoving, isRunning, isCrouching, isJumping; 

    private List<AudioClip> sounds;

    private float period;

    private bool playing = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sounds = new List<AudioClip>();
        period = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if (playing) return;

        if (other.gameObject.CompareTag("Tile"))
        {
            //Debug.Log("타일에 닿았다!");
            ts = other.gameObject.GetComponent<TerrainSetting>();
            if (isMoving)
            {
                if (isRunning)
                {
                    sounds = new List<AudioClip>(ts.terrainData.Run);
                    period = runPeriod;
                }
                else
                {
                    sounds = new List<AudioClip>(ts.terrainData.Walk);
                    period = walkPeriod;
                }

                if (isCrouching)
                {
                    period = crouchPeriod;
                }
                StartCoroutine(Footsteps(period));
            }
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            if (isMoving)
            {
                if (isRunning)
                {
                    sounds = new List<AudioClip>(rockSounds.Run);
                    period = runPeriod;
                }
                else
                {
                    sounds = new List<AudioClip>(rockSounds.Walk);
                    period = walkPeriod;
                }

                if (isCrouching)
                {
                    period = crouchPeriod;
                }
                StartCoroutine(Footsteps(period));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tile"))
        {
            if (isJumping && ts.terrainData.JumpLand != null)
            {
                sounds = new List<AudioClip>(ts.terrainData.JumpLand);
                StopAllCoroutines();
                StartCoroutine(Footsteps(period));
                isJumping = false;
            }
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            if (isJumping && ts.terrainData.JumpLand != null)
            {
                sounds = new List<AudioClip>(rockSounds.Land);
                StopAllCoroutines();
                StartCoroutine(Footsteps(period));
                isJumping = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Tile"))
        {
            if (isJumping && ts.terrainData.JumpStart != null)
            {
                sounds = new List<AudioClip>(ts.terrainData.JumpStart);
                StopAllCoroutines();
                StartCoroutine(Footsteps(period));
            }
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            if (isJumping && ts.terrainData.JumpLand != null)
            {
                sounds = new List<AudioClip>(rockSounds.Jump);
                StopAllCoroutines();
                StartCoroutine(Footsteps(period));
                isJumping = false;
            }
        }
    }
    IEnumerator Footsteps(float period)
    {
        playing = true;

        if (isMoving)
        {
            audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Count)]);
            yield return Util.WaitForSecond(period);
        }
        sounds.Clear();

        playing = false;
    }
}
