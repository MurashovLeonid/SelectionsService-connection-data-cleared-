using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Superbrands.Selection.WebApi.Client.DTOs.Enums;

namespace Superbrands.Selection.WebApi.Client.DTOs
{
    public class Selection
    {
        public string Name { get; set; }
        
        public SelectionStatusDto Status { get; set; }
        
        public long ClientId { get; set; }
        
        public long PartnerId { get; set; }

        public int ProductsCount { get; set; }

        [JsonProperty("id")]
        public long SelectionId { get; set; }
        
        public bool IsDefault { get; set; }
        public int Order { get; set; }
        
        public List<Log> Logs { get; set; }
        
        public long? UpdaterId { get;  set; }
        public DateTime? UpdateTime { get;  set; }

        public int SizesCount { get; set; }

        public int ColorModelsCount { get; set; }

    }
}