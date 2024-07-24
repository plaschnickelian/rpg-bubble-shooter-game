using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(fadeIn(this.gameObject, 1f, 0.8f));
    }

    private IEnumerator fadeIn(GameObject o, float duration, float maxVal) {
        float time = 0;
        float opacity = 0f;
        while (time < duration)
        {
            opacity = Mathf.Lerp(0f, maxVal, time / duration);
            time += Time.deltaTime;

            Color tempCol = o.GetComponent<SpriteRenderer>().color;
            tempCol.a = opacity;
            o.GetComponent<SpriteRenderer>().color = tempCol;

            /* foreach(GameObject o in objs) {
                Color tempCol = o.GetComponent<SpriteRenderer>().color;
                tempCol.a = opacity;
                o.GetComponent<SpriteRenderer>().color = tempCol;
            } */
            yield return null;
        }
    }
}
