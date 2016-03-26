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

        public async Task ExecuteNonQueryAsync(string text, IDictionary<string, object> parameters = null)
        {
            logger.Debug("Executing non query async, text: {0}", text);
            using (var command = CreateCommand(text, parameters))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<object> ExecuteScalarAsync(string text, IDictionary<string, object> parameters = null)
        {
            logger.Debug("Executing scalar async, text: {0}", text);
            using (var command = CreateCommand(text, parameters))
            {
                return await command.ExecuteScalarAsync();
            }
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
