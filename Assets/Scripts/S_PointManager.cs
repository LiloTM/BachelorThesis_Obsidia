using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class S_PointManager : NetworkBehaviour
{
    public List<int> trickPredictList = new List<int>();
    public List<int> trickWonList = new List<int>();
    public List<ulong> playerID = new List<ulong>();

    [ServerRpc(RequireOwnership = false)]
    public void ReceiveTrickPredictServerRpc(int tricksPredict, ulong id)
    {
        trickPredictList.Add(tricksPredict);
        playerID.Add(id);
        trickWonList.Add(0);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddTrickWinServerRpc(ulong id)
    {
        int counter = 0;
        foreach(ulong player in playerID)
        {
            if (player == id)
            {
                trickWonList[counter]++;
            }
            counter++;
        }
    }

    [ServerRpc] 
    public void ResetListsServerRpc()
    {
        trickPredictList = new List<int>();
        trickWonList = new List<int>();
        playerID = new List<ulong>();
    }
}
