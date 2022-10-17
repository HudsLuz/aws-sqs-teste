using Amazon.SQS.Model;
using Amazon.SQS;
using aws_sqs_test_1.Models;
using Newtonsoft.Json;
using Amazon;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace aws_sqs_test_1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bancoMySql = new DBSqlService();
            var client = new AmazonSQSClient(RegionEndpoint.SAEast1);
            var request = new ReceiveMessageRequest
            {
                QueueUrl = "https://sqs.sa-east-1.amazonaws.com/980540644682/aws_sqs_teste"
            };

            var response = await client.ReceiveMessageAsync(request);

            foreach (var message in response.Messages)
            {
                var result = JsonConvert.DeserializeObject<SqsResultDto>(message.Body);

                var notificacao = new Notificacao
                {
                    Filename = result.Records.FirstOrDefault().ResultS3.MessageDate.Name,
                    Filesize = result.Records.FirstOrDefault().ResultS3.MessageDate.Size,
                    Lastmodified = Convert.ToDateTime(result.Records.FirstOrDefault().dateRecord),
                };

                var fileExistente = bancoMySql.BuscarArquivo(notificacao.Filename);

                if (fileExistente != null && fileExistente.Lastmodified >= notificacao.Lastmodified)
                {
                    bancoMySql.CriarLog("Arquivo já atualizado! " + fileExistente.Filename);
                    await client.DeleteMessageAsync(request.QueueUrl, message.ReceiptHandle);
                    continue;
                }
                else if (fileExistente != null)
                {
                    var edicaoString = bancoMySql.AtualizarDados(notificacao);

                    if (edicaoString != null)
                    {
                        bancoMySql.CriarLog("Mensagem atualizada com sucesso! " + edicaoString);
                        await client.DeleteMessageAsync(request.QueueUrl, message.ReceiptHandle);
                    }
                    else
                    {
                        bancoMySql.CriarLog("Ocorreu um erro ao atualizar o dado!");
                    }

                    continue;
                }

                var retornoString = bancoMySql.SalvarDados(notificacao);

                if (retornoString != null)
                {
                    bancoMySql.CriarLog("Mensagem Salva com sucesso! " + retornoString);
                    await client.DeleteMessageAsync(request.QueueUrl, message.ReceiptHandle);
                }
                else
                    bancoMySql.CriarLog("Ocorreu um erro ao gravar o dado!");
            }

            ////Loop
            //while (true)
            //{
            //    var response = await client.ReceiveMessageAsync(request);

            //    foreach (var message in response.Messages)
            //    {
            //        var result = JsonConvert.DeserializeObject<SqsResultDto>(message.Body);

            //        var notificacao = new Notificacao
            //        {
            //            Filename = result.Records.FirstOrDefault().ResultS3.MessageDate.Name,
            //            Filesize = result.Records.FirstOrDefault().ResultS3.MessageDate.Size,
            //            Lastmodified = Convert.ToDateTime(result.Records.FirstOrDefault().dateRecord),
            //        };

            //        var fileExistente = bancoMySql.BuscarArquivo(notificacao.Filename);

            //        if (fileExistente != null && fileExistente.Lastmodified >= notificacao.Lastmodified)
            //        {
            //            bancoMySql.CriarLog("Arquivo já atualizado! " + fileExistente.Filename);
            //            await client.DeleteMessageAsync(request.QueueUrl, message.ReceiptHandle);
            //            continue;
            //        }
            //        else if (fileExistente != null)
            //        {
            //            var edicaoString = bancoMySql.AtualizarDados(notificacao);

            //            if (edicaoString != null)
            //            {
            //                bancoMySql.CriarLog("Mensagem atualizada com sucesso! " + edicaoString);
            //                await client.DeleteMessageAsync(request.QueueUrl, message.ReceiptHandle);
            //            }
            //            else
            //            {
            //                bancoMySql.CriarLog("Ocorreu um erro ao atualizar o dado!");
            //            }

            //            continue;
            //        }

            //        var retornoString = bancoMySql.SalvarDados(notificacao);

            //        if (retornoString != null)
            //        {
            //            bancoMySql.CriarLog("Mensagem Salva com sucesso! " + retornoString);
            //            await client.DeleteMessageAsync(request.QueueUrl, message.ReceiptHandle);
            //        }
            //        else
            //            bancoMySql.CriarLog("Ocorreu um erro ao gravar o dado!");
            //    }
            //}
        }
    }
}
