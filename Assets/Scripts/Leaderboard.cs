using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private Transform leaderboardContainer;
    [SerializeField] private Transform leaderboardEntry;
    
    [SerializeField] private TextMeshProUGUI condensedPlayerEntry;
    [SerializeField] private TextMeshProUGUI condensedOtherEntry1;
    [SerializeField] private TextMeshProUGUI condensedOtherEntry2;
    
    private List<LeaderboardItem> leaderboardItems;
    private List<Transform> leaderboardEntryTransformList;

    const float ENTRY_HEIGHT = 70f;
    const float CONDENSED_ENTRY_HEIGHT = 45f;
    const int WINNING_ELIMS = 30;

    private HUD hud;

    public static Leaderboard _instance;

    public static Leaderboard Instance { get { return _instance; } }
    
    private void Awake()
    {
        if (_instance == null && _instance != this)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        hud = HUD.instance;

        //DontDestroyOnLoad(gameObject);

        leaderboardEntry.gameObject.SetActive(false);
        leaderboardItems = DummyData.players;
        leaderboardItems.Sort();

        leaderboardEntryTransformList = new List<Transform>();

        for (int i = 0; i < leaderboardItems.Count; i++)
        {
            Transform entryTransform = Instantiate(leaderboardEntry, leaderboardContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -ENTRY_HEIGHT * i);
            entryTransform.gameObject.SetActive(true);

            int position = i + 1;

            entryTransform.Find("positionText").GetComponent<TextMeshProUGUI>().text = "" + position;
            entryTransform.Find("nameText").GetComponent<TextMeshProUGUI>().text = leaderboardItems[i].getPlayerName();
            entryTransform.Find("eliminationsText").GetComponent<TextMeshProUGUI>().text = "" + leaderboardItems[i].getPlayerEliminations();
            leaderboardEntryTransformList.Add(entryTransform);

            if (leaderboardItems[i].getPlayerId() == 0)
            {
                updateCondensedLeaderboard(i);  
            }
        }
    }

    public void RefreshLeaderboard()
    {
        leaderboardItems.Sort();

        for(int i = 0; i < leaderboardItems.Count; i++)
        {
            int position = i + 1;

            leaderboardEntryTransformList[i].Find("positionText").GetComponent<TextMeshProUGUI>().text = "" + position;
            leaderboardEntryTransformList[i].Find("nameText").GetComponent<TextMeshProUGUI>().text = leaderboardItems[i].getPlayerName();
            leaderboardEntryTransformList[i].Find("eliminationsText").GetComponent<TextMeshProUGUI>().text = "" + leaderboardItems[i].getPlayerEliminations();

            if (leaderboardItems[i].getPlayerId() == 0)
            {
                updateCondensedLeaderboard(i);
            }
        }
    }

    public void incrementLeaderboardItemEliminations(int playerId)
    {
        int itemIndex = leaderboardItems.FindIndex(l => l.getPlayerId() == playerId);
        int elims = leaderboardItems[itemIndex].getPlayerEliminations();
        leaderboardItems[itemIndex].setPlayerEliminations(elims + 1);

        if (elims >= WINNING_ELIMS)
        {
            endGame();
        }
        RefreshLeaderboard();
    }

    void updateCondensedLeaderboard(int i)
    {
        if (i > 0)
        {
            condensedPlayerEntry.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            condensedOtherEntry1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, CONDENSED_ENTRY_HEIGHT);
            condensedOtherEntry2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -CONDENSED_ENTRY_HEIGHT);


            condensedOtherEntry1.text = getPositionString(i) + " - " + leaderboardItems[i - 1].getPlayerEliminations() + " Eliminations";
            condensedPlayerEntry.text = getPositionString(i + 1) + " - " + leaderboardItems[i].getPlayerEliminations() + " Eliminations";
            condensedOtherEntry2.text = getPositionString(i + 2) + " - " + leaderboardItems[i + 1].getPlayerEliminations() + " Eliminations";
        }
        else
        {
            condensedPlayerEntry.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, CONDENSED_ENTRY_HEIGHT);
            condensedOtherEntry1.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            condensedOtherEntry2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -CONDENSED_ENTRY_HEIGHT);

            condensedPlayerEntry.text = getPositionString(i + 1) + " - " + leaderboardItems[i].getPlayerEliminations() + " Eliminations";
            condensedOtherEntry1.text = getPositionString(i + 2) + " - " + leaderboardItems[i + 1].getPlayerEliminations() + " Eliminations";
            condensedOtherEntry2.text = getPositionString(i + 3) + " - " + leaderboardItems[i + 2].getPlayerEliminations() + " Eliminations";
        }
    }

    string getPositionString(int position)
    {
        string pos = "";

        switch(position)
        {
            case 1:
                pos = position + "ST";
                break;
            case 2:
                pos = position + "ND";
                break;
            case 3:
                pos = position + "RD";
                break;
            default:
                pos = position + "TH";
                break;
        }

        return pos;
            
    }

    public void endGame()
    {
        LeaderboardItem topPlayer = leaderboardItems[0];
        if (topPlayer.getPlayerId() == 0) 
        {
            hud.showVictory(topPlayer.getPlayerEliminations());
        }
        else 
        {
            hud.showDefeat(topPlayer.getPlayerName(), topPlayer.getPlayerEliminations());
        }
    }
}
