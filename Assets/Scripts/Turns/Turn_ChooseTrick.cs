using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Turn_ChooseTrick : Turn_Base
{
    private S_TurnManager _manager;
    private List<ulong> playerIDs;
    private int playersChose;

    public override void EnterTurn(S_TurnManager turnManager)
    {
        Debug.Log("<color=cyan>Choose Trick</color>");
        _manager = turnManager;
        playersChose = 0;

        playerIDs = new List<ulong>(_manager.playerList.getPlayerIDs());
        turnManager.ui.SetTrickButtonsClientRpc(_manager.GetRoundNumber());
    }

    private void CheckIfTurnOver()
    {
        if (playersChose >= playerIDs.Count)
        {
            Debug.Log("All Players have chosen their trick count.");
            _manager.ui.CloseTrickUIClientRpc();
            _manager.ChangeState();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerChoseTrickServerRpc()
    {
        playersChose++;
        CheckIfTurnOver();
    }
}
