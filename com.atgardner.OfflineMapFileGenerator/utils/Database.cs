namespace com.atgardner.OMFG.utils
{
    using NLog;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.IO;
    using System.Threading.Tasks;

    public class Database : IDisposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly DbConnection connection;

        public Database(string filename)
        {
            var directoryName = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            connection = new SQLiteConnection(string.Format("Data Source={0};", filename));
        }

        public void Open()
        {
            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }

        public async Task ExecuteNonQueryAsync(string text, IDictionary<string, object> parameters = null)
        {
            var command = CreateCommand(text, parameters);
            logger.Debug("Executing non query async, text: {0}", text);
            await command.ExecuteNonQueryAsync();
            logger.Debug("Done executing non query async, text: {0}", text);
        }

        public async Task<object> ExecuteScalarAsync(string text, IDictionary<string, object> parameters = null)
        {
            var command = CreateCommand(text, parameters);
            logger.Debug("Executing scalar async, text: {0}", text);
            var result = await command.ExecuteScalarAsync();
            logger.Debug("Done executing scalar async, text: {0}, result: {1}", text, result);
            //await Task.Run(() => {
            //    Console.WriteLine("Inside the running task");
            //});
            return result;
        }

        private bool disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    connection.Dispose();
                }

                disposed = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

        private DbCommand CreateCommand(string text, IDictionary<string, object> parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = text;
            if (parameters == null)
            {
                return command;
            }

            foreach (var pair in parameters)
            {
                var param = command.CreateParameter();
                param.ParameterName = pair.Key;
                param.Value = pair.Value;
                command.Parameters.Add(param);
            }

            return command;
        }
    }
}
