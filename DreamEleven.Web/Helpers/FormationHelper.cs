namespace DreamEleven.Web.Helpers
{
    public static class FormationHelper
    {
        public static List<string> AvailableFormations => new()
        {
            "3-3-4",
            "3-4-3",
            "3-5-2",
            "4-2-4",
            "4-4-2",
            "4-3-3",
            "4-5-1",
            "5-3-2",
            "5-4-1",
        };

        public static List<string> GetSlots(string formation)
        {
            var slots = new List<string> { "GK" }; // her zaman 1 kaleci

            switch (formation)
            {
                case "3-3-4":
                    slots.AddRange(new[] { "CB1", "CB2", "CB3" });
                    slots.AddRange(new[] { "CM1", "CM2", "CAM" });
                    slots.AddRange(new[] { "LW", "ST1", "ST2", "RW" });
                    break;

                case "3-4-3":
                    slots.AddRange(new[] { "CB1", "CB2", "CB3" });
                    slots.AddRange(new[] { "LM", "CM1", "CM2", "RM" });
                    slots.AddRange(new[] { "LW", "ST", "RW" });
                    break;

                case "3-5-2":
                    slots.AddRange(new[] { "CB1", "CB2", "CB3" });
                    slots.AddRange(new[] { "LM", "CM1", "CAM", "CM2", "RM" });
                    slots.AddRange(new[] { "ST1", "ST2" });
                    break;

                case "4-2-4":
                    slots.AddRange(new[] { "LB", "CB1", "CB2", "RB" });
                    slots.AddRange(new[] { "CM1", "CM2" });
                    slots.AddRange(new[] { "LW", "ST1", "ST2", "RW" });
                    break;

                case "4-4-2":
                    slots.AddRange(new[] { "LB", "CB1", "CB2", "RB" });
                    slots.AddRange(new[] { "LM", "CM1", "CM2", "RM" });
                    slots.AddRange(new[] { "ST1", "ST2" });
                    break;

                case "4-3-3":
                    slots.AddRange(new[] { "LB", "CB1", "CB2", "RB" });
                    slots.AddRange(new[] { "CM1", "CM2", "CM3" });
                    slots.AddRange(new[] { "LW", "ST", "RW" });
                    break;

                case "4-5-1":
                    slots.AddRange(new[] { "LB", "CB1", "CB2", "RB" });
                    slots.AddRange(new[] { "LM", "CM1", "CAM", "CM2", "RM" });
                    slots.Add("ST");
                    break;

                case "5-3-2":
                    slots.AddRange(new[] { "LWB", "CB1", "CB2", "CB3", "RWB" });
                    slots.AddRange(new[] { "CM1", "CM2", "CAM" });
                    slots.AddRange(new[] { "ST1", "ST2" });
                    break;

                case "5-4-1":
                    slots.AddRange(new[] { "LWB", "CB1", "CB2", "CB3", "RWB" });
                    slots.AddRange(new[] { "LM", "CM1", "CM2", "RM" });
                    slots.Add("ST");
                    break;

                default:
                    slots.AddRange(new[]
                    {
                        "LB", "CB1", "CB2", "RB",
                        "LM", "CM1", "CM2", "RM",
                        "ST1", "ST2"
                    });
                    break;
            }

            return slots;
        }
    }
}
