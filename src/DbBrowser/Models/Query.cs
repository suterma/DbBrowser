using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DbBrowser.Models
{
    public class Query
    {
        [Required]
        [BindRequired]
        public string ConnectionString { get; set; }

        [Required]
        [BindRequired]
        public string SelectCommand { get; set; }

        /// <summary>
        ///     Gets or sets the resulting data set from the query.
        /// </summary>
        /// <value>
        ///     The data set.
        /// </value>
        [BindNever]
        public DataTable DataSet { get; set; }

        /// <summary>
        ///     Gets or sets the resulting data set from the query.
        /// </summary>
        /// <value>
        ///     The data set.
        /// </value>
        [BindNever]
        public DataTable TableSchema { get; set; }
    }
}