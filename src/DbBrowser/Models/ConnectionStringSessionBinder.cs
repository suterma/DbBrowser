using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace DbBrowser.Models
{
    /// <summary>
    ///     Binds a model of type ConnectionString to a Session value, if not otherwise provided.
    /// </summary>
    /// <remarks>The implementation binds the values to a session variable.</remarks>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ModelBinding.IModelBinder" />
    /// <seealso
    ///     href="https://stackoverflow.com/questions/22357450/how-should-i-maintain-state-between-requests/22357977#22357977" />
    public class ConnectionStringSessionBinder : IModelBinder
    {
        private const string SessionKey = "DbBrowserConnectionString";

        /// <summary>
        ///     Attempts to bind a model.
        /// </summary>
        /// <param name="bindingContext">The <see cref="T:Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingContext" />.</param>
        /// <returns>
        ///     <para>
        ///         A <see cref="T:System.Threading.Tasks.Task" /> which will complete when the model binding process completes.
        ///     </para>
        ///     <para>
        ///         If model binding was successful, the
        ///         <see cref="P:Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingContext.Result" /> should have
        ///         <see cref="P:Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingResult.IsModelSet" /> set to <c>true</c>.
        ///     </para>
        ///     <para>
        ///         A model binder that completes successfully should set
        ///         <see cref="P:Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingContext.Result" /> to
        ///         a value returned from
        ///         <see cref="M:Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingResult.Success(System.Object)" />.
        ///     </para>
        /// </returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            // ModelName is either the class name if this matches the parameter name, or the Name explicitly provided on the model's ModelBinder attribute.
            var modelName = bindingContext.ModelName;

            // Try to fetch the value using existing value providers first, by name of the argument
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                // When not available, try to get from session
                var storedConnectionString = bindingContext.HttpContext.Session.GetString(SessionKey);

                if (string.IsNullOrEmpty(storedConnectionString))
                {
                    // Failed to get from session, complete with an error
                    bindingContext.ModelState.TryAddModelError(modelName, $"The session has no value stored for key '{SessionKey}'");
                }
                else
                {
                    // Bind retrieved value to the model
                    bindingContext.ModelState.SetModelValue(modelName, new ValueProviderResult(new StringValues(storedConnectionString)));
                    bindingContext.Result = ModelBindingResult.Success(
                        new ConnectionStringModel
                        {
                            ConnectionString = storedConnectionString
                        });
                }
            }
            else
            {
                // When provided, by existing value provider, store on session and bind it to the model, then return
                var providedValue = valueProviderResult.FirstValue;
                bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
                bindingContext.HttpContext.Session.SetString(SessionKey, providedValue);
                bindingContext.Result = ModelBindingResult.Success(
                    new ConnectionStringModel
                    {
                        ConnectionString = providedValue
                    });
            }
            return Task.CompletedTask;
        }
    }
}