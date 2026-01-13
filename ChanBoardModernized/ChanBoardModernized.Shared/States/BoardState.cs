using ChanBoardModernized.Shared.Components.DTOs;
using Fluxor;

namespace ChanBoardModernized.Shared.States;

// State
[FeatureState]
public record BoardState
{
    public BoardDTO? CurrentBoard { get; init; }
}

// Action
public record SetCurrentBoard(BoardDTO newBoard);

// Reducer
public static class BoardReducers
{
    [ReducerMethod]
    public static BoardState ReduceSetCurrent(BoardState state, SetCurrentBoard action)
    {
        return new BoardState
        {
            CurrentBoard = action.newBoard
        };
    }
}
