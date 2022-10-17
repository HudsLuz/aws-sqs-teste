using System;

namespace aws_sqs_test_1.Models
{
    public class Notificacao
    {
        public string Filename { get; set; }
        public long Filesize { get; set; }
        public DateTime Lastmodified { get; set; }
    }
}
