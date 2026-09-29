namespace KanvasProje.Core.Models
{
    public class AdminDashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal DailyRevenue { get; set; }
        public decimal WeeklyRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TodayOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ReadyToShipOrders { get; set; }
        public int ShippedOrders { get; set; }
        public int CancelledOrReturnedOrders { get; set; }
        public int ActiveProductCount { get; set; }
        public int LowStockProductCount { get; set; }
        public int UnreadContactCount { get; set; }
        public int TotalVisitsLast7Days { get; set; }
        public int UniqueVisitorsLast7Days { get; set; }
        public IReadOnlyList<string> RevenueChartLabels { get; set; } = Array.Empty<string>();
        public IReadOnlyList<decimal> RevenueChartValues { get; set; } = Array.Empty<decimal>();
        public IReadOnlyList<DashboardProductStatItem> TopSellingProducts { get; set; } = Array.Empty<DashboardProductStatItem>();
        public IReadOnlyList<DashboardProductStatItem> MostViewedProducts { get; set; } = Array.Empty<DashboardProductStatItem>();
        public IReadOnlyList<DashboardLowStockItem> LowStockProducts { get; set; } = Array.Empty<DashboardLowStockItem>();
        public IReadOnlyList<DashboardRecentEntityItem> RecentProducts { get; set; } = Array.Empty<DashboardRecentEntityItem>();
        public IReadOnlyList<DashboardReviewItem> RecentReviews { get; set; } = Array.Empty<DashboardReviewItem>();
        public IReadOnlyList<DashboardActivityItem> RecentActivities { get; set; } = Array.Empty<DashboardActivityItem>();
        public IReadOnlyList<DashboardAlertItem> Alerts { get; set; } = Array.Empty<DashboardAlertItem>();
    }

    public class DashboardProductStatItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string AmountLabel { get; set; } = string.Empty;
    }

    public class DashboardLowStockItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public int Stock { get; set; }
        public bool PreorderOpen { get; set; }
    }

    public class DashboardRecentEntityItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class DashboardReviewItem
    {
        public int ReviewId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool Approved { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DashboardActivityItem
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
    }

    public class DashboardAlertItem
    {
        public string Severity { get; set; } = "info";
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
    }
}
