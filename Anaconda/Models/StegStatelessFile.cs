using System.ComponentModel.DataAnnotations;

namespace Anaconda.Models
{
    public class StegStatelessFile : BaseModel
    {
        public string? FileName { get; set; }
        public string? ImagePath { get; set; }
        public string? ReferenceKey { get; set; }
        public string? Reference { get; set; }
        public Guid UserId { get; set; }
    }
}