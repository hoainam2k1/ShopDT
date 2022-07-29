using System;
using System.Collections.Generic;

#nullable disable

namespace Shop_DT.Models
{
    public partial class Product
    {
        public Product()
        {
            Comments = new HashSet<Comment>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        public int? CatId { get; set; }
        public int? Price { get; set; }
        public int? Discount { get; set; }
        public string Thumb { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? DateModified { get; set; }
        public bool Active { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public int? UnitStock { get; set; }

        public virtual Category Cat { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
