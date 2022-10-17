using System;
using System.Data;
using aws_sqs_test_1.Models;
using Dapper;
using MySql.Data.MySqlClient;

namespace aws_sqs_test_1
{
    public class DBSqlService
    {
        private readonly MySqlConnection _conn;
        private readonly string _connectionString = "Server=127.0.0.1;Port=3306;Database=bancomysql;user=root;password=root;";

        public DBSqlService()
        {
            _conn = new MySqlConnection(_connectionString);
        }

        public Notificacao BuscarArquivo(string fileName)
        {
            var builder = new SqlBuilder();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("SELECT Filename, Filesize, Lastmodified FROM Files WHERE Filename = @Filename");

            var parametros = new DynamicParameters();
            parametros.Add(name: "Filename", value: fileName, direction: ParameterDirection.Input);
            
            var sql = builder.AddTemplate(sb.ToString());
            var result = _conn.QueryFirstOrDefault<Notificacao>(sql.RawSql, parametros);
            return result;
        }

        public string SalvarDados(Notificacao notificacao)
        {
            var sql = @"INSERT INTO Files 
                            (Filename, Filesize, Lastmodified)
                        VALUES 
                            (@Filename, @Filesize, @Lastmodified)";

            var parametros = new DynamicParameters();
            parametros.Add(name: "Filename", value: notificacao.Filename, direction: ParameterDirection.Input);
            parametros.Add(name: "Filesize", value: notificacao.Filesize, direction: ParameterDirection.Input);
            parametros.Add(name: "Lastmodified", value: notificacao.Lastmodified, direction: ParameterDirection.Input);

            _conn.Execute(sql, parametros);
            var filename = parametros.Get<string>("Filename");
            return filename;
        }

        public string AtualizarDados(Notificacao notificacao)
        {
            var sql = @"UPDATE Files
                           SET Filesize = @Filesize,
                               Lastmodified = @Lastmodified
                         WHERE Filename = @Filename";

            var parametros = new DynamicParameters();
            parametros.Add(name: "Filename", value: notificacao.Filename, direction: ParameterDirection.Input);
            parametros.Add(name: "Filesize", value: notificacao.Filesize, direction: ParameterDirection.Input);
            parametros.Add(name: "Lastmodified", value: notificacao.Lastmodified, direction: ParameterDirection.Input);

            _conn.Execute(sql, parametros);
            var filename = parametros.Get<string>("Filename");
            return filename;
        }

        public void CriarLog(string mensagem)
        {
            var sql = @"INSERT INTO Log
                            (DataEvento, Mensagem)
                        VALUES
                            (@Data, @Mensagem)";

            var parametros = new DynamicParameters();
            parametros.Add(name: "Data", value: DateTime.Now, direction: ParameterDirection.Input);
            parametros.Add(name: "Mensagem", value: mensagem, direction: ParameterDirection.Input);

            _conn.Execute(sql, parametros);
        }
    }
}
