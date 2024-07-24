using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    public GameObject player;
    public List<GameObject> nodes;
    private int currentNode;

    public bool move;
    public float speed = 5f;
    private Vector3 destination;

    void Start()
    {
        currentNode = 0;
        nodes[1].GetComponent<CircleCollider2D>().enabled = true;
        setPlayerPosition(0);
    }

    void Update() {
        if(move) {
            player.transform.position = Vector3.MoveTowards(player.transform.position, destination, speed * Time.deltaTime);
            if(player.transform.position == destination) {
                move = false;
            }
        }
    }

    public void setPlayerPosition(int index) {
        player.transform.position = nodes[index].transform.position;
    }

    public void movePlayerToNode(GameObject node) {
        currentNode = nodes.IndexOf(node);
        if(currentNode != nodes.Count - 1) {
            nodes[currentNode + 1].GetComponent<CircleCollider2D>().enabled = true;
        }
        nodes[currentNode].GetComponent<CircleCollider2D>().enabled = false;

        destination = node.transform.position;
        move = true;
    }
}
