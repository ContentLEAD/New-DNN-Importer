using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

    public class DataTables
    {

       static public DataTable GetTable(string whichTable)
        {
            DataTable table = new DataTable();
            DataColumn column;

            switch (whichTable)
            {
                case "Blog_Entries":
                    {
                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.Int32");
                        column.ColumnName = "BlogID";
                        table.Columns.Add(column);

                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.Int32");
                        column.ColumnName = "EntryID";
                        table.Columns.Add(column);

                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.String");
                        column.ColumnName = "Title";
                        table.Columns.Add(column);

                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.String");
                        column.ColumnName = "Entry";
                        table.Columns.Add(column);

                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.DateTime");
                        column.ColumnName = "AddedDate";
                        table.Columns.Add(column);

                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.Boolean");
                        column.ColumnName = "Published";
                        table.Columns.Add(column);

                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.String");
                        column.ColumnName = "Description";
                        table.Columns.Add(column);

                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.Boolean");
                        column.ColumnName = "AllowComments";
                        table.Columns.Add(column);

                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.Boolean");
                        column.ColumnName = "DisplayCopyright";
                        table.Columns.Add(column);

                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.String");
                        column.ColumnName = "Copyright";
                        table.Columns.Add(column);

                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.String");
                        column.ColumnName = "Permalink";
                        table.Columns.Add(column);

                        column = new DataColumn();
                        column.DataType = System.Type.GetType("System.String");
                        column.ColumnName = "BraftonID";
                        table.Columns.Add(column);

                        return table;
                    }
                case "Blog_Entry_Categories":
                    {
                        table.Columns.Add("EntryCatID");
                        table.Columns.Add("EntryID");
                        table.Columns.Add("CatID");
                        return table;
                    }
                case "Blog_Categories":
                    {
                        table.Columns.Add("CatID");
                        table.Columns.Add("Category");
                        table.Columns.Add("slug");
                        table.Columns.Add("ParentID");
                        table.Columns.Add("PortalID");
                        return table;
                    }
                default:
                    return table;
            }
        }

    }
