using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "CardList", menuName = "CardCreation/Card List")]
public class SO_CardList : ScriptableObject
{
    [SerializeField] private string _sourceFolder;
    [SerializeField] private List<SO_Card> _CardList;
    public List<SO_Card> CardList()
    {
        return _CardList;
    }

#if UNITY_EDITOR
    [ContextMenu("AutoPopulate")]
    private void AutoPopulate()
    {
        _CardList = new List<SO_Card>();

        string[] GUIDs = AssetDatabase.FindAssets("t:SO_Card", new[] { _sourceFolder });
        foreach (string guid in GUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SO_Card currentCard = AssetDatabase.LoadAssetAtPath(path, typeof(SO_Card)) as SO_Card;
            _CardList.Add(currentCard);
        }
    }
#endif
}
