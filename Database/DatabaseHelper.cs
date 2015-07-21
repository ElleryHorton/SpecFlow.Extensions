﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace SpecFlow.Extensions.Database
{
    public class DatabaseHelper
    {
        public const string DATABASE_TEST_ONE = "Test.One";
        public const string DATABASE_TEST_TWO = "Test.Two";

        private string _connectionString;
        private int _databaseDelayMilliseconds = 1000;

        public DatabaseHelper(string databaseName)
        {
            _databaseName = databaseName.ToString();
            _connectionString = string.Format("Server= localhost; Database= {0}; Integrated Security=True;", _databaseName);
        }

        public DatabaseHelper(string databaseName, int delayMilliseconds)
        {
            _databaseDelayMilliseconds = delayMilliseconds;
            _databaseName = databaseName.ToString();
            _connectionString = string.Format("Server= localhost; Database= {0}; Integrated Security=True;", _databaseName);
        }

        public void Truncate(string tableName)
        {
            ExecuteNonQuery(string.Format("TRUNCATE TABLE {0}", tableName));
        }

        public int Count(string tableName)
        {
            return ExecuteScalar(string.Format("SELECT COUNT(*) FROM {0}", tableName));
        }

        private string BuildWhereParameters(List<SQL> whereParameters)
        {
            if (whereParameters != null)
            {
                if (whereParameters.Count > 0)
                {
                    var parameters = new List<string>();
                    whereParameters.ForEach(parameter => parameters.Add(parameter.ToString()));
                    return string.Format(" WHERE {0}", string.Join(" AND ", parameters));
                }
            }
            return string.Empty;
        }
        private string BuildOnParameters(List<SQL> onParameters)
        {
            if (onParameters != null)
            {
                if (onParameters.Count > 0)
                {
                    var parameters = new List<string>();
                    onParameters.ForEach(parameter => parameters.Add(parameter.ToStringNotEscaped()));
                    return string.Format(" ON {0}", string.Join(" AND ", parameters));
                }
            }
            return string.Empty;
        }

        private string GetSelectSql(string valueName, string tableName, string whereParameters)
        {
            if (!string.IsNullOrEmpty(whereParameters))
            {
                whereParameters = string.Format(" WHERE {0}", whereParameters);
            }

            return string.Format("SELECT {0} FROM {1}{2}", valueName, tableName, whereParameters);
        }

        public int GetIntegerValue(string valueName, string tableName, SQL whereParameter)
        {
            return ExecuteScalar(GetSelectSql(valueName, tableName, whereParameter.ToString()));
        }

        public int GetIntegerValue(string valueName, string tableName, List<SQL> whereParameters = null)
        {
            return ExecuteScalar(GetSelectSql(valueName, tableName, BuildWhereParameters(whereParameters)));
        }

        public DataTable Select(string tableName, SQL whereParameter)
        {
            return Select("*", tableName, whereParameter);
        }

        public DataTable Select(string tableName, List<SQL> whereParameters = null)
        {
            return Select("*", tableName, whereParameters);
        }

        public DataTable Select(string valueName, string tableName, SQL whereParameter)
        {
            return Select(GetSelectSql(valueName, tableName, whereParameter.ToString()));
        }

        public DataTable Select(string valueName, string tableName, List<SQL> whereParameters = null)
        {
            return Select(GetSelectSql(valueName, tableName, BuildWhereParameters(whereParameters)));
        }

        private DataTable Select(string sql)
        {
            Thread.Sleep(_databaseDelayMilliseconds);
            DataSet ds = Execute(sql);
            return ds.Tables[0];
        }

        public DataTable InnerJoin(string valueName, string tableName1, string tableShortName1, string tableName2, string tableShortName2, SQL onParameter, SQL whereParamter)
        {
            return Select(GetInnerJoinSql(valueName, tableName1, tableShortName1, tableName2, tableShortName2, onParameter.ToStringNotEscaped(), whereParamter.ToString()));
        }

        public DataTable InnerJoin(string valueName, string tableName1, string tableShortName1, string tableName2, string tableShortName2, List<SQL> onParameters = null, List<SQL> whereParamters = null)
        {
            return Select(GetInnerJoinSql(valueName, tableName1, tableShortName1, tableName2, tableShortName2, BuildOnParameters(onParameters), BuildWhereParameters(whereParamters)));
        }

        private string GetInnerJoinSql(string valueName, string tableName1, string tableShortName1, string tableName2, string tableShortName2, string onParameters, string whereParameters)
        {
            if (!string.IsNullOrEmpty(onParameters))
            {
                onParameters = string.Format(" ON {0}", onParameters);
            }

            if (!string.IsNullOrEmpty(whereParameters))
            {
                whereParameters = string.Format(" WHERE {0}", whereParameters);
            }

            return string.Format("SELECT {0} From {1} {2} Inner Join {3} {4}{5}{6}", valueName, tableName1, tableShortName1, tableName2, tableShortName2, onParameters, whereParameters);
        }

        public int Delete(string tableName, SQL whereParameter)
        {
            return ExecuteNonQuery(GetDeleteSql(tableName, whereParameter.ToString()));
        }

        public int Delete(string tableName, List<SQL> whereParameters = null)
        {
            return ExecuteNonQuery(GetDeleteSql(tableName, BuildWhereParameters(whereParameters)));
        }

        private string GetDeleteSql(string tableName, string whereParameters)
        {
            if (!string.IsNullOrEmpty(whereParameters))
            {
                whereParameters = string.Format(" WHERE {0}", whereParameters);
            }
            
            return string.Format("DELETE FROM {0}{1}", tableName, whereParameters);
        }

        private string _databaseName;

        private int ExecuteNonQuery(string sql)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                {
                    return sqlCommand.ExecuteNonQuery();
                }
            }
        }

        private int ExecuteScalar(string sql)
        {
            int value;
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                {
                    sqlCommand.CommandText = sql;
                    value = (Int32)sqlCommand.ExecuteScalar();
                }
            }
            return value;
        }

        private DataSet Execute(string sql)
        {
            DataSet ds = new DataSet();
            using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                {
                    sqlCommand.CommandText = sql;
                    var adapter = new SqlDataAdapter(sqlCommand);
                    adapter.Fill(ds);
                }
            }
            return ds;
        }
    }
}