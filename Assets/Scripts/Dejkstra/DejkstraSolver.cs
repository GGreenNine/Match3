using System;
using System.Collections.Generic;
using System.Linq;
using BoardActions;
using Data;
using Game;
using SwapM3;
using UnityEngine;

namespace Dejkstra
{
    public class DejkstraSolver
    {
        private IGameScore _gameScore;

        private SortedSet<State> _statesPriorityByCost = new();
        private HashSet<State> _visitedStates = new();
        private BoardDefaultSwapM3Executor _defaultSwapM3Executor;
        private Dictionary<State, (State, IEnumerable<IBoardAction>)> _parents = new();


        public DejkstraSolver(IGameScore gameScore)
        {
            _gameScore = gameScore;
            _defaultSwapM3Executor = new();
        }

        public State Solve(in State startState, int targetP, out Stack<IEnumerable<IBoardAction>> actionsTakenStack)
        {
            var stateStartCopy = startState.DeepCopy();
            _statesPriorityByCost.Add(stateStartCopy);
            _parents.Add(startState, (default, null));
            
            while (_statesPriorityByCost.Count > 0)
            {
                var proceedState = _statesPriorityByCost.First(); //first() returns min value
                if (!_visitedStates.Add(proceedState))
                {
                    continue;
                }

                _statesPriorityByCost.Remove(proceedState);
                _visitedStates.Add(proceedState);

                if (proceedState.CumulativePoints >= targetP)
                {
                    actionsTakenStack = GetActionsTaken(_parents, startState, proceedState);
                    return proceedState;
                }

                var actions = FindAvailableSwaps(proceedState);
                foreach (var tilesForSwap in actions)
                {
                    var possibleNextState = proceedState.DeepCopy();
                    var actionsTaken = _defaultSwapM3Executor.SwapExecute(possibleNextState, tilesForSwap).ToList();
                    possibleNextState = actionsTaken.Last().ModifiedState.DeepCopy();
                    var allPoints = actionsTaken.Where(action => action is AddTilesScoreAction)
                        .Sum(action => action.ModifiedState.Points);
                    possibleNextState.EdgeWeight = proceedState.EdgeWeight + possibleNextState.Cost;
                    possibleNextState.CumulativePoints += allPoints;
                    
                    // If the priority queue doesn’t contain the next state,
                    // or if it contains the next state at a higher cost, then add
                    // it to the priority queue.
                    var found = _statesPriorityByCost.TryGetValue(possibleNextState, out var foundState);
                    if (!found || foundState.EdgeWeight > possibleNextState.EdgeWeight)
                    {
                        _statesPriorityByCost.Add(possibleNextState);
                        _parents.Add(possibleNextState, (proceedState, actionsTaken));
                    }
                }
            }

            Debug.Log("Solution wasn't found");
            actionsTakenStack = new();
            return startState;
        }

        private Stack<IEnumerable<IBoardAction>> GetActionsTaken(Dictionary<State, (State, IEnumerable<IBoardAction>)> parents,
            State start, State current)
        {
            Stack<IEnumerable<IBoardAction>> actionsTaken = new();
            while (!current.Equals(start, current))
            {
                actionsTaken.Push(parents[current].Item2);
                current = parents[current].Item1;
            }
            
            return actionsTaken;
        }

        private List<TileData[]> FindAvailableSwaps(in State state)
        {
            List<TileData[]> availableSwaps = new();
            HashSet<(TileData, TileData)> uniqueSwaps = new();

            for (int i = 0; i < state.ColorsMap.GetLength(0); i++)
            {
                for (int j = 0; j < state.ColorsMap.GetLength(1); j++)
                {
                    var tileToCheck = state.Board[i, j];
                    var adjacentTiles = tileToCheck.FindAdjacentTiles(state);

                    if (adjacentTiles.Count > 0)
                    {
                        foreach (var adjacentTile in adjacentTiles)
                        {
                            var swapPair = (tileToCheck, adjacentTile);
                            var reverseSwapPair = (adjacentTile, tileToCheck);

                            if (!uniqueSwaps.Contains(swapPair) && !uniqueSwaps.Contains(reverseSwapPair))
                            {
                                var tilesToSwap = new[] { tileToCheck, adjacentTile };
                                var isSwapLegal = state.IsSwapLegal(tilesToSwap);

                                if (isSwapLegal)
                                {
                                    availableSwaps.Add(tilesToSwap);
                                    uniqueSwaps.Add(swapPair);
                                }
                            }
                        }
                    }
                }
            }

            return availableSwaps;
        }
    }
    
}