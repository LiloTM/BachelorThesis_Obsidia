using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Turn_PlayCard : Turn_Base
{
    //TODO: set an order of what player plays
    private S_TurnManager _manager;
    private List<ulong> playerIDs;
    
    private int _playersPlayed;
    private int _currentPlayerActiveIndex;
    
    public override void EnterTurn(S_TurnManager turnManager)
    {
        Debug.Log("<color=cyan>Play Card</color>");
        _manager = turnManager;
        playerIDs = new List<ulong>(_manager.playerList.getPlayerIDs());
        _manager.ui.ResetUIClientRpc(false);

        _playersPlayed = 0;
        _currentPlayerActiveIndex = _manager.lastPlayerWon;

        _manager.pile.ResetPileServerRpc();
        NextPlayerTurn();
    }

    private void CheckIfTurnOver()
    {
        if (_playersPlayed >= playerIDs.Count)
        {
            Debug.Log("All Players have played their card.");
            _manager.ChangeState();
        }
        else
        {
            NextPlayerTurn();
        }
    }

    private void NextPlayerTurn()
    {
        //TODO: order players in order of who last won
        /// calls EnableCardPlayClientRpc for singular players here, to let only them play their card

        if (_currentPlayerActiveIndex >= playerIDs.Count) _currentPlayerActiveIndex = 0;

        NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(playerIDs[_currentPlayerActiveIndex]);
        S_PlayerHand hand = playerObject.GetComponent<S_PlayerHand>();

        if(_currentPlayerActiveIndex >= playerIDs.Count) _currentPlayerActiveIndex = 0;
        else _currentPlayerActiveIndex++;

        hand.EnableCardPlayClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerPlayedServerRpc()
    {
        _playersPlayed++;
        CheckIfTurnOver();
    }
}
