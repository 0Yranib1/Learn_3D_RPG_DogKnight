using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class SaveManager : Singleton<SaveManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerData();
        }
    }

    public void SavePlayerData()
    {
        Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }
    
    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }
    
    public void Save(Object data,string key)
    {
        //TODO:检查是否有办法指定存档位置 当前为默认位置
        var jsonData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key,jsonData);
        PlayerPrefs.Save();
    }

    public void Load(Object data,string key)
    {
      if(PlayerPrefs.HasKey(key))
      {
          JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key),data);
          
      }
    }
}
