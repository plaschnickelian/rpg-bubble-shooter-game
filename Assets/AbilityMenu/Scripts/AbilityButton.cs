using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityButton : MonoBehaviour
{
    public Sprite btnUp;
    public Sprite btnDown;
    public UnityEvent buttonClick;
    public AudioSource mySounds;
    public AudioClip clickSound;

    void Awake() {
        if(buttonClick == null) {
            buttonClick = new UnityEvent();
        }
    }

    void OnMouseDown() {
        GetComponent<SpriteRenderer>().sprite = btnDown;
        mySounds.PlayOneShot(clickSound);
    }

    void OnMouseUp() {
        buttonClick.Invoke();
        GetComponent<SpriteRenderer>().sprite = btnUp;
    }
}
