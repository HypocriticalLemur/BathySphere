using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLightBlinker : MonoBehaviour
{
    Animator animator;
    bool animated => animator != null;
    const float MIN_IDLE_TIME_SECONDS = 10f;
    const float MAX_IDLE_TIME_SECONDS = 20f;
    public bool _blinks = false;
    public float _idle_time = 0f;
    float idleClipLength = 0f;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        var clips = animator.runtimeAnimatorController.animationClips;
        /*
        foreach (var clip in clips)
        {
            if (clip.name == "Idle")
                idleClipLength = clip.length; break;
        }*/

        idleClipLength = animator.GetCurrentAnimatorStateInfo(1).length / animator.GetCurrentAnimatorStateInfo(1).speed;
        Debug.Log($"Idle clip length is {idleClipLength}");
    }
    void Start()
    {
        StartCoroutine(Waiter());
    }

    IEnumerator Waiter()
    {
        while (animated)
            yield return Randomiser();
    }
    IEnumerator Randomiser()
    {
        float random = Random.Range(MIN_IDLE_TIME_SECONDS, MAX_IDLE_TIME_SECONDS);
        _idle_time = random;
        //animator.SetBool(name = "isBlinking", true);
        _blinks = true;
        Debug.Log("Blink!!!");
        //yield return new WaitForSeconds(idleClipLength);
        animator.Play("LightBlinkGentle");
        //animator.SetBool(name = "isBlinking", false);
        _blinks = false;
        yield return new WaitForSeconds(random);
    }
}
