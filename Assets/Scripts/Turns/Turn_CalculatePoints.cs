using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Turn_CalculatePoints : Turn_Base
{
    private S_TurnManager _manager;
    private List<ulong> playerIDs;

    private int _currentPlayerTrickCount;
    private int _currentPlayerWonTricks;
    public override void EnterTurn(S_TurnManager turnManager)
    {
        Debug.Log("<color=cyan>Calculate Points</color>");
        _manager = turnManager;
        playerIDs = _manager.pointManager.playerID;

       
        float[] allPoints = FindPlayerPoints();

        //this totally can get optimised by not finding the hand every single time
        NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(playerIDs[0]);
        S_PlayerHand hand = playerObject.GetComponent<S_PlayerHand>();
        
        hand.ReceivePointsClientRpc(playerIDs.ToArray(), allPoints);

        _manager.pointManager.ResetListsServerRpc();
        _manager.ui.ResetUIClientRpc(false);
        _manager.ui.ResetPointsClientRpc();
        _manager.ChangeState();

        //TODO: not only the players but also the PointManager need to update the players trick wins and predictions. That way I access the Point Manager for all calculations being done in this turn
    }

    private float[] FindPlayerPoints()
    {
        List<float> playerPoints = new List<float>();
        int counter = 0;
        foreach (ulong player in playerIDs)
        {

            _currentPlayerTrickCount = _manager.pointManager.trickPredictList[counter];
            _currentPlayerWonTricks = _manager.pointManager.trickWonList[counter];
            Debug.Log("In CalcPoints: Predicition was " + _currentPlayerTrickCount + " and won was " + _currentPlayerWonTricks);


            playerPoints.Add(CalcPlayerPoints());
            counter++;
        }
        return playerPoints.ToArray();
    }

    private float CalcPlayerPoints()
    {
        float points = 0;
        if (_currentPlayerTrickCount == _currentPlayerWonTricks)
        {
            if (_currentPlayerTrickCount == 0)
            {
                points = 2 + _manager.GetRoundNumber();
            }
            else
            {
                points = 2 + 2 * _currentPlayerWonTricks;
            }
        }
        else
        {
            points = -1 * Mathf.Abs(_currentPlayerTrickCount-_currentPlayerWonTricks);
        }
        return points;

        /*
           Win: 2 points + 2 point per won trick
           Lose: -1 point per extra or missing trick
           0 tricks: 2 + points equal to round number

        */
    }
}
