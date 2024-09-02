using System;

namespace PraksaCodebehind
{
    public class BasketballMatch
    {
        private Team team1;
        private Team team2;
        private int team1Score;
        private int team2Score;
        public static Random random = new Random();
        public BasketballMatch()
        {
            this.team1 = new Team();
            this.team2 = new Team();
            MatchResult();
        }
        public BasketballMatch(Team team1, Team team2)
        {
            this.team1 = team1;
            this.team2 = team2;
            MatchResult();
        }

        public Team Team1 { get => team1; set => team1 = value; }
        public Team Team2 { get => team2; set => team2 = value; }
        public int Team1Score { get => team1Score; }
        public int Team2Score { get => team2Score; }

        public void MatchResult()
        {
            double fibaRankDifference = team2.FIBARanking1 - team1.FIBARanking1;
            double firstTeamWinsProbability = 0.5 + (fibaRankDifference / 68);
            double secondTeamWinsProbability = 1 - firstTeamWinsProbability;
            double winnerDecidingNumber = random.NextDouble();

            bool firstTeamWins = winnerDecidingNumber < firstTeamWinsProbability;

        
            if (firstTeamWins)
                MatchResultCalculatorFirstTeamWins(firstTeamWinsProbability, secondTeamWinsProbability);
            else
                MatchResultCalculatorSecondTeamWins(secondTeamWinsProbability, firstTeamWinsProbability);

        }
        public void MatchResultCalculatorFirstTeamWins(double firstTeamWinsProbability, double secondTeamWinsProbability)
        {

            
            if (firstTeamWinsProbability > secondTeamWinsProbability)
            {
                team1Score = (int)(random.Next(65, 75) + firstTeamWinsProbability * 40);
                team2Score = (int)(random.Next(50, 60) + secondTeamWinsProbability * 50);
            }
            else
            {
                team1Score = (int)(random.Next(65, 75) + secondTeamWinsProbability * 40);
                team2Score = (int)(random.Next(60, 66) + firstTeamWinsProbability * 50);
            }
            if (team1Score == team2Score)
                team1Score += 5;
        }
        public void MatchResultCalculatorSecondTeamWins(double secondTeamWinsProbability, double firstTeamWinsProbability)
        {
            if (secondTeamWinsProbability > firstTeamWinsProbability)
            {
                team2Score = (int)(random.Next(65, 75) + secondTeamWinsProbability * 40);
                team1Score = (int)(random.Next(50, 60) + firstTeamWinsProbability * 50);
            }
            else
            {
                team2Score = (int)(random.Next(65, 75) + firstTeamWinsProbability * 40);
                team1Score = (int)(random.Next(60, 66) + secondTeamWinsProbability * 50);
            }
            if (team1Score == team2Score)
                team1Score += 5;
        }
        public override string ToString()
        {
            return $"\t{team1.TeamName} - {team2.TeamName} ({team1Score}:{team2Score})";
        }
        public string ToStringWithoutScore()
        {
            return $"\t{team1.TeamName} - {team2.TeamName}";
        }
    }
}
