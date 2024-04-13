using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameStateEnterHandler : IGameStateBaseHandler
{
    void OnGameStateEnter(EGameState gameState);
}
