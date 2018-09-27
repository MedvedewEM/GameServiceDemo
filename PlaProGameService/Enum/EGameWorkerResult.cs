namespace PlaProGameService.Enum
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EGameWorkerResult
    {
        Ok,
        NoFreeBots,
        InvalidQuery
    }
}
