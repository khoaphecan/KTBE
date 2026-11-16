namespace KTBE.Models.ViewModel
{
    using System;
    using PagedList;

    public class OrderSearchVM
    {
        public string CustomerName { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? OrderDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public IPagedList<Order> Orders { get; set; }
    }
}
