using Newtonsoft.Json;
using Superbrands.Libs.DDD.Abstractions;
using Superbrands.Selection.Domain.Selections;

namespace Superbrands.Selection.Domain.Logs.Messages
{
    public class LogMessages : LogEntry
    {
        private LogMessages(string message, long selectionId) : base(message, selectionId)
        {
        }

        public static LogEntry ProductAddedToSelection(ColorModelMeta productMeta, long selectionId)
        {
            var metaSerialized = JsonConvert.SerializeObject(productMeta);
            return new LogEntry($"Product {metaSerialized} added to selection", selectionId);
        }
        
        public static LogEntry ProductRemovedFromSelection(ColorModelMeta productMeta, long selectionId)
        {
            var metaSerialized = JsonConvert.SerializeObject(productMeta);
            return new LogEntry($"Product {metaSerialized} removed from selection", selectionId);
        }
        
        public static LogEntry ColorModelAddedToProduct(ColorModelMeta productMeta, long selectionId, string colorModelSku)
        {
            var metaSerialized = JsonConvert.SerializeObject(productMeta);
            return new LogEntry($"Color model {colorModelSku} added to product {metaSerialized}", selectionId);
        }
        
        public static LogEntry ColorModelDeletedFromProduct(ColorModelMeta productMeta, long selectionId,string colorModelSku)
        {
            var metaSerialized = JsonConvert.SerializeObject(productMeta);
            return new LogEntry($"Color model {colorModelSku} deleted from product {metaSerialized}", selectionId);
        }

        public static LogEntry SizeDeletedFromProduct(ColorModelMeta productMeta, long selectionId,  string sizeSku)
        {
            var metaSerialized = JsonConvert.SerializeObject(productMeta);
            return new LogEntry($"Size {sizeSku} deleted from product {metaSerialized}", selectionId);
        }
    }
}