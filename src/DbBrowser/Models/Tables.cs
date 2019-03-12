using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Data;

namespace DbBrowser.Models
{
    /// <summary>
    /// A Model for retrieving Table names.
    /// </summary>
    public class TableNames
    {
        /// <summary>
        /// Gets or sets the table names.
        /// </summary>
        /// <value>
        /// The table names.
        /// </value>
        public IEnumerable<string> Names { get; set; }

    }
}