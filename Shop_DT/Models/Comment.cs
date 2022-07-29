using System;
using System.Collections.Generic;

#nullable disable

namespace Shop_DT.Models
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public int? CustomerId { get; set; }
        public string CommentContent { get; set; }
        public int? ProductId { get; set; }
        public string Image { get; set; }
        public DateTime? CommentDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
    }
}
