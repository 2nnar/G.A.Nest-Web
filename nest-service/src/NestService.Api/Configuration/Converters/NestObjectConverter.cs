using NestService.Api.Models;
using NestService.Api.ViewModels;
using NestService.Api.ViewModels.NestObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace NestService.Api.Configuration.Converters
{
    public class NestObjectConverter : JsonConverter<NestObjectViewModel>
    {
        public override NestObjectViewModel? ReadJson(JsonReader reader, Type objectType, NestObjectViewModel? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var type = Enum.Parse<NestObjectType>(jObject.SelectToken("type")?.ToString() ?? string.Empty);
            return type switch
            {
                NestObjectType.Polygon => jObject.ToObject<NestPolygonViewModel>(),
                NestObjectType.Curve => jObject.ToObject<NestCurveViewModel>(),
                NestObjectType.Group => jObject.ToObject<NestGroupViewModel>(),
                NestObjectType => jObject.ToObject<NestObjectViewModel>(),
            };
        }

        public override void WriteJson(JsonWriter writer, NestObjectViewModel? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
