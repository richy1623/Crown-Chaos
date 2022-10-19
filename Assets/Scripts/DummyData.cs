using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyData
{
    public static List<LeaderboardItem> players = new List<LeaderboardItem>()
    {
        new LeaderboardItem(0, "You", 6),
        new LeaderboardItem(1, "Bot 1", 8),
        new LeaderboardItem(2, "Bot 2", 4),
        new LeaderboardItem(3, "Bot 3", 2),
        new LeaderboardItem(4, "Bot 4", 1),
        new LeaderboardItem(5, "Bot 5", 0),
        new LeaderboardItem(6, "Bot 6", 1),
        new LeaderboardItem(7, "Bot 7", 3),
        new LeaderboardItem(8, "Bot 8", 2),
        new LeaderboardItem(9, "Bot 9", 5)
    };
}
