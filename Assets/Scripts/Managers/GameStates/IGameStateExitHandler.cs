using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameStateExitHandler : IGameStateBaseHandler
{
    void OnGameStateExit(EGameState gameState);
}
