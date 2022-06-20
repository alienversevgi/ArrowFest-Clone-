using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public ArrowController SwordController;
    public ArrowDeckManager ArrowDeckManager;
    public LevelManager LevelManager;
    public PoolManager PoolManager;

    public TextMeshProUGUI text;

    public ChibiGroup group;
    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        float current = 0;
        current = Time.frameCount / Time.time;
        int avgFrameRate = (int)current;
        //Debug.Log(avgFrameRate.ToString() + " FPS");
        text.text = avgFrameRate.ToString() + " FPS";
    }

    public void StartGame()
    {
        PoolManager.Initialize();
        LevelManager.LoadLevel();
        SwordController.Initialize(LevelManager.PathCreator.path, LevelManager.CurrentLevelData.RoadData.roadMeshSettings.roadWidth);
        ArrowDeckManager.Initialize();
    }

    public void GameOver()
    {

    }
}
