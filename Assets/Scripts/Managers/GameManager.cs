using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStatus playerStats;

    public void RigisterPlayer(CharacterStatus player)
    {
        playerStats = player;
    }
}
