using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyToStartButton : MonoBehaviour
{
    public GameObject activityManager;
    public Animator animator;

    private ActivityManager activityScript;

    // Start is called before the first frame update
    void Start()
    {
        activityScript = activityManager.GetComponent<ActivityManager>();
    }

    private void OnMouseOver() 
    {
        if (Input.GetMouseButton(0)) 
        {
            StartCoroutine(activityScript.initializeStartGameloop());
            /* this.gameObject.GetComponent<BoxCollider2D>().enabled = false; */
            this.gameObject.SetActive(false);
        }
    }

    void OnMouseEnter() {
        this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            animator.SetTrigger("FadeIn");
    }

    void OnMouseExit() {
        animator.SetTrigger("FadeOut");
    }
}
