using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Solver : MonoBehaviour
{
    [SerializeField] private S_Deck deck;
    private SO_Card winnerCard;
    private CardType specialCard;
    private int winnerPlayer;
    public int SolverCalc(List<int> playedCards, List<int> playerIDs)
    {
        specialCard = CardType.Humanoid;
        winnerCard = deck.GetCard(playedCards[0]);
        winnerPlayer = playerIDs[0];
        List<SO_Card> SOcards = new List<SO_Card>();

        bool isSpecialInPile = false;
        bool isGateInPile = false;
        bool isFeyInPile = false;
        bool isElemInPile = false;
        bool isCelesInPile = false;
        bool isFiendInPile = false;
      
        /// if first card is Gate, next card is winner card color
        int counter = 0;
        foreach (int index in playedCards)
        {
            SO_Card currentCard = deck.GetCard(index);
            if (currentCard.cardType != CardType.Gate)
            {
                winnerCard = deck.GetCard(playedCards[counter]);
                winnerPlayer = playerIDs[counter];
                break;
            }
            counter++;
            if(counter >= playedCards.Count)
            {
                winnerCard = deck.GetCard(playedCards[0]);
                winnerPlayer = playerIDs[0];
                return winnerPlayer;
            }
        }

        /// check the colour cards and sort out special cards
        counter = 0;
        foreach (int index in playedCards)
        {
            SO_Card currentCard = deck.GetCard(index);
            SOcards.Add(currentCard);

            /// sort out special cards
            if (currentCard.colour == CardColour.Special)
            {
                isSpecialInPile = true;
                switch (currentCard.cardType)
                {
                    case CardType.Elemental:
                        isElemInPile = true;
                        break;
                    case CardType.Celestial:
                        isCelesInPile = true;
                        break;
                    case CardType.Fiend:
                        isFiendInPile = true;
                        break;
                    case CardType.Gate:
                        isGateInPile = true;
                        break;
                    case CardType.Fey:
                        isFeyInPile = true;
                        break;
                    default:
                        break;
                }
                counter++;
                continue;
            }

            /// check the colour cards
            if (currentCard.colour != winnerCard.colour)
            {
                counter++;
                continue;
            }
            if(currentCard.value > winnerCard.value)
            {
                winnerCard = currentCard;
                winnerPlayer = playerIDs[counter];
            }
            counter++;
        }
        if (!isSpecialInPile) return winnerPlayer;

        if (isFeyInPile) specialCard = CardType.Fey;
        if (isElemInPile) specialCard = CardType.Elemental;
        else if (isCelesInPile) specialCard = CardType.Celestial;
        else if (isFiendInPile) specialCard = CardType.Fiend;

        if (isElemInPile && isCelesInPile) specialCard = CardType.Celestial;
        if (isFiendInPile && isCelesInPile) specialCard = CardType.Fiend;
        if (isFiendInPile && isElemInPile) specialCard = CardType.Elemental;

        if(isElemInPile && isCelesInPile && isFiendInPile) specialCard = CardType.Fiend;

        /// Special case: Lusche and Fey has been played
        if (isGateInPile && isFeyInPile) specialCard  = CardType.Gate;
        
        counter = 0;
        foreach (SO_Card card in SOcards)
        {
            if (specialCard == CardType.Humanoid) break;

            if (card.cardType == specialCard)
            {
                winnerCard = card;
                winnerPlayer = playerIDs[counter];
                return winnerPlayer;
            }
            counter++;
        }
        Debug.Log(winnerCard + " " + winnerCard.cardType);
        return winnerPlayer;
    }
}
