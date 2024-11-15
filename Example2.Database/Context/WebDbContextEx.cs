using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example2.Database.Context
{
    public partial class WebDbContext
    {
        public IEnumerable<T> Query<T>(string sql, object? param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var tx = Database.CurrentTransaction?.GetDbTransaction();

            return Database.GetDbConnection().Query<T>(sql, param, tx, buffered, commandTimeout, commandType);
        }
    }
}
