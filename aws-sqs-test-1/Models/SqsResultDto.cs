using Newtonsoft.Json;
using System.Collections.Generic;

namespace aws_sqs_test_1.Models
{
    public class SqsResultDto
    {
        [JsonProperty("Records")]
        public List<RecordsDto> Records { get; set; }
    }

    public class RecordsDto
    {
        [JsonProperty("eventTime")]
        public string dateRecord { get; set; }

        [JsonProperty("s3")]
        public ResultS3Dto ResultS3 { get; set; }
    }
}
