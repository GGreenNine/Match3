using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoardActions;
using Data;
using Dejkstra;
using Exceptions;
using Factories;
using Game;
using GameState;
using Loaders;
using QFSW.QC;
using SwapM3;
using UnityEngine;
using Zenject;

public class MatchThreeGrid : MonoBehaviour
{
    [SerializeField] private GameObject _interactionBlockScreen;

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
        _matchThreeExecutor = new BoardDefaultSwapM3Executor();
        _gameScore = gameScore;
        _globalTimer = globalTimer;
    }

    [Command]
    public void Solve(int targetPoints)
    {
        var dejkstraSolver = new DejkstraSolver(_gameScore);
        var solvedState = dejkstraSolver.Solve(in InitialState, targetPoints, out var actionsTakenStack);
        StartCoroutine(UpdateBoardCoroutine(actionsTakenStack));
    }

    [Command]
    private void SaveState()
    {
        StateSaveLoader stateSaveLoader = new();
        stateSaveLoader.SaveState(in InitialState);
    }

    [Command]
    private void LoadState()
    {
        GenerateBoard();
        StateSaveLoader stateSaveLoader = new();
        var loadedState = stateSaveLoader.LoadState();
        UpdateBoard(loadedState);
    }

    private void OnStateUpdated(IGameState newState)
    {
        if (newState._gameStateType == GameStateType.Gameplay)
        {
            GenerateBoard();
        }

        if (newState._gameStateType == GameStateType.ResultScreen)
        {
            ClearBoard();
        }
    }

    [Command]
    private void GenerateBoard()
    {
        ClearBoard();
        InitialState = _stateFactory.Create(new ColorsMapGeneration());
        _tileControls = _gridFactory.Create(InitialState, transform, TileClickHandler, TileDraggHandler);
    }

    [Command]
    private void ClearBoard()
    {
        InitialState = default;
        foreach (var tileControl in _tileControls)
        {
            Destroy(tileControl.Value.gameObject);
        }
        _tileControls.Clear();
        _selectedTiles.Clear();
        _matchThreeExecutor = new BoardDefaultSwapM3Executor();
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
            var boardActions = _matchThreeExecutor.SwapExecute(InitialState, _selectedTiles.ToArray());
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
        InitialState = state;
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
        _interactionBlockScreen.gameObject.SetActive(true);
        foreach (var action in boardActions)
        {
            if (action is AddTilesScoreAction)
            {
                _gameScore.AddScore(action.ModifiedState.Points);
            }
            UpdateBoard(action.ModifiedState);
            yield return new WaitForSeconds(0.3f);
        }

        _interactionBlockScreen.gameObject.SetActive(false);
        _globalTimer.Continue();
    }
    private IEnumerator UpdateBoardCoroutine(Stack<IEnumerable<IBoardAction>> boardActions)
    {
        while (boardActions.Count > 0)
        {
            var nextBoardStateActions = boardActions.Pop();
            yield return UpdateBoardCoroutine(nextBoardStateActions);
        }
    }
    


    private void UpdateScore()
    {
        _gameScore.AddScore(_gameScore.ScoreTempReactiveProperty.Value);
    }
}