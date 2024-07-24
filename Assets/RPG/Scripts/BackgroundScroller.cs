using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Range(-1f,1f)]
    public float scrollSpeed = 1f;
    private float offset;
    private Material mat;
    private bool activated;

    void Awake()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        offset += (Time.deltaTime * scrollSpeed) / 5f;
        mat.SetTextureOffset("_MainTex", new Vector2(0, offset));
    }

    public void activate() {
        gameObject.SetActive(true);
        StartCoroutine(fadeIn(1.5f,0.6f));
    }

    public void deactivate() {
        /* StartCoroutine(fadeOut(1f,0f)); */
        gameObject.SetActive(false);
    }

    private IEnumerator fadeIn(float duration, float maxVal) {
        float time = 0;
        float opacity = 0f;
        while (time < duration)
        {
            opacity = Mathf.Lerp(0f, maxVal, time / duration);
            time += Time.deltaTime;

            Color tempCol = mat.color;
            tempCol.a = opacity;
            mat.color = tempCol;

            /* foreach(GameObject o in objs) {
                Color tempCol = o.GetComponent<SpriteRenderer>().color;
                tempCol.a = opacity;
                o.GetComponent<SpriteRenderer>().color = tempCol;
            } */
            yield return null;
        }
    }

    private IEnumerator fadeOut(float duration, float minVal) {
        float time = 0;
        float opacity = 0f;
        while (time < duration)
        {
            opacity = Mathf.Lerp(0.6f, minVal, time / duration);
            time += Time.deltaTime;

            Color tempCol = mat.color;
            tempCol.a = opacity;
            mat.color = tempCol;

            /* foreach(GameObject o in objs) {
                Color tempCol = o.GetComponent<SpriteRenderer>().color;
                tempCol.a = opacity;
                o.GetComponent<SpriteRenderer>().color = tempCol;
            } */
            yield return new WaitForSeconds(duration);
        }
    }
}
