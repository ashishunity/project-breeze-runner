using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveData
{
    public int rows;
    public int cols;
    public int score;
    public int Turns;
    public int combo;
    public int selectedLevel;
    public List<int> shuffledCardIds; // full card layout in order
    public List<int> matchedCardIndices; // index in card list that are matched
}
public class SaveLoadManager : MonoBehaviour
{


    private void Start()
    {
        if (PlayerPrefs.HasKey("savegame"))
        {
            GameManager.Instance.LoadGame();
        }

    }

    public void SaveGame(SaveData data)
    {

        string json = JsonUtility.ToJson(data);
        
        PlayerPrefs.SetString("savegame", json);
        PlayerPrefs.Save();
        Debug.Log("Game Data Saved! ");
    }
}


