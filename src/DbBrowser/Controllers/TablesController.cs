using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DbBrowser.Filters;
using DbBrowser.Models;
using Microsoft.AspNetCore.Mvc;

namespace DbBrowser.Controllers
{
    [EvaluatePerformanceFilter]
    public class TablesController : Controller
    {
        /// <summary>
        ///     Displays the available Tables.
        /// </summary>
        /// <param name="connectionStringModel">The connection string model.</param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public IActionResult Index(ConnectionStringModel connectionStringModel)
        {
            //TODO Als alternative einen Mood als String binden, evtl. Frage auf Stackoverflow dazu, wie kann ich einen Default-Typen binden??

            if (!ModelState.IsValid)
                return RedirectToAction("Index", "ConnectionString");

            var tableNames = GetTableNames(connectionStringModel.ConnectionString);

            return View(new TableNames
            {
                Names = tableNames
            });
        }

        /// <summary>
        ///     Holt die Tabellennamen mithilfe des DbSchemas. Zugriffsrechte auf das Schema sind Voraussetzung.
        /// </summary>
        /// <returns>Die verfügbaren Tabellennamen</returns>
        /// <devdoc>
        ///     Siehe auch http://stackoverflow.com/a/3914051/79485
        /// </devdoc>
        public IEnumerable<string> GetTableNames(string connectionString)
        {
            var tableNames = new List<string>();
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                //BASE TABLE bezeichnet Tabellen, VIEW wäre für Views
                var command = new SqlCommand(
                    "SELECT TABLE_NAME " +
                    "FROM INFORMATION_SCHEMA.TABLES " +
                    "WHERE TABLE_TYPE = 'BASE TABLE'", sqlConnection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tableName = reader.GetString(0);
                        tableNames.Add(tableName);
                    }
                    reader.Close();
                }
            }
            return tableNames;
        }

        /// <summary>
        ///     Gets the data set as result from the select command.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="selectCommand">The select command.</param>
        /// <returns></returns>
        private Tuple<DataTable, DataTable> GetDataSetAndSchema(string connectionString, string selectCommand)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand
                {
                    CommandText = selectCommand,
                    CommandType = CommandType.Text,
                    Connection = sqlConnection
                })
                {
                    sqlConnection.Open();

                    var dataTable = new DataTable();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataTable);
                    }

                    var schemaTable = GetSchema(cmd);
                    return new Tuple<DataTable, DataTable>(dataTable, schemaTable);
                }
            }
        }

        /// <summary>
        ///     Gets the schema as result from the select command.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <returns></returns>
        private DataTable GetSchema(SqlCommand cmd)
        {
            {
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read(); //TODO justo toread
                    var schemaTable = reader.GetSchemaTable();
                    return schemaTable;
                }
            }
        }
    }
}