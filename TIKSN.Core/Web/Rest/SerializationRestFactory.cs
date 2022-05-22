using System;
using System.Collections.Generic;
using TIKSN.Serialization;

namespace TIKSN.Web.Rest
{
    public class SerializationRestFactory : ISerializerRestFactory, IDeserializerRestFactory
    {
        private readonly IDictionary<string, Tuple<ISerializer<string>, IDeserializer<string>>> map;

        public SerializationRestFactory(JsonSerializer jsonSerializer, JsonDeserializer jsonDeserializer,
            DotNetXmlSerializer dotNetXmlSerializer, DotNetXmlDeserializer dotNetXmlDeserializer) => this.map = new Dictionary<string, Tuple<ISerializer<string>, IDeserializer<string>>>
            {
                {
                    "application/json",
                    new Tuple<ISerializer<string>, IDeserializer<string>>(jsonSerializer, jsonDeserializer)
                },
                {
                    "application/xml",
                    new Tuple<ISerializer<string>, IDeserializer<string>>(dotNetXmlSerializer, dotNetXmlDeserializer)
                }
            };

        IDeserializer<string> IDeserializerRestFactory.Create(string mediaType) => this.map[mediaType].Item2;

        ISerializer<string> ISerializerRestFactory.Create(string mediaType) => this.map[mediaType].Item1;
    }
}
