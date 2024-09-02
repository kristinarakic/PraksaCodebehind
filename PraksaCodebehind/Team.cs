using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PraksaCodebehind
{
    [Serializable]
    public class Team
    {
        private string teamName;
        private string ISOCode;
        private int FIBARanking;
        private int points;
        private int numberOfWins;
        private int numberOfLosses;
        private int scoredBaskets;
        private int recievedBaskets;
        private int basketDifference;
        private int rang;

        public Team() {
            teamName = string.Empty;
            ISOCode = string.Empty;
            FIBARanking = 0;
            points = 0;
            numberOfWins = 0;
            numberOfLosses = 0;
            scoredBaskets = 0;
            recievedBaskets = 0;
            basketDifference = 0;
            rang = 0;
    }

        public Team(string teamName, string ISOCode, int FIBARanking)
        {
            this.teamName = teamName;
            this.ISOCode = ISOCode;
            this.FIBARanking = FIBARanking;
        }

        [JsonPropertyName("Team")]
        public string TeamName { get => teamName; set => teamName = value; }

        [JsonPropertyName("ISOCode")]
        public string ISOCode1 { get => ISOCode; set => ISOCode = value; }

        [JsonPropertyName("FIBARanking")]
        public int FIBARanking1 { get => FIBARanking; set => FIBARanking = value; }
        public int Points { get => points; set => points = value; }
        public int NumberOfWins { get => numberOfWins; set => numberOfWins = value; }
        public int NumberOfLosses { get => numberOfLosses; set => numberOfLosses = value; }
        public int ScoredBaskets { get => scoredBaskets; set => scoredBaskets = value; }
        public int RecievedBaskets { get => recievedBaskets; set => recievedBaskets = value; }
        public int BasketDifference { get => basketDifference; set => basketDifference = value; }
        public int Rang { get => rang; set => rang = value; }
        public override string ToString()
        {
            return $"    Tim: {teamName}\n    ISO Code: {ISOCode}\n    FIBA Ranking: {FIBARanking}\n";
        }
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Team other = (Team)obj;
            return TeamName == other.TeamName && ISOCode == other.ISOCode;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(TeamName, ISOCode);
        }



    }
}
