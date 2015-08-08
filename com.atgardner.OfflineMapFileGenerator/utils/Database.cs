namespace com.atgardner.OMFG.utils
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.IO;
    using System.Threading.Tasks;

    public class Database : IDisposable
    {
        private readonly DbConnection connection;

        public Database(string filename)
        {
            var directoryName = Path.GetDirectoryName(filename);
            Directory.CreateDirectory(directoryName);
            connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;", filename));
        }

        public void Open()
        {
            connection.Open();
        }

        public async Task ExecuteNonQueryAsync(string text)
        {
            await ExecuteNonQueryAsync(text, new Dictionary<string, object>());
        }

        public async Task ExecuteNonQueryAsync(string text, IDictionary<string, object> parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = text;
            foreach (var pair in parameters)
            {
                var param = command.CreateParameter();
                param.ParameterName = pair.Key;
                param.Value = pair.Value;
                command.Parameters.Add(param);
            }

            await command.ExecuteNonQueryAsync();
        }

        public async Task<object> ExecuteScalarAsync(string text)
        {
            return await ExecuteScalarAsync(text, new Dictionary<string, object>());
        }

        public async Task<object> ExecuteScalarAsync(string text, IDictionary<string, object> parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = text;
            foreach (var pair in parameters)
            {
                var param = command.CreateParameter();
                param.ParameterName = pair.Key;
                param.Value = pair.Value;
                command.Parameters.Add(param);
            }

            return await command.ExecuteScalarAsync();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    connection.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
