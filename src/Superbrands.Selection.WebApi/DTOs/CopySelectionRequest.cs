using System.ComponentModel.DataAnnotations;

namespace Superbrands.Selection.WebApi.DTOs
{
    public class CopySelectionRequest
    {
        [Required]
        public long[] TargetSelections { get; set; }
        public long[] ProductsToCopy { get; set; }
    }
}