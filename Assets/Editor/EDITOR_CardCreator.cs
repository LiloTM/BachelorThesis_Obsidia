using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EDITOR_CardCreator : EditorWindow
{
    private string targetPath;

    private string _cardName = "";
    private string _cardValue = "0";
    private CardType _cardType; 
    private CardColour _cardColour;

    private string _valueRange;

    [MenuItem("Obsidia/Card Creator")]
    public static void ShowWindow()
    {
        GetWindow<EDITOR_CardCreator>("Card Creator");
    }
    private void OnGUI()
    {
        targetPath = EditorGUILayout.TextField("Target Path: ", targetPath);
        EditorGUILayout.Space(10.0f);

        _cardName = EditorGUILayout.TextField("Name: ", _cardName);
        _cardValue = EditorGUILayout.TextField("Value: ", _cardValue);
        _cardType = (CardType)EditorGUILayout.EnumPopup(_cardType);
        _cardColour = (CardColour)EditorGUILayout.EnumPopup(_cardColour);
        EditorGUILayout.Space(20.0f);

        if (GUILayout.Button("Create Card"))
        {
            CreateCard(int.Parse(_cardValue));
        }
        EditorGUILayout.Space(10.0f);

        _valueRange = EditorGUILayout.TextField("Max Value (will start at 1): ", _valueRange);
        if (GUILayout.Button("Create Set"))
        {
            CreateSet();
        }
    }

    private void CreateCard(int cardValue)
    {
        SO_Card newCard = ScriptableObject.CreateInstance<SO_Card>();
        newCard.name = _cardName;
        newCard.value = cardValue;
        newCard.cardType = _cardType;
        newCard.colour = _cardColour;

        string uniquePath = AssetDatabase.GenerateUniqueAssetPath(targetPath + "/NewItem.asset");
        AssetDatabase.CreateAsset(newCard, uniquePath);
        AutoSetName(newCard);
    }
    private void CreateSet()
    {
        for(int i = 1; i <= int.Parse(_valueRange); i++)
        {
            CreateCard(i);
        }
    }

    private void AutoSetName(SO_Card card)
    {
        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(card, out string GUID, out long localId);
        string path = AssetDatabase.GUIDToAssetPath(GUID);

        string newName = card.colour != CardColour.Special
            ? newName = "Card_" + card.colour.ToString() + "_" + card.value.ToString()
            : newName = "Card_" + card.cardType.ToString() + card.name.ToString();

        AssetDatabase.RenameAsset(path, newName);
    }
}
