using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Superbrands.Libs.DDD.Abstractions;

namespace Superbrands.Selection.WebApi.Extensions
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Gets user from HttpContext
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static OperationLog GetOperationLog([NotNull] this HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var source = context.Request.Host.ToString();
            return new OperationLog(source, context.GetUserId());
        }

        private static string GetUserId(this HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("userId", out var userId))
                userId = context.User.GetUserId();

            return userId;
        }
    }
}
