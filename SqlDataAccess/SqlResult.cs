using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SqlDataAccess
{
    public static class SqlDataAccess{
        public static SqlResult Execute(this SqlConnection sqlConnection, SqlCommand sqlCommand)
        {
            // Execute the result based on the command type.
            return new SqlResult(new List<List<string>>(),5);
        }

    }

public class SqlResult
    {
        private List<List<string>> _results;
        private int _rowsAffected;

        public List<List<string>> Results { get { return _results; } }
        public int RowsAffected { get { return _rowsAffected; } }

        public SqlResult(List<List<string>> results, int rowsAffected)
        {
            _results = results; _rowsAffected = rowsAffected;
        }
    }
}
