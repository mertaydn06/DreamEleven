namespace DreamEleven.Web.Helpers
{
    public static class FormationHelper
    {
        public static List<string> GetSlots(string formation)
        {
            var slots = new List<string> { "GK" }; // her zaman 1 kaleci

            switch (formation)
            {
                case "4-4-2":
                    slots.AddRange(new[] { "LB", "CB1", "CB2", "RB" });           // 4 defans
                    slots.AddRange(new[] { "LM", "CM1", "CM2", "RM" });           // 4 orta saha
                    slots.AddRange(new[] { "ST1", "ST2" });                       // 2 forvet
                    break;

                case "4-3-3":
                    slots.AddRange(new[] { "LB", "CB1", "CB2", "RB" });
                    slots.AddRange(new[] { "CM1", "CM2", "CM3" });
                    slots.AddRange(new[] { "LW", "ST", "RW" });
                    break;

                case "3-5-2":
                    slots.AddRange(new[] { "CB1", "CB2", "CB3" });
                    slots.AddRange(new[] { "LM", "CM1", "CAM", "CM2", "RM" });
                    slots.AddRange(new[] { "ST1", "ST2" });
                    break;

                case "4-5-1":
                    slots.AddRange(new[] { "LB", "CB1", "CB2", "RB" });
                    slots.AddRange(new[] { "LM", "CM1", "CAM", "CM2", "RM" });
                    slots.Add("ST");
                    break;

                case "3-4-3":
                    slots.AddRange(new[] { "CB1", "CB2", "CB3" });
                    slots.AddRange(new[] { "LM", "CM1", "CM2", "RM" });
                    slots.AddRange(new[] { "LW", "ST", "RW" });
                    break;

                case "5-3-2":
                    slots.AddRange(new[] { "LWB", "CB1", "CB2", "CB3", "RWB" });
                    slots.AddRange(new[] { "CM1", "CM2", "CAM" });
                    slots.AddRange(new[] { "ST1", "ST2" });
                    break;

                default:
                    // fallback: 4-4-2
                    slots.AddRange(new[] { "LB", "CB1", "CB2", "RB", "LM", "CM1", "CM2", "RM", "ST1", "ST2" });
                    break;
            }

            return slots;
        }
    }
}
