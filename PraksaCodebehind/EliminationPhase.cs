using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PraksaCodebehind
{
    internal class EliminationPhase
    {
        private Dictionary<string, List<Team>> potsDictionary;
        private List<BasketballMatch> quarterFinalsMatches;
        private List<BasketballMatch> semiFinalsMatches;
        private List<Team> firstThreePlacesTeams;
        private GroupStage stage;

        public EliminationPhase(GroupStage stage)
        {
            this.stage = stage;
            potsDictionary = AddTeamsToPotsDictionary();
            quarterFinalsMatches = GenerateAllQuarterFinalsMatches();
            semiFinalsMatches = GenerateSemiFinalsMatches();
            firstThreePlacesTeams = new List<Team>();
        }


        //Generators
        private Dictionary<string, List<Team>> AddTeamsToPotsDictionary()
        {
            Dictionary<string, List<Team>> potsDictionary = new Dictionary<string, List<Team>>()
            {
                {"D", new List<Team>()},
                {"E", new List<Team>()},
                {"F", new List<Team>()},
                {"G", new List<Team>()}
            };

            foreach (var team in stage.GetQualifiedTeams())
            {
                if (team.Rang == 1 || team.Rang == 2)
                    potsDictionary["D"].Add(team);
                else if (team.Rang == 3 || team.Rang == 4)
                    potsDictionary["E"].Add(team);
                else if (team.Rang == 5 || team.Rang == 6)
                    potsDictionary["F"].Add(team);
                else if (team.Rang == 7 || team.Rang == 8)
                    potsDictionary["G"].Add(team);
            }

            return potsDictionary;
        }

        // Quarter Finals
        private List<BasketballMatch> GenerateAllQuarterFinalsMatches()
        {
            List<BasketballMatch> quarterFinalsMatches = new List<BasketballMatch>();
            Dictionary<string, List<Team>> pots = CopyPotsDictionary(potsDictionary);

            var match1 = GenerateOneQuarterFinalsMatch("D", "G");
            pots["D"].Remove(match1.Team1);
            pots["G"].Remove(match1.Team2);

            var match2 = GenerateOneQuarterFinalsMatch("E", "F");
            pots["E"].Remove(match2.Team1);
            pots["F"].Remove(match2.Team2);

            var match3 = new BasketballMatch(pots["D"][0], pots["G"][0]);
            var match4 = new BasketballMatch(pots["E"][0], pots["F"][0]);

            quarterFinalsMatches.Add(match1);
            quarterFinalsMatches.Add(match2);
            quarterFinalsMatches.Add(match3);
            quarterFinalsMatches.Add(match4);

            return quarterFinalsMatches;
        }
        private BasketballMatch GenerateOneQuarterFinalsMatch(string groupName1, string groupName2)
        {
            List<BasketballMatch> matchesGroupStage = stage.MatchesDictionary.Values.SelectMany(list => list).ToList();
            List<BasketballMatch> matchesQuarterFinals = stage.MatchesDictionary.Values.SelectMany(list => list).ToList();

            BasketballMatch match, secondPairMatch;
            int indexTeam1, indexTeam2, indexTeam1SecondMatch, indexTeam2SecindMatch;

            do
            {
                indexTeam1 = BasketballMatch.random.Next(0, potsDictionary[groupName1].Count);
                indexTeam2 = BasketballMatch.random.Next(0, potsDictionary[groupName2].Count);
                indexTeam1SecondMatch = potsDictionary[groupName1].Count - 1 - indexTeam1;
                indexTeam2SecindMatch = potsDictionary[groupName2].Count - 1 - indexTeam2;

                match = new BasketballMatch(potsDictionary[groupName1][indexTeam1], potsDictionary[groupName2][indexTeam2]);
                secondPairMatch = new BasketballMatch(potsDictionary[groupName1][indexTeam1SecondMatch], potsDictionary[groupName2][indexTeam2SecindMatch]);
            }
            while (matchesGroupStage.Contains(match) || matchesGroupStage.Contains(secondPairMatch));
            matchesQuarterFinals.Add(match);
            matchesQuarterFinals.Add(secondPairMatch);

            return match;
        }
        private Dictionary<string, List<Team>> CopyPotsDictionary(Dictionary<string, List<Team>> original)
        {
            Dictionary<string, List<Team>> copy = new Dictionary<string, List<Team>>();
            foreach (var list in original)
            {
                copy[list.Key] = new List<Team>(list.Value);
            }
            return copy;
        }

            // Semi Finals
        private List<Team> GenerateAllQuarterFinalsWinners()
        {
            List<Team> quarterFinalsWinners = new List<Team>();
            foreach (var match in quarterFinalsMatches)
            {
                if (match.Team1Score > match.Team2Score)
                    quarterFinalsWinners.Add(match.Team1);
                else
                    quarterFinalsWinners.Add(match.Team2);
            }
            return quarterFinalsWinners;
        }
        private List<BasketballMatch> GenerateSemiFinalsMatches()
        {
            List<Team> quarterFinalsWinners = GenerateAllQuarterFinalsWinners();
            List<BasketballMatch> semiFinalsMatches = new List<BasketballMatch>();

            Team? team2 = new Team();
            int team1Index = BasketballMatch.random.Next(0, quarterFinalsWinners.Count);
            Team team1 = quarterFinalsWinners[team1Index];

            if (potsDictionary["D"].Contains(team1) || potsDictionary["G"].Contains(team1))
                team2 = quarterFinalsWinners.FirstOrDefault(t => potsDictionary["F"].Contains(t)) ?? quarterFinalsWinners.FirstOrDefault(t => potsDictionary["E"].Contains(t));
            else if (potsDictionary["F"].Contains(team1) || potsDictionary["E"].Contains(team1))
                team2 = quarterFinalsWinners.FirstOrDefault(t => potsDictionary["D"].Contains(t)) ?? quarterFinalsWinners.FirstOrDefault(t => potsDictionary["G"].Contains(t));

            if (team2 != null)
            {
                var match1 = new BasketballMatch(team1, team2);
                quarterFinalsWinners.Remove(match1.Team1);
                quarterFinalsWinners.Remove(match1.Team2);
                var match2 = new BasketballMatch(quarterFinalsWinners[0], quarterFinalsWinners[1]);
                semiFinalsMatches.Add(match1);
                semiFinalsMatches.Add(match2);
            }
            return semiFinalsMatches;
        }

            // Finals
        private List<Team> GenerateSemiFinalsWinners()
        {
            List<Team> semiFinalsWinners = new List<Team>();
            foreach (var match in semiFinalsMatches)
            {
                if (match.Team1Score > match.Team2Score)
                    semiFinalsWinners.Add(match.Team1);
                else
                    semiFinalsWinners.Add(match.Team2);
            }
            return semiFinalsWinners;
        }
        private BasketballMatch GenerateFinalsMatch()
        {
            List<Team> semiFinalsWinners = GenerateSemiFinalsWinners();
            BasketballMatch match = new BasketballMatch(semiFinalsWinners[0], semiFinalsWinners[1]);
            if (match.Team1Score > match.Team2Score)
            {
                firstThreePlacesTeams.Add(match.Team1);
                firstThreePlacesTeams.Add(match.Team2);
            }
            else
            {
                firstThreePlacesTeams.Add(match.Team2);
                firstThreePlacesTeams.Add(match.Team1);
            }
            return match;
        }
        
            // Third Place
        private List<Team> GenerateSemiFinalsLoosers()
        {
            List<Team> semiFinalsLoosers = new List<Team>();
            foreach (var match in semiFinalsMatches)
            {
                if (match.Team1Score > match.Team2Score)
                    semiFinalsLoosers.Add(match.Team2);
                else
                    semiFinalsLoosers.Add(match.Team1);
            }
            return semiFinalsLoosers;
        }
        private BasketballMatch GenerateThirdPlaceMatch()
        {
            List<Team> semiFinalLoosers = GenerateSemiFinalsLoosers();
            BasketballMatch match = new BasketballMatch(semiFinalLoosers[0], semiFinalLoosers[1]);
            if (match.Team1Score > match.Team2Score)
                firstThreePlacesTeams.Add(match.Team1);
            else
                firstThreePlacesTeams.Add(match.Team2);

            return match;
        }

            //Printers
        public void PrintPots()
        {
            Console.WriteLine("  Šeširi: ");
            foreach (var group in potsDictionary.Keys)
            {
                Console.WriteLine("    Šešir " + group);
                foreach (var team in potsDictionary[group])
                {
                    Console.WriteLine("\t" + team.TeamName);
                }
            }
            Console.WriteLine();
        }
        public void PrintSemiFinals()
        {
            Console.WriteLine("  Polufinale");
            foreach (var match in semiFinalsMatches)
            {
                Console.WriteLine("    "+match.ToString());
            }
            Console.WriteLine();
        }
        public void PrintQuarterFinals()
        {
            Console.WriteLine("  Eliminaciona faza:");
            for (int i = 0; i < quarterFinalsMatches.Count; i++)
            {
                Console.WriteLine(quarterFinalsMatches[i].ToStringWithoutScore());
                if (i == 1)
                    Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("  Četvrtfinale:");
            for (int i = 0; i < quarterFinalsMatches.Count; i++)
            {
                Console.WriteLine(quarterFinalsMatches[i].ToString());
                if (i == 1)
                    Console.WriteLine();
            }
            Console.WriteLine();
        }
        public void PrintFinalsMatch()
        {
            BasketballMatch match = GenerateFinalsMatch();
            Console.WriteLine("  Finale:");
            Console.WriteLine(match.ToString());
            Console.WriteLine();
        }
        public void PrintThirdPlaceMatch()
        {
            BasketballMatch match = GenerateThirdPlaceMatch();
            Console.WriteLine("  Utakmica za treće mesto:");
            Console.WriteLine(match.ToString());
            Console.WriteLine();
        }
        public void PrintFirstThreePlaces()
        {
            Console.WriteLine("  Medalje:");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine($"\t{i+1}. {firstThreePlacesTeams[i].TeamName}");
            }
        }

    }
}
