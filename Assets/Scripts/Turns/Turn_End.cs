using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Turn_End : Turn_Base
{
    private S_TurnManager _manager;
    private List<ulong> playerIDs = new List<ulong>();
    private int playersReady;
    public override void EnterTurn(S_TurnManager turnManager)
    {
        Debug.Log("<color=cyan>End</color>");
        _manager = turnManager;
        playerIDs = _manager.playerList.getPlayerIDs();

        _manager.ui.ResetUIClientRpc(true);
        CheckIfReadyToShutDownClientRpc();
    }

    [ClientRpc]
    private void CheckIfReadyToShutDownClientRpc()
    {
        ReadyToShutDownServerRpc();
    }

    [ServerRpc(RequireOwnership=false)]
    private void ReadyToShutDownServerRpc()
    {
        playersReady++;
        if(playerIDs.Count == playersReady) NetworkManager.Singleton.Shutdown();
    }
}
