using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class S_TurnManager : NetworkBehaviour
{
    [SerializeField] private Turn_Base currentTurn;
    [Space]
    [SerializeField] private Turn_Start TurnStart = new Turn_Start();
    [SerializeField] private Turn_DealCards TurnDealCards= new Turn_DealCards();
    [SerializeField] private Turn_ChooseTrick TurnChooseTrick = new Turn_ChooseTrick();
    [SerializeField] private Turn_PlayCard TurnPlayCard = new Turn_PlayCard();
    [SerializeField] private Turn_WinTrick TurnWinTrick = new Turn_WinTrick();
    [SerializeField] private Turn_CalculatePoints TurnCalculatePoints = new Turn_CalculatePoints();
    [SerializeField] private Turn_End TurnEnd = new Turn_End();
    [Space]

    [SerializeField] public S_Deck deck;
    [SerializeField] public S_Pile pile;
    [SerializeField] public S_PlayerList playerList;
    [SerializeField] public S_UIManager ui;
    [SerializeField] public S_PointManager pointManager;

    private int turnIndex = 0;
    private int cardsPlayed = 0; /// indicates if we need to return to PlayCards Turn or if the "subround" is over
    private int roundNumber = 0; /// number of overall rounds
    public int startingPlayer;
    public int lastPlayerWon;
    public int GetRoundNumber() { return roundNumber; }

    private void Start()
    {
        turnIndex = 0;
        roundNumber = 1;
        currentTurn = TurnStart;
        startingPlayer = 0;
    }

    public void StartGame()
    {
        turnIndex = 0;
        roundNumber = 1;
        currentTurn = TurnStart;
        currentTurn.EnterTurn(this);
    }

    public void ChangeState()
    {
        turnIndex++;
        switch (turnIndex)
        {
            case 0: 
                currentTurn = TurnStart;
                break;
            case 1: 
                currentTurn = TurnDealCards;
                cardsPlayed = 0;
                break;
            case 2:
                currentTurn = TurnChooseTrick;
                break;
            case 3: 
                currentTurn = TurnPlayCard;
                break;
            case 4:
                currentTurn = TurnWinTrick;
                cardsPlayed++;
                break;
            case 5:
                /// restarts to play cards or calculates points for correctly guessed tricks
                if (cardsPlayed < roundNumber)
                {
                    turnIndex = 3;
                    currentTurn = TurnPlayCard;
                }
                else
                {
                    startingPlayer++;
                    if (startingPlayer >= playerList.getPlayerIDs().Count) startingPlayer = 0;
                    lastPlayerWon = startingPlayer;
                    currentTurn = TurnCalculatePoints;
                }
                break;
            case 6: 
                /// restarts the loop or ends the game
                roundNumber++;
                if (roundNumber > 10)
                {
                    currentTurn = TurnEnd;
                }
                else
                {
                    turnIndex = 1;
                    cardsPlayed = 0;
                    currentTurn = TurnDealCards;
                }
                break;
            case 7:
                turnIndex = 0;
                currentTurn = TurnStart;
                break;
            default:
                break;
        }
        currentTurn.EnterTurn(this);
    }
}
