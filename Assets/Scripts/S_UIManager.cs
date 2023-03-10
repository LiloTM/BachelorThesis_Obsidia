using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class S_UIManager : NetworkBehaviour
{
    /// Main Menu
    [SerializeField] private GameObject MainMenu;

    /// Card UI
    [SerializeField] private List<GameObject> cardPrefabs;

    /// Win Lose UI
    [SerializeField] private TMPro.TMP_Text winLose;
    [SerializeField] private TMPro.TMP_Text winTricksText;
    [SerializeField] private TMPro.TMP_Text pointsText;
    [SerializeField] private TMPro.TMP_Text predictedText;
    [SerializeField] private TMPro.TMP_Text roundNumberText;
    private int winCount; 

    /// Pile
    [SerializeField] private List<GameObject> pileUI;
    [SerializeField] private S_Deck deck;
    private List<SO_Card> pileSOcardsList = new List<SO_Card>();

    /// Trick UI
    [SerializeField] private GameObject trickUI;
    [SerializeField] private List<GameObject> buttonList;


    [SerializeField] private Turn_ChooseTrick chooseTrickTurn;
    [SerializeField] private S_PointManager pointManager;
    [SerializeField] private GameObject GameplayMenu;
    [SerializeField] private GameObject SettingsMenu;

    private void OnEnable()
    {
        int counter = 0;
        foreach (GameObject card in cardPrefabs)
        {
            S_UI_Cards cardUI = cardPrefabs[counter].GetComponent(typeof(S_UI_Cards)) as S_UI_Cards;
            cardUI.buttonPressEvent.AddListener(OnButtonClick);
            counter++;
        }
    }

    private void OnDisable()
    {
        int counter = 0;
        foreach (GameObject card in cardPrefabs)
        {
            S_UI_Cards cardUI = cardPrefabs[counter].GetComponent(typeof(S_UI_Cards)) as S_UI_Cards;
            cardUI.buttonPressEvent.RemoveListener(OnButtonClick);
            counter++;
        }
    }

    public void ToggleCardClickability(bool isClickable)
    {
        int counter = 0;
        foreach (GameObject card in cardPrefabs)
        {
            S_UI_Cards cardUI = cardPrefabs[counter].GetComponent(typeof(S_UI_Cards)) as S_UI_Cards;
            cardUI.ToggleButton(isClickable);
            counter++;
        }
    }

    public void OnButtonClick(SO_Card playedSOcard)
    {   
        S_PlayerHand hand = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<S_PlayerHand>();
        hand.PlayCardFromHand(playedSOcard);
    }

    public void SpawnCards(List<SO_Card> cardLogic)
    {
        int counter = 0;
        foreach (SO_Card card in cardLogic)
        {
            cardPrefabs[counter].SetActive(true);
            S_UI_Cards cardUI = cardPrefabs[counter].GetComponent(typeof(S_UI_Cards)) as S_UI_Cards;
            cardUI.OnSpawn(card);

            counter++;
        }
    }

    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////// ///
    /// Win Lose UI
    /// 

    [ClientRpc]
    public void SetWinLoseClientRpc(int ID)
    {
        if ((ulong)ID == NetworkManager.Singleton.LocalClientId)
        {
            winCount++; 
            winTricksText.text = "Won tricks: " + winCount;
            winLose.text = "You Win";
        }
        else winLose.text = "You Lose";
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPointsServerRpc(float points, ulong ID)
    {
        SetPointsClientRpc(points, ID);
    }

    [ClientRpc]
    public void SetPointsClientRpc(float points, ulong ID)
    {
        if(NetworkManager.Singleton.LocalClientId == ID)
        {
            pointsText.text = "Points: " + points;
        }
    }

    [ClientRpc]
    public void ResetUIClientRpc(bool resetPointsAndMenu)
    {
        winLose.text = "";
        foreach (GameObject go in pileUI)
        {
            go.SetActive(false);
        }
        pileSOcardsList = new List<SO_Card>();

        if (resetPointsAndMenu)
        {
            winTricksText.text = "";
            predictedText.text = "";
            roundNumberText.text = "";
            winCount = 0;
            pointsText.text = "";
            SettingsMenu.SetActive(false);
            GameplayMenu.SetActive(false);
            MainMenu.SetActive(true);
        }
    }
    [ClientRpc]
    public void ResetPointsClientRpc()
    {
        winCount = 0;
        winTricksText.text = "Won tricks: ";
        predictedText.text = "Prediction: ";
    }

    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////// ///
    /// Pile UI 

    [ClientRpc]
    public void SetPileClientRpc(int pileCardIndex)
    {
        pileSOcardsList.Add(deck.GetCard(pileCardIndex));
        
        int counter = 0;
        foreach (SO_Card card in pileSOcardsList)
        {
            pileUI[counter].SetActive(true);
            S_UI_Cards cardUI = pileUI[counter].GetComponent(typeof(S_UI_Cards)) as S_UI_Cards;
            cardUI.OnSpawn(card);

            counter++;
        }
    }
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////// ///
    /// Trick Button UI 
    
    [ClientRpc]
    public void SetTrickButtonsClientRpc(int roundNumber)
    {
        trickUI.SetActive(true);
        foreach (GameObject go in buttonList)
        {
            go.GetComponent<Button>().interactable = true;
            go.SetActive(false);
        }

        for (int i=0; i<roundNumber+1; i++)
        {
            buttonList[i].SetActive(true);
        }
    }

    [ClientRpc]
    public void CloseTrickUIClientRpc()
    {
        trickUI.SetActive(false);
    }

    public void OnTrickButtonClick(int index)
    {
        foreach (GameObject go in buttonList)
        {
            go.GetComponent<Button>().interactable = false;
        }
        predictedText.text = "Prediction: " + index;
        pointManager.ReceiveTrickPredictServerRpc(index, NetworkManager.Singleton.LocalClientId);
        chooseTrickTurn.PlayerChoseTrickServerRpc();
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }

    public void ResetUI()
    {
        winLose.text = "";
        winTricksText.text = "";
        predictedText.text = "";
        roundNumberText.text = "";
        winCount = 0;
        foreach (GameObject go in pileUI)
        {
            go.SetActive(false);
        }
        pileSOcardsList = new List<SO_Card>();
        pointsText.text = "";
        SettingsMenu.SetActive(false);
        GameplayMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void Disconnect()
    {
        ResetUI();
        NetworkManager.Singleton.Shutdown();
    }

    [ClientRpc]
    public void SetRoundNumberClientRpc(int number)
    {
        roundNumberText.text = "Round " + number;
    }

    [ServerRpc]
    public void DisconnectServerRpc()
    {
        ResetUIClientRpc(true);
        Disconnect();
    }
}
