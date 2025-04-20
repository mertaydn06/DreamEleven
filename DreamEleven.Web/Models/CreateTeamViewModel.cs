namespace DreamEleven.Web.Models
{
    public class CreateTeamViewModel
    {
        public string TeamName { get; set; } = string.Empty;
        public string Formation { get; set; } = string.Empty;

        public List<TeamPlayerInput> Players { get; set; } = new List<TeamPlayerInput>();
    }
}
