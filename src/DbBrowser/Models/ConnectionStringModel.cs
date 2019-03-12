using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DbBrowser.Models
{
    /// <summary>
    /// A Model for a connection string.
    /// </summary>
    /// <remarks>Supports custom model binding, as per <seealso cref="ConnectionStringSessionBinder"/></remarks>
    [ModelBinder(BinderType = typeof(ConnectionStringSessionBinder), Name = "ConnectionString")]
    public class ConnectionStringModel
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        [Required]
        [BindRequired]
        [DataType(DataType.Text)]
        [Display(Name = "Database Connection String")]
        public string ConnectionString { get; set; }
    }
}