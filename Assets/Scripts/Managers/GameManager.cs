﻿using UnityEngine;
using System;
using System.Linq;

using Object = UnityEngine.Object;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class GameManager : MonoBehaviour
{
    public float DeathFreezeTime = 3f;
    public PlayerBase       Player;
    public EnemySpawner     EnemiesSpawner;

    public GraphManager     GraphManagerPrefab;

    public bool     GameRunning { get; private set; }
    public int      DotsCount { get; private set; }
    public int      CurrentLevel { get; private set; }

    public GraphManager GraphManagerInstance { get { return graphManager; } }

    public event Action<bool>   GameEnded = delegate { };
    public event Action         GameStarted = delegate { };
    public event Action         LevelStarted = delegate { };
    public event Action         LevelFinished = delegate { };
    public event Action         LifeLost = delegate { };
    public event Action         PowerDotCollected = delegate { };

    private GraphManager graphManager;
    private LevelManager.LevelDefinition currentLevelDef;

    public static GameManager Instance;

    void Awake()
    {
        ScoreBall.BallSpawned += OnBallSpawned;
        ScoreBall.BallCollected += OnBallCollected;
        ScoreBall.BallCollected += OnPowerDotCollected;
        
        graphManager = Instantiate<GraphManager>(GraphManagerPrefab);
        Instance = this;
    }

    void Start()
    {
        Player.Movement.PlayerTargetReached += OnPlayerTargetReached;
        Player.Movement.PlayerTargetChanged += OnPlayerTargetChanged;
        Player.Movement.PlayerNextTargetChanged += OnPlayerNextTargetChanged;
        LevelManager.InitiateLevels(graphManager.transform.position);
        StartCoroutine(ApplyBackgroundHealthIndicator());
        AudioManager.Instance.TurnOnMusic();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Collecting a power dot");
            PowerDotCollected();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GuiManager.Instance.IsMenuOn)
            {
                EndGame(false);
                GuiManager.Instance.ShowMenu();
            }
            else
            {
                Application.Quit();
            }

        }
    }

    public void StartGame()
    {
        Player.Stats.ResetStats();
        CurrentLevel = 1;
        GameStarted();
        StartLevel(CurrentLevel);
    }

    public void StartLevel(int levelNo)
    {
        var levelDef = LevelManager.GetLevelDefinition(levelNo);
        currentLevelDef = levelDef;
        graphManager.Generate(levelDef.GraphType, levelDef.GraphSettings, levelDef.PowerDotLocations);
        SpawnPlayer();
        EnemiesSpawner.SpawnEnemies(levelDef.EnemiesCount, levelDef.EnemiesRespawnNodeIndexes);
        GameRunning = true;
        LevelStarted();
    }

    public void EndGame(bool success)
    {
        graphManager.DestroyGraph();
        EnemiesSpawner.ClearEnemies();
        DotsCount = 0;
        GameRunning = false;
        GameEnded(success);
    }

    public void LooseLife()
    {               
        LifeLost();
        StartCoroutine(LoseLifeCoroutine());
    }

    private IEnumerator LoseLifeCoroutine()
    {
        SetDeathEffects(true);
        yield return WaitRealTime(DeathFreezeTime);
        ClearNodesModifications();
        EnemiesSpawner.SpawnEnemies(currentLevelDef.EnemiesCount, currentLevelDef.EnemiesRespawnNodeIndexes);
        SpawnPlayer();
        SetDeathEffects(false);
    }

    private IEnumerator WaitRealTime(float time)
    {
        var t = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < t + time)
        {
            yield return null;
        }
    }

    private void SetDeathEffects(bool dead)
    {
        var effects = FindObjectsOfType<Grayscale>();
        foreach (var e in effects)
        {
            e.enabled = dead;
        }
        Time.timeScale = dead ? 0f : 1f;
    }

    private void ClearNodesModifications()
    {
        foreach (Node n in graphManager.Nodes)
        {
            n.Show();
            ChangeNodeColor(n, Color.white);
        }
    }

    private IEnumerator ApplyBackgroundHealthIndicator()
    {
        var cameras = Camera.main.GetComponentsInChildren<Camera>();
        var t = Time.time;        
        while(true)
        {                        
            foreach (var c in cameras)
            {
                var color = (1 - Player.Stats.Lifes * 1f / PlayerStats.MaxLifes) / 3 * Mathf.Sin((Time.time - t) * 2);                
                c.backgroundColor = new Color(GameRunning ? color : 0f, 0, 0);
            }
            yield return null;
        }        
    }

    private void FinishLevel()
    {
        LevelFinished();
        graphManager.DestroyGraph();
        CurrentLevel++;
        if (CurrentLevel > LevelManager.MaxLevelNo)
        {
            EndGame(true);
        }
        else
        {
            StartLevel(CurrentLevel);
        }
    }

    private void SpawnPlayer()
    {
        var node = graphManager.Nodes[LevelManager.GetLevelDefinition(CurrentLevel).PlayerRespawnNodeIndex];
        Player.Movement.Spawn(node);
        Player.transform.LookAt(graphManager.GraphCenter());
        var head = Player.transform.Find("Camera/Head");
        head.GetComponent<GvrHead>().trackRotation = false;
        head.localRotation = Quaternion.identity;
        head.GetComponent<GvrHead>().trackRotation = true;
        //// DEBUG
        //var debugthing = Instantiate(graphManager.NodeObjectPrefab);
        //debugthing.GetComponent<MeshRenderer>().material.color = Color.red;
        //debugthing.transform.position = graphManager.GraphCenter();
        //// DEBUG
    }

    private void OnBallSpawned(ScoreBall ball)
    {
        DotsCount += 1;
    }

    private void OnBallCollected(ScoreBall ball)
    {
        DotsCount -= 1;
        if (DotsCount == 0)
        {
            FinishLevel();
        }
    }

    private void OnPowerDotCollected(ScoreBall ball)
    {
        if (!ball.isPowerDot)
            return;
        PowerDotCollected();
    }

    private void OnPlayerTargetReached(PlayerLinearMovement player)
    {
        ChangeNeightboursColors(player.CurrentNode, Color.green);
        player.CurrentNode.Hide(true);
    }

    private void OnPlayerNextTargetChanged(PlayerLinearMovement player)
    {
        if (player.NextTargetNode)
        {
            ChangeNodeColor(player.NextTargetNode, Color.yellow);
        }
    }

    private void OnPlayerTargetChanged(PlayerLinearMovement player)
    {
        foreach(var n in graphManager.Nodes)
        {
            if (n != player.CurrentNode)
            {
                n.Show();
            }
        }   

        if (player.PreviousNode)
        {
            ChangeNeightboursColors(player.PreviousNode, Color.white);
        }
        if (player.TargetNode)
        {
            ChangeNeightboursColors(player.TargetNode, Color.green);
            player.TargetNode.Hide();
        }
    }

    private void ChangeNeightboursColors(Node node, Color color)
    {
        var neightbours = graphManager.GetNeightbours(node);
        foreach (var n in neightbours)
        {
            ChangeNodeColor(n, color);
        }
    }

    private void ChangeNodeColor(Node node, Color color)
    {
        node.GetComponent<Renderer>().material.color = color;
    }
}
