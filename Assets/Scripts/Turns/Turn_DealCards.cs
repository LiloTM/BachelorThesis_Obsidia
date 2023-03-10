using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Turn_DealCards : Turn_Base
{
    private S_TurnManager _manager;
    public override void EnterTurn(S_TurnManager turnManager)
    {
        Debug.Log("<color=cyan>Deal Cards</color>");
        _manager = turnManager;

        _manager.deck.ResetCardList();
        SendCardsServerRpc();
        _manager.ui.SetRoundNumberClientRpc(_manager.GetRoundNumber());
        _manager.ChangeState();
    }


    //TODO: this is a client rpc. you don't need the loop... ur dumb
    [ServerRpc] 
    private void SendCardsServerRpc()
    {
        foreach (ulong player in _manager.playerList.getPlayerIDs())
        {
            NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(player);
            S_PlayerHand hand = playerObject.GetComponent<S_PlayerHand>();

            hand.ReceiveCardsClientRpc(_manager.deck.DrawCardIndex(_manager.GetRoundNumber()));
            hand.UI_SpawnCardsClientRpc();
        }
    }

    /*[ServerRpc]
    private void V2SendCardsServerRpc()
    {
        S_PlayerHand hand = _manager.playerList.getPlayerList()[0];
        foreach (ulong player in _manager.playerList.getPlayerIDs())
        {
            hand.ReceiveCardsClientRpc(_manager.deck.DrawCardIndex(_manager.GetRoundNumber()), player);
            hand.UI_SpawnCardsClientRpc();
        }
    }*/
}