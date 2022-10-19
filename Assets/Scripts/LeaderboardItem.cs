using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardItem: IComparable<LeaderboardItem>
{
    private int playerId;
    private string playerName;
    private int playerEliminations;

    public LeaderboardItem(int playerId, string playerName, int playerEliminations)
    {   
        this.playerId = playerId;
        this.playerName = playerName;
        this.playerEliminations = playerEliminations;
    }
    public int getPlayerId()
    {
        return this.playerId;
    }
    public string getPlayerName()
    {
        return this.playerName;
    }

    public int getPlayerEliminations()
    {
        return this.playerEliminations;
    }

    public void setPlayerEliminations(int eliminations)
    {
        this.playerEliminations = eliminations;
    }

    public int CompareTo(LeaderboardItem other)
    {
        return other.getPlayerEliminations().CompareTo(this.playerEliminations);
    }
}
