using ChanBoardModernized.Shared.Components.DTOs;
using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChanBoardModernized.Shared.States;

// State
[FeatureState]
public record ThreadState
{
    public ThreadDTO? CurrentDTO { get; init; }
}

// Action
public record SetCurrentThread(ThreadDTO newThread);
public record CommentPostedAddRecentComments(Guid Guid, int Count);

// Reducer
public static class ThreadReducers
{
    [ReducerMethod]
    public static ThreadState ReduceSetCurrent(ThreadState state, SetCurrentThread action)
    {
        return new ThreadState
        {
            CurrentDTO = action.newThread
        };
    }
}
