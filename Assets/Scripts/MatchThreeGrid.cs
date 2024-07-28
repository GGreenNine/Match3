using System.Collections;
using System.Collections.Generic;
using BoardActions;
using Data;
using Exceptions;
using Factories;
using Game;
using GameState;
using SwapM3;
using UnityEngine;
using Zenject;

public class MatchThreeGrid : MonoBehaviour
{
    private State InitialState;
    private Dictionary<TileData, TileControl> _tileControls = new();
    private readonly List<TileData> _selectedTiles = new();
    private BoardDefaultSwapM3Executor _matchThreeExecutor;

    private GridFactory _gridFactory;
    private StateFactory _stateFactory;
    private IGameStateController _gameStateController;
    private IGameScore _gameScore;
    private GameGlobalTimer _globalTimer;
        
    [Inject]
    private void SetDependencies(GridFactory gridFactory, StateFactory stateFactory, IGameScore gameScore, IGameStateController gameStateController, GameGlobalTimer globalTimer)
    {
        _gridFactory = gridFactory;
        _stateFactory = stateFactory;
        _gameStateController = gameStateController;
        _matchThreeExecutor = new BoardDefaultSwapM3Executor(gameScore);
        _gameScore = gameScore;
        _globalTimer = globalTimer;
    }

    private void Awake()
    {
        _gameStateController.OnStateUpdated += OnStateUpdated;
    }

    private void OnStateUpdated(IGameState newState)
    {
        if (newState._gameStateType == GameStateType.Gameplay)
        {
            InitialState = _stateFactory.Create(new ColorsMapRandomGeneration());
            _tileControls = _gridFactory.Create(InitialState, transform, TileClickHandler, TileDraggHandler);
        }

        if (newState._gameStateType == GameStateType.ResultScreen)
        {
            ClearBoard();
        }
    }

    private void ClearBoard()
    {
        InitialState = default;
        foreach (var tileControl in _tileControls)
        {
            Destroy(tileControl.Value.gameObject);
        }
        _tileControls.Clear();
        _selectedTiles.Clear();
        _matchThreeExecutor = new BoardDefaultSwapM3Executor(_gameScore);
    }
        
    private void TileClickHandler(TileControl tileControl)
    {
        _selectedTiles.Add(tileControl.TileData);
        if (_selectedTiles.Count == 2)
        {
            TrySwapTiles();
        }
    }

    private void TrySwapTiles()
    {
        try
        {
            var boardActions = _matchThreeExecutor.SwapExecute(InitialState, _selectedTiles);
            StartCoroutine(UpdateBoardCoroutine(boardActions));
        }
        catch (SwapIsNotLegalException e)
        {
            Debug.Log(e);
        }
        finally
        {
            _selectedTiles.Clear();
        }
    }

    private void TileDraggHandler(TileControl draggedTile, Vector3 dragPosition)
    {
        TileControl closestTile = null;
        float closestDistance = float.MaxValue;

        foreach (var tileControl in _tileControls.Values)
        {
            if (tileControl == draggedTile) continue;
            float distance = Vector3.Distance(dragPosition, tileControl.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = tileControl;
            }
        }

        if (closestTile != null && closestDistance < 2f) 
        {
            _selectedTiles.Clear();
            _selectedTiles.Add(draggedTile.TileData);
            _selectedTiles.Add(closestTile.TileData);
            TrySwapTiles();
        }
    }

    private void UpdateBoard(State state)
    {
        foreach (var tile in state.Board)
        {
            _tileControls.TryGetValue(tile, out var control);
            var tileColorIndex = state.ColorsMap[tile.X, tile.Y];
            var color = ColorsMapDefinitions.GetColor(tileColorIndex);
            if (control != null)
            {
                control.SetColor(color);
            }
        }
    }

    private IEnumerator UpdateBoardCoroutine(IEnumerable<IBoardAction> boardActions)
    {
        _globalTimer.StopTime();
        foreach (var action in boardActions)
        {
            UpdateBoard(action.ModifiedState);
            yield return new WaitForSeconds(1);
        }

        UpdateScore();
        _globalTimer.Continue();
    }


    private void UpdateScore()
    {
        _gameScore.AddScore(_gameScore.ScoreTempReactiveProperty.Value);
    }
}