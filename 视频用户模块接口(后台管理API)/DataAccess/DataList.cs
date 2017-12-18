using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess
{
    public class DataList
    {
        /// <summary>
        /// 返回分页记录
        /// </summary>
        /// <param name="TableName">表名称</param>
        /// <param name="Fields">表字段(a,b,c,d)</param>
        /// <param name="SqlWhere">查询条件，不需要加where</param>
        /// <param name="OrderField">排序字段(ID Asc,Name Desc)</param>
        /// <param name="PageSize">每页记录数</param>
        /// <param name="CurrentPage">当前页数</param>
        /// <param name="RecordCount">记录总数</param>
        /// <returns></returns>
        public static DataSet GetList(string TableName, string Fields, string SqlWhere, string OrderField, int PageSize, int CurrentPage, out int RecordCount)
        {
            return SqlHelper.GetList(TableName, Fields, SqlWhere, OrderField, PageSize, CurrentPage, out RecordCount);
        }

        /// <summary>
        /// 返回指定条数的记录
        /// </summary>
        /// <param name="ReturnCount">返回条数</param>
        /// <param name="Table">表名或视图名</param>
        /// <param name="Fields">返回字段集合</param>
        /// <param name="StrWhere">条件不要加Where</param>
        /// <param name="Sort">排序不要加Order By</param>
        /// <returns></returns>
        public static DataSet GetList(int ReturnCount, string Table, string Fields, string StrWhere, string Sort)
        {
            return SqlHelper.GetList(ReturnCount, Table, Fields, StrWhere, Sort);
        }      

    }
}