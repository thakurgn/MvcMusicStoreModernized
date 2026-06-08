using System.ComponentModel.DataAnnotations;

namespace MvcMusicStoreModernized.Models
{
    public class Cart
    {
        [Key]
        public int RecordId { get; set; }

        public string CartId { get; set; } = string.Empty;

        public int AlbumId { get; set; }

        public int Count { get; set; }

        public DateTime DateCreated { get; set; }

        public Album? Album { get; set; }
    }
}