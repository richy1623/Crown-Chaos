using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scores : MonoBehaviour
{
    Leaderboard leaderboard;

    // Start is called before the first frame update
    void Start()
    {
        leaderboard = new Leaderboard();
    }

    public String getLeaderboard()
    {
        return leaderboard.ToString();
    }

    private void OnEnable()
    {
        Player.LoadPlayer += LoadPlayer;
        Bolt.OnPlayerHit += HitPlayer;
    }

    private void OnDisable()
    {
        Bolt.OnPlayerHit -= HitPlayer;
        Player.LoadPlayer += LoadPlayer;
    }

    private void HitPlayer(int shooter, int hit)
    {
        if (shooter != hit)
        {
            leaderboard.scorePoint(shooter);
        }
        else
        {
            leaderboard.loosePoint(shooter);
        }
        print(leaderboard);
    }

    private void LoadPlayer(int playerID)
    {
        leaderboard.addPlayer(playerID);
    }

    public class Score
    {
        private int playerID;
        private int playerPoints;
        private int position;
        public Score(int playerID, int position)
        {
            this.playerID = playerID;
            this.playerPoints = 0;
            this.position = position;
        }

        public int id() { return playerID; }
        public int points() { return playerPoints; }
        public int pos() { return position; }

        public void inc() { playerPoints++; }
        public void dec() { playerPoints--; }

        public void swap(Score other)
        {
            int temp = position;
            position = other.position;
            other.position = temp;
        }

        public bool isPlayer(int id)
        {
            return playerID == id;
        }

        public override string ToString() { return $"{position+1}: player{playerID}, score={playerPoints}"; }
    }

    public class Leaderboard
    {
        public List<Score> leaderboard;

        public Leaderboard()
        {
            leaderboard = new List<Score>();
        }

        public void addPlayer(int PlayerID)
        {
            leaderboard.Add(new Score(PlayerID, leaderboard.Count));
        }

        public void scorePoint(int PlayerID)
        {
            foreach (Score s in leaderboard)
            {
                if (s.isPlayer(PlayerID)){
                    s.inc();
                    upSort(s.pos());
                    print("found " + PlayerID+" points "+s.points());
                    break;
                }
            }
        }
        public void loosePoint(int PlayerID)
        {
            foreach (Score s in leaderboard)
            {
                if (s.isPlayer(PlayerID))
                {
                    s.dec();
                    downSort(s.pos());
                    break;
                }
            }
        }
        private void upSort(int index)
        {
            if (index <= 0) return;
            Score score = leaderboard[index];
            for (int i = index; i > 0; i--)
            {
                Score temp = leaderboard[i - 1];
                if (score.points() > temp.points())
                {
                    score.swap(temp);
                    leaderboard[i - 1] = score;
                    leaderboard[i] = temp;
                }
                else
                {
                    break;
                }
            }
        }
        private void downSort(int index)
        {
            if (index >= leaderboard.Count - 1) return;
            Score score = leaderboard[index];
            for (int i = index; i < leaderboard.Count-1; i++)
            {
                Score temp = leaderboard[i + 1];
                if (score.points() < temp.points())
                {
                    score.swap(temp);
                    leaderboard[i] = temp;
                    leaderboard[i + 1] = score;
                }
                else
                {
                    break;
                }
            }
        }

        public override string ToString()
        {
            return "Leaderboard\n"+String.Join("\n", leaderboard);
        }
    }
}
