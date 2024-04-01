using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStatus playerStats;
    private CinemachineFreeLook followCamera;

    private List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
    public void RigisterPlayer(CharacterStatus player)
    {
        playerStats = player;

        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if (followCamera != null)
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }

    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            
            if (item.destinationTag == TransitionDestination.DestinationTag.MainMenu)
            {
                return item.transform;
            }
        }

        return null;
    }
}
