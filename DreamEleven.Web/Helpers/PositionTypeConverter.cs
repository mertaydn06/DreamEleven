using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using DreamEleven.Entities;

namespace DreamEleven.Web
{
    public class PositionTypeConverter : JsonConverter<Player.PositionType>
    {
        public override Player.PositionType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            return value switch  // json'dan gelen oyuncu mevkilerini burada entity'deki enum değerleriyle eşleştirdik.
            {
                "Goalkeeper" => Player.PositionType.Goalkeeper,
                "Defender" => Player.PositionType.Defender,
                "Midfielder" => Player.PositionType.Midfielder,
                "Forward" => Player.PositionType.Forward,
                _ => throw new JsonException($"Invalid Position value: {value}")
            };
        }

        public override void Write(Utf8JsonWriter writer, Player.PositionType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
