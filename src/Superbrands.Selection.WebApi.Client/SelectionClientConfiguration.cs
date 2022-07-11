using System;

namespace Superbrands.Selection.WebApi.Client
{
    /// <summary>
    /// Конфигурация для клиента
    /// </summary>
    public class SelectionClientConfiguration
    {
        /// <summary>
        /// Базовый путь к сервису, http://domain.local/api/v1/
        /// </summary>
        public Uri BaseUri { get; set; }
    }
}