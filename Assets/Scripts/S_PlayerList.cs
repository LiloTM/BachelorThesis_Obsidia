using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class S_PlayerList : MonoBehaviour
{
    [SerializeField] private List<ulong> playerIDs;
    public List<ulong> getPlayerIDs() { return playerIDs; }

    [ServerRpc]
    public void AddPlayerToPlayerListServerRpc(S_PlayerHand player)
    {
        playerIDs = (List<ulong>) NetworkManager.Singleton.ConnectedClientsIds;
    }
}
