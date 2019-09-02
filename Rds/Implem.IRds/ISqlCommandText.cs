﻿using System;
using System.Text;
namespace Implem.IRds
{
    public interface ISqlCommandText
    {
        string CreateSelectIdentity(
            string template,
            string identityColumnName);

        string CreateSelectStatementTerminator(bool selectIdentity);

        string CreateUpdateOrInsert(
            string tableBracket,
            string setClause,
            Action<StringBuilder> sqlWhereAppender,
            string intoAndValueClause);

        string CreateTopClause(int top);

        string CreateLimitClause(int limit);

        string CreateDelete(string template);

        string CreateIdentityInsert(string template);
    }
}