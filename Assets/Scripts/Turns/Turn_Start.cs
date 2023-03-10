using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn_Start : Turn_Base
{
    private S_TurnManager _manager;
    public override void EnterTurn(S_TurnManager turnManager)
    {
        Debug.Log("<color=cyan>Start</color>");
        _manager = turnManager;
        _manager.ChangeState();
    }
}
