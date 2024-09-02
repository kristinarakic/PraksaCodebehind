using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace PraksaCodebehind
{
    public class GroupStage
    {
        private Dictionary<string, List<Team>> groupsDictionary;
        private Dictionary<string, List<BasketballMatch>> matchesDictionary;

        public GroupStage(string jsonString)
        {
            groupsDictionary = JsonSerializer.Deserialize<Dictionary<string, List<Team>>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            matchesDictionary = new Dictionary<string, List<BasketballMatch>>()
            { 
                {"A", new List<BasketballMatch>() }, 
                {"B", new List<BasketballMatch>() }, 
                {"C", new List<BasketballMatch>() } 
            };
        }
        public Dictionary<string, List<Team>> GroupsDictionary { get => groupsDictionary; set => groupsDictionary = value; }
        public Dictionary<string, List<BasketballMatch>> MatchesDictionary { get => matchesDictionary; set => matchesDictionary = value; }
        public void PrintGroupStageMatches()
        {
            Dictionary<int, string> romanNumbers = new Dictionary<int, string>() { { 1, "I" }, { 2, "II" }, { 3, "III" } };
            for (int i = 1; i < 4; i++)
            {
                Console.WriteLine("  Grupna faza - " + romanNumbers[i] + " kolo");
                Console.WriteLine("    Grupa A:");
                SimulateTwoUniqueMatches("A");
                Console.WriteLine("    Grupa B:");
                SimulateTwoUniqueMatches("B");
                Console.WriteLine("    Grupa C:");
                SimulateTwoUniqueMatches("C");
                Console.WriteLine();

            }
        }
        public static bool ContainsMatch(List<BasketballMatch> matches, BasketballMatch match)
        {
            return matches.Any( m => 
                (m.Team1 == match.Team1 && m.Team2 == match.Team2) ||
                (m.Team2 == match.Team1 && m.Team1 == match.Team2)
            );
        }
        public void SimulateTwoUniqueMatches(string groupName)
        {
            List<Team> group = groupsDictionary[groupName];
            int matchesAdded = 0;
            Team lastTeamAdded = new Team();
            for (int i = 0; i <group.Count - 1; i++)
            {
                for (int j = i + 1; j < group.Count; j++)
                {
                    var match = new BasketballMatch(group[i], group[j]);

                    bool isValidMatch = (group[i] != lastTeamAdded && group[j] != lastTeamAdded);
                    if (!ContainsMatch(matchesDictionary[groupName], match) && isValidMatch)
                    {
                        matchesDictionary[groupName].Add(match);
                        lastTeamAdded = group[j];
                        matchesAdded++;

                        MatchOutcomeCalculator(match);

                        Console.WriteLine(match.ToString());

                        if (matchesAdded == 2)
                        {
                            return;
                        }
                        break;
                    }

                }
            }
        }
        void MatchOutcomeCalculator(BasketballMatch match)
        {
            if (match.Team1Score > match.Team2Score)
            {
                match.Team1.Points += 2;
                match.Team2.Points += 1;
                match.Team1.NumberOfWins += 1;
                match.Team2.NumberOfLosses += 1;
                match.Team1.BasketDifference += match.Team1Score - match.Team2Score;
                match.Team2.BasketDifference -= match.Team1Score - match.Team2Score;
                match.Team1.RecievedBaskets += match.Team2Score;
                match.Team2.RecievedBaskets += match.Team1Score;
            }
            else
            {
                match.Team2.Points += 2;
                match.Team1.Points += 1;
                match.Team2.NumberOfWins += 1;
                match.Team1.NumberOfLosses += 1;
                match.Team2.BasketDifference += match.Team2Score - match.Team1Score;
                match.Team1.BasketDifference -= match.Team2Score - match.Team1Score;
                match.Team2.RecievedBaskets += match.Team1Score;
                match.Team1.RecievedBaskets += match.Team2Score;
            }
            match.Team1.ScoredBaskets += match.Team1Score;
            match.Team2.ScoredBaskets += match.Team2Score;
        }   
        public Dictionary<string, List<Team>> TeamRanking(string group)
        {
            var rankedGroups = new Dictionary<string, List<Team>>();
            var teams = groupsDictionary[group];

            teams = teams.OrderByDescending(t => t.Points).ToList();

            var teamsWithTies = teams.GroupBy(t => t.Points).Where(g => g.Count() > 1).ToList();

            foreach (var tieGroup in teamsWithTies)
            {
                var tiedTeams = tieGroup.ToList();

                if (tiedTeams.Count == 2)
                {
                    var match = matchesDictionary[group].FirstOrDefault(m =>
                        (m.Team1 == tiedTeams[0] && m.Team2 == tiedTeams[1]) ||
                        (m.Team2 == tiedTeams[0] && m.Team1 == tiedTeams[1])
                    );

                    if (match != null)
                    {
                        if ((match.Team1 == tiedTeams[0] && match.Team1Score > match.Team2Score) ||
                            (match.Team2 == tiedTeams[0] && match.Team2Score > match.Team1Score))
                        {
                            continue;
                        }
                        else
                        {
                            var temp = teams[teams.IndexOf(tiedTeams[0])];
                            teams[teams.IndexOf(tiedTeams[0])] = tiedTeams[1];
                            teams[teams.IndexOf(tiedTeams[1])] = temp;
                        }
                    }

                    continue;
                }

                if (tiedTeams.Count > 2)
                {
                    var pointDifferences = tiedTeams.ToDictionary(t => t, t => 0);
                    foreach (var match in matchesDictionary[group])
                    {
                        if (tiedTeams.Contains(match.Team1) && tiedTeams.Contains(match.Team2))
                        {
                            if (match.Team1Score > match.Team2Score)
                            {
                                pointDifferences[match.Team1] += match.Team1Score - match.Team2Score;
                                pointDifferences[match.Team2] -= match.Team1Score - match.Team2Score;
                            }
                            else
                            {
                                pointDifferences[match.Team2] += match.Team2Score - match.Team1Score;
                                pointDifferences[match.Team1] -= match.Team2Score - match.Team1Score;
                            }
                        }
                    }

                    tiedTeams = tiedTeams.OrderByDescending(t => pointDifferences[t]).ToList();

                    int startIndex = teams.IndexOf(tieGroup.First());

                    for (int k = 0; k < tiedTeams.Count; k++)
                    {
                        teams[startIndex + k] = tiedTeams[k];
                    }
                }
            }

            rankedGroups[group] = teams;
            return rankedGroups;

        }
        public void PrintRankedGroups()
        {
            var groups = new[] { "A", "B", "C" };
            Console.WriteLine("  Konačan plasman u grupama:");
            foreach (var group in groups)
            {
                Console.WriteLine($"    Grupa {group}:");
                Console.WriteLine("\t\t   Ime          |  Pobede  |  Porazi  |  Bodovi  | Postignuti koševi | Primljeni koševi | Koš razlika");

                var rankedGroups = TeamRanking(group);

                for (int i = 0; i < rankedGroups[group].Count; i++)
                {
                    var team = rankedGroups[group][i];
                    Console.WriteLine($"\t  {i + 1}. {team.TeamName.PadRight(12)}\t     {team.NumberOfWins}\t\t{team.NumberOfLosses}\t   {team.Points} \t\t {team.ScoredBaskets} \t\t     {team.RecievedBaskets}\t     {team.BasketDifference.ToString("+0;-0;0")}");
                }
                Console.WriteLine();
            }
        }
        public List<Team> GetQualifiedTeams()
        {
            Dictionary<int, List<Team>> qualifiedTeams = new Dictionary<int, List<Team>>() 
            { 
                {1, new List<Team>() },
                {2, new List<Team>() },
                {3, new List<Team>() }
            };
            var rankedGroups = new Dictionary<string, List<Team>>();
            var groups = new[] { "A", "B", "C" };

            for (int i = 0; i < groups.Length; i++)
            {
                rankedGroups = TeamRanking(groups[i]);
                qualifiedTeams[1].Add(rankedGroups[groups[i]][0]);
                qualifiedTeams[2].Add(rankedGroups[groups[i]][1]);
                qualifiedTeams[3].Add(rankedGroups[groups[i]][2]);
            }
            qualifiedTeams[1] = qualifiedTeams[1].OrderByDescending(t => t.Points).ThenByDescending(t => t.BasketDifference).ThenByDescending(t => t.ScoredBaskets).ToList();
            qualifiedTeams[2] = qualifiedTeams[2].OrderByDescending(t => t.Points).ThenByDescending(t => t.BasketDifference).ThenByDescending(t => t.ScoredBaskets).ToList();
            qualifiedTeams[3] = qualifiedTeams[3].OrderByDescending(t => t.Points).ThenByDescending(t => t.BasketDifference).ThenByDescending(t => t.ScoredBaskets).Take(2).ToList();

            AssignRangs(qualifiedTeams);

            return qualifiedTeams.Values.SelectMany(list => list).ToList(); 
        }
        private void AssignRangs(Dictionary<int, List<Team>> qualifiedTeams) 
        {
            for (int i = 0; i < 3; i++)
            {
                qualifiedTeams[1][i].Rang = i + 1;
            }
            for (int i = 0; i < 3; i++)
            {
                qualifiedTeams[2][i].Rang = i + 4;
            }
            for (int i = 0; i < 2; i++)
            {
                qualifiedTeams[3][i].Rang = i + 7;
            }
        }
        public void PrintQualifiedTeams()
        {
            List<Team> qualifiedTeams = GetQualifiedTeams();
            Console.WriteLine("  Kvalifikovani timovi za eliminacionu fazu:");

            for (int i = 0; i < qualifiedTeams.Count; i++)
            {
                Console.WriteLine($"\t {i + 1}. {qualifiedTeams[i].TeamName}");
            }
            Console.WriteLine();
        }

    }
}

