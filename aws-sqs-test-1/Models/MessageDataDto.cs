using Newtonsoft.Json;

namespace aws_sqs_test_1.Models
{
    public class MessageDataDto
    {
        [JsonProperty("key")]
        public string Name { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }
    }
}
