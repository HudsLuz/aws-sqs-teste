using Newtonsoft.Json;

namespace aws_sqs_test_1.Models
{
    public class ResultS3Dto
    {
        [JsonProperty("object")]
        public MessageDataDto MessageDate { get; set; }
    }
}
