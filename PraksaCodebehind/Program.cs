
using Microsoft.VisualBasic;
using PraksaCodebehind;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;


string fileJson;
try
{
    fileJson = File.ReadAllText(@"C:\Users\lenovo\source\repos\PraksaCodebehind\PraksaCodebehind\groups.json", Encoding.UTF8);
}
catch
{
    Console.WriteLine("podaci nisu ucitani");
    return;

}

GroupStage stage = new(fileJson);
EliminationPhase eliminationPhase = new EliminationPhase(stage);

stage.PrintGroupStageMatches();
stage.PrintRankedGroups();
stage.PrintQualifiedTeams();
eliminationPhase.PrintPots();
eliminationPhase.PrintQuarterFinals();
eliminationPhase.PrintSemiFinals();
eliminationPhase.PrintThirdPlaceMatch();
eliminationPhase.PrintFinalsMatch();
eliminationPhase.PrintFirstThreePlaces();


Console.ReadKey();


