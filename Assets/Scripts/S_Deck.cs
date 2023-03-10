using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Deck : MonoBehaviour
{
    [SerializeField] private SO_CardList soCardList;
    [SerializeField] private List<SO_Card> _cardList;

    private void Awake()
    {
        ResetCardList();

        int counter = 0;
        foreach(SO_Card current in _cardList)
        {
            current.index = counter;
            counter++;
        }
    }

    public int[] DrawCardIndex(int cardsToDraw)
    {
        //finds differing random numbers
        List<int> randomIntegers = new List<int>();
        List<int> randomCardIndex = new List<int>();
        int random;
        for (int i = 0; i<cardsToDraw; i++)
        {
            do { random = Random.Range(0, _cardList.Count-i); } // Count-i, otherwise an overflow can occur
            while (randomIntegers.Contains(random));
            randomIntegers.Add(random);
        }
        //find card index and remove cards from list
        foreach (int i in randomIntegers)
        {
            randomCardIndex.Add(_cardList[i].index);
            _cardList.RemoveAt(i);
        }

        int[] indexArray = randomCardIndex.ToArray();
        return indexArray;
    }

    public SO_Card GetCard(int index)
    {
        //Binary Search
        int lowerBound = 0;
        int upperBound = soCardList.CardList().Count-1;
        List<SO_Card> currentList = new List<SO_Card>(soCardList.CardList());

        while (lowerBound <= upperBound)
        {
            int midPoint = (lowerBound + upperBound) / 2;
        
            if (currentList[midPoint].index > index)
            {
                upperBound = midPoint - 1;
            }
            else if (currentList[midPoint].index < index)
            {
                lowerBound = midPoint + 1;
            }
            else return currentList[midPoint];
        }
        return null;
    }

    public void ResetCardList()
    {
        _cardList = new List<SO_Card>(soCardList.CardList());
    }
}
