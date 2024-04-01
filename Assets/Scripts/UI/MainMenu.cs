using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private Button newGameBtn;
    private Button continueBtn;
    private Button quitBtn;

    private void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueBtn=transform.GetChild(2).GetComponent<Button>();
        quitBtn=transform.GetChild(3).GetComponent<Button>();
        
        newGameBtn.onClick.AddListener(NewGame);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);
    }

    void NewGame()
    {
        PlayerPrefs.DeleteAll();
        //转换场景
        SceneController.Instance.TransitionToFirstLevel();
    }
    
    void ContinueGame()
    {
        //转换场景，读取进度
        SceneController.Instance.TransitionToLoadGame();
    }
    
    void QuitGame()
    {
        Application.Quit();
    }
}
