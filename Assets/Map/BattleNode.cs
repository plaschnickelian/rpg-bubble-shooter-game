using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class BattleNode : Node
{

    public int numberOfEnemies;
    public int numberOfWaves;

    public GameObject mainCamera;

    public Animator disableEverything;

    protected override IEnumerator Event()
    {
        disableEverything.SetTrigger("FadeIn");
        yield return new WaitForSeconds(2f);

        yield return new WaitUntil(() => mapManager.GetComponent<MapManager>().move == false);

        mainCamera.GetComponent<AudioListener>().enabled = false;
        GlobalVariables.enemyNumber = numberOfEnemies;
        GlobalVariables.waves = numberOfWaves;
        AsyncOperation async = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);

        yield return new WaitUntil(() => async.isDone == true);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Main"));

        yield return null;
    }
}
