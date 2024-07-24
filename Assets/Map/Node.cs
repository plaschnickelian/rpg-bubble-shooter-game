using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Node : MonoBehaviour
{

    public GameObject mapManager;

    void Start()
    {
        
    }

    void OnMouseUp() {
        mapManager.GetComponent<MapManager>().movePlayerToNode(gameObject);

        StartCoroutine(Event());
    }

    protected virtual IEnumerator Event() {
        yield return null;
    }
}
