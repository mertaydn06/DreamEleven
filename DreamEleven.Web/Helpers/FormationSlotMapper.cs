using DreamEleven.Entities;

namespace DreamEleven.Web.Helpers
{
    public static class FormationSlotMapper
    {
        public static Player.PositionType GetPositionType(string slot)
        {
            if (slot == "GK") return Player.PositionType.Goalkeeper;
            if (slot.StartsWith("CB") || slot == "LB" || slot == "RB" || slot == "LWB" || slot == "RWB")
                return Player.PositionType.Defender;
            if (slot.StartsWith("CM") || slot.StartsWith("CDM") || slot == "CAM" || slot == "LM" || slot == "RM")
                return Player.PositionType.Midfielder;
            if (slot.StartsWith("ST") || slot == "LW" || slot == "RW")
                return Player.PositionType.Forward;

            return Player.PositionType.Midfielder;
        }
    }
}
