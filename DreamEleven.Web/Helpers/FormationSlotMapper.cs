using DreamEleven.Entities;

namespace DreamEleven.Web.Helpers
{
    public static class SlotPositionMapper
    {
        public static Player.PositionType GetPositionType(string slot)
        {
            if (slot == "GK") return Player.PositionType.Goalkeeper;
            if (slot.StartsWith("CB") || slot == "LB" || slot == "RB" || slot == "LWB" || slot == "RWB")
                return Player.PositionType.Defender;
            if (slot.StartsWith("CM") || slot == "LM" || slot == "RM" || slot == "CAM")
                return Player.PositionType.Midfielder;
            if (slot.StartsWith("ST") || slot == "LW" || slot == "RW")
                return Player.PositionType.Forward;

            return Player.PositionType.Midfielder;
        }
    }
}
