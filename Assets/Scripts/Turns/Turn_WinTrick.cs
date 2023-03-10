using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class Turn_WinTrick : Turn_Base
{
    private S_TurnManager _manager;
    private IEnumerator coroutine;
    public override void EnterTurn(S_TurnManager turnManager)
    {
        Debug.Log("<color=cyan>Win Trick</color>");
        _manager = turnManager;

        _manager.pile.CalculateWinnerServerRpc();
        int ID = _manager.pile.GetWinner();
        SetTrickWinnerServerRpc(ID);

        coroutine = WaitAndPrint(2.0f);
        StartCoroutine(coroutine);      
    }

    [ServerRpc]
    private void SetTrickWinnerServerRpc(int ID)
    {
        NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject((ulong)ID);
        S_PlayerHand hand = playerObject.GetComponent<S_PlayerHand>();
        _manager.lastPlayerWon = ID;

        _manager.pointManager.AddTrickWinServerRpc((ulong)ID);
        _manager.ui.SetWinLoseClientRpc(ID);
    }

    private IEnumerator WaitAndPrint(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _manager.ChangeState();
    }
}
