using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PraksaCodebehind
{
    [Serializable]
    internal class Groups
    {
        private string groupName;
        private List<Team> teams;

        public Groups(string group_name, List<Team>listOfTeams)
        {
            groupName = group_name ?? "";
            teams = listOfTeams ?? new List<Team>();
        }

        public string GroupName { get => groupName; set => groupName = value; }
        public List<Team> Teams { get => teams; set => teams = value; }
    }
}
