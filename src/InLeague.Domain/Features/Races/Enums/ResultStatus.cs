namespace InLeague.Domain.Features.Races.Enums;

public enum ResultStatus
{
    Finished = 0,       // ukończył wyścig
    DNF = 1,            // Did Not Finish — nie ukończył
    DNS = 2,            // Did Not Start — nie wystartował
    Disqualified = 3    // zdyskwalifikowany
}
