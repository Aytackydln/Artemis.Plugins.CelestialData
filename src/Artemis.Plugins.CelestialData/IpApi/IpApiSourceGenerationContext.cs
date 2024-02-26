using System.Text.Json.Serialization;

namespace Artemis.Plugins.CelestialData.IpApi;

[JsonSerializable(typeof(IpData))]
public partial class IpApiSourceGenerationContext : JsonSerializerContext;