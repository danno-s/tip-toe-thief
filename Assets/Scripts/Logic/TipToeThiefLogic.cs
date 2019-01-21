using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TipToeThiefLogic : MonoBehaviour, TipToeThiefEntity
{

    public ChinchillaLogic player;
    public GameObject maskLayer;
    public Camera gameCamera;
    public float alphaRate;
    public List<GameObject> stages;
    [HideInInspector]
    public int remainingLives;

    private SpriteRenderer layerSprite;
    private Coroutine currentCoroutine;
    private bool lightsOn;
    private int currentStage;
    private Color zero = new Color(0, 0, 0, 0);

    // Use this for initialization
    void Start()
    {
        layerSprite = maskLayer.GetComponent<SpriteRenderer>();
        lightsOn = true;
        currentStage = 0;
    }

    /***
     *  Toggles the state of the lights. If the lights turned off or on,
     *  then a black filter is applied on the game, to hide its entities.
     **/
    public void ToggleLights()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        if (lightsOn)
            currentCoroutine = StartCoroutine("BlockCamera");
        else
            currentCoroutine = StartCoroutine("UnBlockCamera");
    }

    public void RestartLevel(TipToeThiefEntity caller, bool loseLife = true)
    {
        Debug.LogFormat("Restarting to level {0}", currentStage);

        foreach (var tipToeThiefObject in
            stages[currentStage].GetComponentsInChildren<TipToeThiefResettableObject>())
        {
            tipToeThiefObject.Reset();
        }

        player.Reset();

        if (loseLife)
            remainingLives--;

        CheckGameOver(caller);
    }

    private void CheckGameOver(TipToeThiefEntity caller)
    {
        if (remainingLives > 0)
            return;

        // Make a transition object, to store the cause of gameover
        GameObject screenTransition = new GameObject("Scene Transition Object");
        screenTransition.AddComponent<TipToeThiefSceneTransition>();

        var transition = screenTransition.GetComponent<TipToeThiefSceneTransition>();

        if (caller.IsCat())
            transition.gameOverCause = GameOverCause.CaughtByCat;
        else if (caller.IsOwl())
            transition.gameOverCause = GameOverCause.CaughtByOwl;

        // Last scene is gameOver screen
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }

    internal void NextLevel()
    {
        currentStage = currentStage == stages.Count - 1 ? currentStage : currentStage + 1;
    }

    private IEnumerator BlockCamera()
    {
        while (1f - layerSprite.color.a > 0.001f)
        {
            layerSprite.color = Color.Lerp(layerSprite.color, Color.black, alphaRate * Time.deltaTime);

            yield return null;
        }

        lightsOn = false;
    }

    private IEnumerator UnBlockCamera()
    {
        while (layerSprite.color.a > 0.001f)
        {
            layerSprite.color = Color.Lerp(layerSprite.color, zero, alphaRate * Time.deltaTime);

            yield return null;
        }

        lightsOn = true;
    }

    internal void SetStage(int debugStage)
    {
        if (debugStage >= 0 && debugStage < stages.Count)
            currentStage = debugStage;
        RestartLevel(this, false);
    }

    internal void SetLives(int lives)
    {
        remainingLives = lives;
    }


    public bool IsLightOn()
    {
        return lightsOn;
    }

    public bool IsCat()
    {
        return false;
    }

    public bool IsOwl()
    {
        return false;
    }
}
