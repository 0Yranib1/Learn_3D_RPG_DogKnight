using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    public GameObject playerPrefab;
    private GameObject player;
    private NavMeshAgent playerAgent;
    public SceneFader sceneFaderPrefab;
    private bool fadeFinished;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case (TransitionPoint.TransitionType.SameScene):
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case (TransitionPoint.TransitionType.DifferentScene):
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        SaveManager.Instance.SavePlayerData();
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return StartCoroutine(fade.FadeOut(2f));
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position,
                GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();
            yield return StartCoroutine(fade.FadeIn(2f));
            yield break;
        }else
        {
            player=GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position,GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;
            yield return null;
        }
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();
        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
                return entrances[i];
        }
        return null;
    }

    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("SampleScene"));
    }
    
    IEnumerator LoadLevel(String scene)
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        if (scene != "")
        {
            yield return StartCoroutine(fade.FadeOut(2f));
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position,
                GameManager.Instance.GetEntrance().rotation);
            //保存数据
            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fade.FadeIn(2f));
            yield break;
        }
    }

    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    IEnumerator LoadMain()
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(3f));
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fade.FadeIn(3f));
        yield break;
    }

    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    public void EndNotify()
    {
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());
        }
        
    }
}
