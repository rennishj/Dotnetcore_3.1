using Microsoft.Win32.SafeHandles;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace DataAccess.ConnectionFactory
{
    public interface IDatabaseConnectionProvider
    {
        IDbConnection CreateConnection();
    }
    public class SqlConnectionProvider : IDatabaseConnectionProvider, IDisposable
    {
        private readonly string _connectionString;
        private bool _disposed = false;
        private IDbConnection _connection;
        private SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);
        public SqlConnectionProvider(string connectionString)
        {
            _connectionString = connectionString ?? throw  new ArgumentNullException(nameof(connectionString));
        }
        public IDbConnection CreateConnection()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
                return _connection;
            else
            {
                if(_connection == null)
                {
                    _connection = new SqlConnection(_connectionString);
                     _connection.Open();                    
                }
                
                return _connection;
            }           
            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _handle.Dispose();

                if (_connection != null)
                {
                    if (_connection.State != ConnectionState.Closed)
                        _connection.Close();

                    _connection.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
