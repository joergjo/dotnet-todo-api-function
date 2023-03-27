using System;
using Newtonsoft.Json;

namespace TodoApi
{
    public class TodoItem
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("text")]
        public string Content { get; set; }

        [JsonProperty("isCompleted")]
        public bool IsCompleted { get; set; }
    }
}
