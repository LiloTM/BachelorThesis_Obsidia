using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using System;

public class S_PlayerHand : NetworkBehaviour
{
    private S_Deck deck;
    private S_Pile pile;
    private S_PlayerList playerList;
    private S_UIManager uiManager;
    private bool myTurn;
    
    [SerializeField] private int[] _cardIndex;
    [SerializeField] private List<SO_Card> _cardsSO;

    private Turn_PlayCard playCardsTurn;

    [SerializeField] private float pointsCount;

    private void Awake()
    {
        deck = GameObject.Find("Deck").GetComponent<S_Deck>();
        pile = GameObject.Find("Pile").GetComponent<S_Pile>();
        playerList = GameObject.Find("PlayerList").GetComponent<S_PlayerList>();
        playCardsTurn = GameObject.Find("Turn_PlayCards").GetComponent<Turn_PlayCard>();
        uiManager = GameObject.Find("Canvas").GetComponent<S_UIManager>();

        myTurn = false;

        ResetCards();
    }

    private void ResetCards()
    {
        _cardIndex = null;
        _cardsSO = new List<SO_Card>();
    }

    private void Start()
    {
        playerList.AddPlayerToPlayerListServerRpc(this);
    }

    [ClientRpc]
    public void ReceiveCardsClientRpc(int[] drawnCards)
    {
        if(IsOwner && IsClient)
        {
            ResetCards();
            _cardIndex = drawnCards;
            UpdateCards();
        }
    }

    [ClientRpc]
    public void EnableCardPlayClientRpc()
    {
        if (IsOwner && IsClient)
        {
            myTurn = true;
            uiManager.ToggleCardClickability(true);
        }
    }

    [ClientRpc]
    public void UI_SpawnCardsClientRpc()
    {
        if (IsOwner && IsClient)
        {
            uiManager.SpawnCards(_cardsSO);
        }
    }

    [ClientRpc]
    public void ReceivePointsClientRpc(ulong[] playerIDs, float[] points)
    {
        int counter = 0;
        foreach(ulong id in playerIDs)
        {
            if (id == NetworkManager.Singleton.LocalClientId)
            {
                pointsCount += points[counter];
                break;
            }
            else
            {
                counter++;
            }
        }
        uiManager.SetPointsServerRpc(pointsCount, NetworkManager.LocalClientId);
    }

    public void PlayCardFromHand(SO_Card cardPlayed)
    {
        if (myTurn)
        {
            int counter = 0;
            foreach(SO_Card card in _cardsSO)
            {
                if(card == cardPlayed)
                {
                    SendToUIPileServerRpc(_cardIndex[counter]);
                    pile.PlaceCardOnPileServerRpc(_cardIndex[counter], (int)NetworkManager.Singleton.LocalClientId);
                    break;
                }
                counter++;
            }

            myTurn = false;
            uiManager.ToggleCardClickability(false);
            if (playCardsTurn) playCardsTurn.PlayerPlayedServerRpc();
        }
    }

    [ServerRpc]
    private void SendToUIPileServerRpc(int i)
    {
        uiManager.SetPileClientRpc(i);
    }

    private void UpdateCards()
    {
        foreach (int index in _cardIndex)
        {
            _cardsSO.Add(deck.GetCard(index));
        }
    }
}
