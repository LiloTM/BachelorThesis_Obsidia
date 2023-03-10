using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class S_Pile : NetworkBehaviour
{
    [SerializeField] private S_Solver solver;
    [SerializeField] private List<int> _cardIndexPile = new List<int>();
    [SerializeField] private List<int> _cardPlayer = new List<int>();
    private int winnerIndex;

    [ServerRpc(RequireOwnership = false)]
    public void PlaceCardOnPileServerRpc(int cardIndex, int cardPlayer)
    {
        _cardIndexPile.Add(cardIndex);
        _cardPlayer.Add(cardPlayer);
    }

    [ServerRpc]
    public void CalculateWinnerServerRpc()
    {
        winnerIndex = solver.SolverCalc(_cardIndexPile, _cardPlayer);
        Debug.Log("<color=red>" + winnerIndex + "</color>");
    }

    public int GetWinner()
    {
        return winnerIndex;
    }

    [ServerRpc]
    public void ResetPileServerRpc()
    {
        _cardIndexPile = new List<int>();
        _cardPlayer = new List<int>();
    }
}
