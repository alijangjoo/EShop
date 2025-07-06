using System.ComponentModel.DataAnnotations;

namespace Web.UI.Models
{
    public class WishlistViewModel
    {
        public List<WishlistItem> Items { get; set; } = new List<WishlistItem>();
        public int TotalItems { get; set; }
        public string ShareUrl { get; set; } = string.Empty;
        public bool IsEmpty => Items.Count == 0;
    }

    public class WishlistItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsInStock { get; set; }
        public DateTime AddedDate { get; set; }
        public string ProductUrl => $"/Product/Details/{ProductId}";
        public string FormattedPrice => $"{Price:N0} تومان";
        public string FormattedDate => AddedDate.ToString("yyyy/MM/dd");
    }

    public class SharedWishlistViewModel
    {
        public List<WishlistItem> Items { get; set; } = new List<WishlistItem>();
        public string SharerName { get; set; } = string.Empty;
        public DateTime ShareDate { get; set; }
        public string ShareMessage { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public bool IsEmpty => Items.Count == 0;
    }

    public class GiftRegistryViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string EventType { get; set; } = string.Empty; // Wedding, Birthday, etc.
        public bool IsPublic { get; set; }
        public bool IsActive { get; set; }
        public List<WishlistItem> Items { get; set; } = new List<WishlistItem>();
        public string ShareUrl { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string FormattedEventDate => EventDate.ToString("yyyy/MM/dd");
        public int TotalItems => Items.Count;
        public decimal TotalValue => Items.Sum(x => x.Price);
        public string FormattedTotalValue => $"{TotalValue:N0} تومان";
    }

    public class CreateGiftRegistryViewModel
    {
        [Required(ErrorMessage = "عنوان لیست هدیه الزامی است")]
        [StringLength(100, ErrorMessage = "عنوان نباید بیشتر از 100 کاراکتر باشد")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "توضیحات نباید بیشتر از 500 کاراکتر باشد")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "تاریخ رویداد الزامی است")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; } = DateTime.Now.AddDays(30);

        [Required(ErrorMessage = "نوع رویداد الزامی است")]
        [StringLength(50, ErrorMessage = "نوع رویداد نباید بیشتر از 50 کاراکتر باشد")]
        public string EventType { get; set; } = string.Empty;

        public bool IsPublic { get; set; } = true;
        public bool IsActive { get; set; } = true;
    }
}