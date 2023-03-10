using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionUIManager : NetworkBehaviour
{
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ClientButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TMP_InputField joinCodeInput;

    [SerializeField] private GameObject RelayMenu;
    [SerializeField] private GameObject WaitingRoom;
    [SerializeField] private GameObject GameplayMenu;

    [SerializeField] private S_TurnManager turnManager;
    private int playersReady;

    private void Start()
    {
        HostButton?.onClick.AddListener(async() =>
        {
            if (RelayManager.Instance.IsRelayEnabled)
            {
                Debug.Log("SetupRelay has been called");
                await RelayManager.Instance.SetupRelay();
            }

            if (NetworkManager.Singleton.StartHost()) { 
                Debug.Log("Host has started");
                RelayMenu.SetActive(false);
                WaitingRoom.SetActive(true);
            }
        });
        
        ClientButton?.onClick.AddListener(async() =>
        {
            if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCodeInput.text))
                await RelayManager.Instance.JoinRelay(joinCodeInput.text);

            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("Client has started");
                RelayMenu.SetActive(false);
                WaitingRoom.SetActive(true);
            }
        });

        readyButton?.onClick.AddListener(() =>
        {
            checkIfEveryoneReadyServerRpc();
            readyButton.interactable = false;
        });
    }

    [ServerRpc(RequireOwnership = false)]
    public void checkIfEveryoneReadyServerRpc()
    {
        playersReady++;
        if (NetworkManager.Singleton.ConnectedClients.Count <= playersReady)
        {
            InitiateGameStartClientRpc();
            turnManager.StartGame();
        }
    }

    [ClientRpc]
    public void InitiateGameStartClientRpc()
    {
        Debug.Log("Game starts...");
        WaitingRoom.SetActive(false);
        GameplayMenu.SetActive(true);
    }
}
