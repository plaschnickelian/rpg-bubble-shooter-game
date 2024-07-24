using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game1Spawner : MonoBehaviour
{
    public GameObject battleControllerObject;
    private Game1BattleController battleController;
    public GameObject activityManager;

    public GameObject hero;
    public GameObject enemyType1;

    public int wave = 0;
    public int ofWaves;
    public GameObject waveTextObject;
    private TextMeshProUGUI waveText;

    private GameObject heroInstance;
    private GameObject enemy1Instance;
    private GameObject enemy2Instance;
    private GameObject enemy3Instance;
    public List<GameObject> enemyInstances;

    private Vector2 heroPosition;
    private Vector2 enemy1Position;
    private Vector2 enemy2Position;
    private Vector2 enemy3Position;
    private Vector2[] enemyPositions;

    public AudioSource mySounds;
    public AudioClip warningAppearing;
    public AudioClip enemiesSpawning;

    public void initialize()
    {
        ofWaves = GlobalVariables.waves;
        enemyInstances.Add(enemy1Instance);
        enemyInstances.Add(enemy2Instance);
        enemyInstances.Add(enemy3Instance);

        waveText = waveTextObject.GetComponent<TextMeshProUGUI>();
        battleController = battleControllerObject.GetComponent<Game1BattleController>();

        heroPosition = transform.position + new Vector3(-12f, -1f, 0f);
        enemy1Position = transform.position + new Vector3(0.5f, -2f, 0);
        enemy2Position = transform.position + new Vector3(10f, -2f, 0);
        enemy3Position = transform.position + new Vector3(19.5f, -2f, 0);

        enemyPositions = new Vector2[] { enemy1Position, enemy2Position, enemy3Position };

        start();
    }

    private void start()
    {
        heroInstance = Instantiate(hero, heroPosition, Quaternion.identity);
        heroInstance.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "SubBack";
        heroInstance.GetComponent<Hero>().activityManager = activityManager;

        battleController.heroObject = heroInstance;
        battleController.hero = heroInstance.GetComponent<Hero>();

        heroInstance.GetComponent<Hero>().battleController = battleController;

        StartCoroutine(createNewEnemySet(GlobalVariables.enemyNumber));
    }

    private void addEnemiesToController()
    {
        if (enemyInstances[0])
        {
            battleController.enemies[0] = enemyInstances[0];
            battleController.enemy1Object = enemyInstances[0];
            battleController.enemy1 = enemyInstances[0].GetComponent<Enemy>();
        }

        if (enemyInstances[1])
        {
            battleController.enemies[1] = enemyInstances[1];
            battleController.enemy2Object = enemyInstances[1];
            battleController.enemy2 = enemyInstances[1].GetComponent<Enemy>();
        }

        if (enemyInstances[2])
        {
            battleController.enemies[2] = enemyInstances[2];
            battleController.enemy3Object = enemyInstances[2];
            battleController.enemy3 = enemyInstances[2].GetComponent<Enemy>();
        }

    }

    public IEnumerator createNewEnemySet(int numberOfEnemies)
    {
        wave++;
        updateWaveText();

        for (int i = 0; i < numberOfEnemies; i++)
        {
            mySounds.PlayOneShot(enemiesSpawning);
            enemyInstances[i] = createNewEnemy(i);
            yield return new WaitForSeconds(0.5f);
        }

        addEnemiesToController();
    }

    public IEnumerator enableWarnings()
    {
        foreach (GameObject enemyObject in battleController.enemies)
        {
            if (enemyObject)
            {
                Enemy enemy = enemyObject.GetComponent<Enemy>();

                if (enemy.turnsToAction == 1)
                {
                    mySounds.PlayOneShot(warningAppearing);
                    enemy.attackWarning.SetActive(true);
                    enemy.attackWarning.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "SubBack";
                    enemy.attackWarning.transform.GetChild(1).GetComponent<SpriteRenderer>().sortingLayerName = "SubBack";
                    enemy.attackWarning.transform.GetChild(2).GetComponent<SpriteRenderer>().sortingLayerName = "SubBack";
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }

    private void updateWaveText()
    {
        waveText.text = $"{wave} / {ofWaves}";
    }

    private GameObject createNewEnemy(int number)
    {
        GameObject newEnemy = Instantiate(enemyType1, enemyPositions[number], Quaternion.identity);

        newEnemy.GetComponent<Enemy>().battleController = battleController;
        newEnemy.GetComponent<Enemy>().index = number;

        return newEnemy;
    }

    public GameObject getEnemy()
    {
        return enemy1Instance;
    }

    public GameObject getHero()
    {
        return heroInstance;
    }
}
