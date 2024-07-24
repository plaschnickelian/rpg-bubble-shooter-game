using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCounter : MonoBehaviour
{
    private int turnsLeft;
    public int amountOfTurns;
    public GameObject gameManager;
    private GameManager gameManagerComponent;
    public GameObject[] rectangles;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerComponent = gameManager.GetComponent<GameManager>();
        amountOfTurns = 3;
        turnsLeft = amountOfTurns;
    }

    // Update is called once per frame
    void Update()
    {
        turnsLeft = gameManagerComponent.shootCount - gameManagerComponent.shootAmount;

        switch (turnsLeft)
        {
            case 0:
                rectangles[0].SetActive(false);
                rectangles[1].SetActive(false);
                rectangles[2].SetActive(false);
                break;
            case 1:
                rectangles[0].SetActive(true);
                rectangles[1].SetActive(false);
                rectangles[2].SetActive(false);
                break;
            case 2:
                rectangles[0].SetActive(true);
                rectangles[1].SetActive(true);
                rectangles[2].SetActive(false);
                break;
            case 3:
                rectangles[0].SetActive(true);
                rectangles[1].SetActive(true);
                rectangles[2].SetActive(true);
                break;
        }
    }
}
