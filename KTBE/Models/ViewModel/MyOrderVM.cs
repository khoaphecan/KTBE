namespace KTBE.Models.ViewModel
{
    using System.Collections.Generic;

    public class OrderStatusGroup
    {
        public string StatusName { get; set; }
        public int TotalCount { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
        public bool IsExpanded { get; set; }
    }

    public class MyOrderVM
    {
        public string SearchTerm { get; set; }
        public string ActiveStatus { get; set; } = "Tất cả đơn";
        public string ExpandedStatus { get; set; }
        public int TotalUserOrders { get; set; }
        public List<OrderStatusGroup> OrderGroups { get; set; } = new List<OrderStatusGroup>();
    }
}
