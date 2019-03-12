using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DbBrowser.Filters
{
    /// <summary>
    ///     Evaluates the performance of an action and stores a log message in the view bag.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.IActionFilter" />
    /// <devdoc>
    ///     See
    ///     https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/hands-on-labs/aspnet-mvc-4-custom-action-filters#aspnet-mvc-4-custom-action-filters
    ///     But also https://stackoverflow.com/questions/22535921/how-to-use-actionfilterattribute-to-log-running-times for
    ///     lifetime issues.
    ///     Action filter attributes are presumably instantiated as single instances and used over multiple requests.
    /// </devdoc>
    public class EvaluatePerformanceFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        ///     Start a performance watch.
        /// </summary>
        /// <param name="context"></param>
        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            context.HttpContext.Items[nameof(EvaluatePerformanceFilterAttribute) + "StartTicks"] = DateTime.Now.Ticks;
        }

        /// <summary>
        ///     Stops the performance watch and adds the resulting elapsed time to the view data as "ActionExecutionTime".
        /// </summary>
        /// <param name="context"></param>
        /// <inheritdoc />
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            long stopTicks = DateTime.Now.Ticks;
            long startTicks = (long) context.HttpContext.Items[nameof(EvaluatePerformanceFilterAttribute) + "StartTicks"];

            TimeSpan elapsed = TimeSpan.FromTicks(stopTicks - startTicks);

            ViewResult view = context.Result as ViewResult;
            view?.ViewData.Add("PerformanceDisplay", $"Duration: {elapsed.TotalSeconds} [sec] for action {context.ActionDescriptor.DisplayName} on {context.Controller.GetType().Name}");
        }
    }
}