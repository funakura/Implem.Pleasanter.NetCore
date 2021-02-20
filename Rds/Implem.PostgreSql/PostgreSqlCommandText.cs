﻿using Implem.IRds;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
namespace Implem.PostgreSql
{
    internal class PostgreSqlCommandText : ISqlCommandText
    {
        private static string fullTextEngine = "PG_Trgm";
        public string FullTextEngine
        {
            set { fullTextEngine = value; }
            get
            {
                return fullTextEngine;
            }
        }

        public string CreateDelete(string template)
        {
            return template + " RETURNING * ";
        }

        public string CreateRestore(string template)
        {
            return template + " RETURNING * ";
        }

        public string CreateIdentityInsert(string template)
        {
            return string.Empty;
        }

        public string CreateLimitClause(int limit)
        {
            return limit > 0 ? $" limit {limit} " : string.Empty;
        }

        public string CreateSelectIdentity(
            string template,
            string identityColumnName)
        {
            return string.Format(template, identityColumnName);
        }

        public string CreateSelectStatementTerminator(bool selectIdentity)
        {
            return selectIdentity ? "" : ";";
        }

        public string CreateTopClause(int top)
        {
            return string.Empty;
        }

        public string CreateTryCast(string left, string name, string from, string to)
        {
            return from == "int"
                ? $"\"{left}\".\"{name}\""
                : $"(CASE WHEN \"{left}\".\"{name}\"~E'^\\\\d+$' THEN \"{left}\".\"{name}\"::{to} ELSE null END)";
        }

        public string CreateUpdateOrInsert(
            string tableBracket,
            string setClause,
            Action<StringBuilder> sqlWhereAppender,
            string intoClause,
            string valueClause)
        {
            var commandText = new StringBuilder();

            commandText
                .Append(" with CTE1 as ( ")
                .Append("update ")
                .Append(tableBracket)
                .Append(setClause);
            sqlWhereAppender(commandText);
            commandText
                .Append(" returning 0 ")
                .Append(" ) ")
                .Append(" insert into ")
                .Append(tableBracket)
                .Append(" ( ")
                .Append(intoClause)
                .Append(" ) select ")
                .Append(valueClause)
                .Append(" where not exists(select * from CTE1) ");
            return commandText.ToString();
        }

        public string CreateFullTextWhereItem(string itemsTableName, string paramName)
        {
            switch (fullTextEngine)
            {
                case "":
                case "PG_Trgm":
                    return $"(\"{itemsTableName}\".\"FullText\" %> @{paramName}#CommandCount#)";
                case "PGroonga":
                    return $"(\"{itemsTableName}\".\"FullText\" &@~ @{paramName}#CommandCount#)";
                default:
                    throw new NotSupportedException($"FullTextEngine[{fullTextEngine}] is not supported by Pleasanter.");
            }
        }

        public string CreateFullTextWhereBinary(
            string itemsTableName,
            string paramName)
        {
            return $"(exists(select * from \"Binaries\" where \"Binaries\".\"ReferenceId\"=\"{itemsTableName}\".\"ReferenceId\" and (encode(\"Bin\", 'escape') %> @{paramName}#CommandCount#)))";
        }

        public Dictionary<string,string> CreateSearchTextWords(
            Dictionary<string,string> words,
            string searchText)
        {
            return new Dictionary<string, string> { [Strings.NewGuid()] = searchText };
        }
    }
}
