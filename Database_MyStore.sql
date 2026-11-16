-- ==============================================================
-- DATABASE INITIALIZATION SCRIPT: MyStore
-- Project: ASP.NET MVC5 (.NET Framework 4.7.2) + EF6 Database First
-- ==============================================================

IF DB_ID('MyStore') IS NULL
BEGIN
    CREATE DATABASE [MyStore];
END
GO

USE [MyStore]
GO

-- 1. Bảng User: Quản lý tài khoản Admin và Customer
IF OBJECT_ID('dbo.OrderDetail', 'U') IS NOT NULL DROP TABLE dbo.OrderDetail;
IF OBJECT_ID('dbo.Order', 'U') IS NOT NULL DROP TABLE dbo.[Order];
IF OBJECT_ID('dbo.Customer', 'U') IS NOT NULL DROP TABLE dbo.Customer;
IF OBJECT_ID('dbo.[User]', 'U') IS NOT NULL DROP TABLE dbo.[User];
IF OBJECT_ID('dbo.Product', 'U') IS NOT NULL DROP TABLE dbo.Product;
IF OBJECT_ID('dbo.Category', 'U') IS NOT NULL DROP TABLE dbo.Category;
GO

CREATE TABLE [dbo].[User] (
    [Username] NVARCHAR (255) NOT NULL,
    [Password] NVARCHAR (255) NOT NULL,
    [UserRole] NVARCHAR (50)  NOT NULL, -- 'Admin', 'Customer'
    PRIMARY KEY CLUSTERED ([Username] ASC)
);
GO

-- 2. Bảng Customer: Lưu thông tin chi tiết khách hàng
CREATE TABLE [dbo].[Customer] (
    [CustomerID]      INT IDENTITY (1, 1) NOT NULL,
    [CustomerName]    NVARCHAR (MAX) NOT NULL,
    [CustomerPhone]   NVARCHAR (15)  NOT NULL,
    [CustomerEmail]   NVARCHAR (MAX) NULL,
    [CustomerAddress] NVARCHAR (MAX) NULL,
    [Username]        NVARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([CustomerID] ASC),
    CONSTRAINT [FK_User_Customer] FOREIGN KEY ([Username]) REFERENCES [dbo].[User] ([Username])
);
GO

-- 3. Bảng Category: Phân loại danh mục sản phẩm
CREATE TABLE [dbo].[Category] (
    [CategoryID]   INT IDENTITY (1, 1) NOT NULL,
    [CategoryName] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([CategoryID] ASC)
);
GO

-- 4. Bảng Product: Lưu thông tin sản phẩm
CREATE TABLE [dbo].[Product] (
    [ProductID]         INT IDENTITY (1, 1) NOT NULL,
    [CategoryID]        INT NOT NULL,
    [ProductName]       NVARCHAR (MAX)  NOT NULL,
    [ProductDecription] NVARCHAR (MAX)  NOT NULL,
    [ProductPrice]      DECIMAL (18, 2) NOT NULL,
    [ProductImage]      NVARCHAR (MAX)  NULL,
    PRIMARY KEY CLUSTERED ([ProductID] ASC),
    CONSTRAINT [FK_Pro_Category] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Category] ([CategoryID])
);
GO

-- 5. Bảng Order: Lưu thông tin đơn đặt hàng
CREATE TABLE [dbo].[Order] (
    [OrderID]         INT IDENTITY (1, 1) NOT NULL,
    [CustomerID]      INT NOT NULL,
    [OrderDate]       DATETIME NOT NULL,
    [TotalAmount]     DECIMAL (18, 2) NOT NULL,
    [PaymentStatus]   NVARCHAR (MAX) NULL, -- 'Chưa thanh toán', 'Thanh toán tiền mặt', 'Đã thanh toán'
    [PaymentMethod]   NVARCHAR (MAX) NULL, -- 'Tiền mặt', 'Chuyển khoản', 'PayPal'
    [ShippingMethod]  NVARCHAR (MAX) NULL, -- 'Giao hàng nhanh', 'Giao hàng tiết kiệm'
    [ShippingAddress] NVARCHAR (MAX) NOT NULL,
    [OrderStatus]     NVARCHAR (MAX) NULL, -- 'Chờ thanh toán', 'Đang xử lý', 'Đang vận chuyển', 'Đã giao', 'Đã hủy'
    PRIMARY KEY CLUSTERED ([OrderID] ASC),
    CONSTRAINT [FK_Customer_Order] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([CustomerID])
);
GO

-- 6. Bảng OrderDetail: Lưu chi tiết các sản phẩm trong đơn hàng
CREATE TABLE [dbo].[OrderDetail] (
    [ID]         INT IDENTITY (1, 1) NOT NULL,
    [ProductID]  INT NOT NULL,
    [OrderID]    INT NOT NULL,
    [Quantity]   INT NOT NULL,
    [UnitPrice]  DECIMAL (18, 2) NOT NULL,
    [TotalPrice] AS (Quantity * UnitPrice) PERSISTED,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Pro_OrderDetail] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product] ([ProductID]),
    CONSTRAINT [FK_Order_OrderDetail] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([OrderID])
);
GO

-- ==============================================================
-- SEED DATA CƠ BẢN (NỀN TẢNG)
-- ==============================================================

-- Tài khoản quản trị duy nhất (Admin)
INSERT INTO [dbo].[User] ([Username], [Password], [UserRole]) VALUES 
(N'admin', N'123456', N'Admin');
GO

-- Danh mục sản phẩm
INSERT INTO [dbo].[Category] ([CategoryName]) VALUES
(N'Sách Kỹ Năng - Kinh Doanh'),
(N'Sách Văn Học - Tiểu Thuyết'),
(N'Sách Công Nghệ - Lập Trình');
GO

-- Sản phẩm sách mẫu kèm ảnh
INSERT INTO [dbo].[Product] ([CategoryID], [ProductName], [ProductDecription], [ProductPrice], [ProductImage]) VALUES
(1, N'Trò Chơi Của Những Kẻ Hiểu Luật', N'Chiến lược tư duy để làm chủ cuộc chơi tài chính', 106548.00, N'~/Content/images/tro-choi.png'),
(1, N'Nghệ Thuật Từ Chối – Cách Nói Không Mà Vẫn Có Được Đồng Thuận', N'Kỹ năng giao tiếp và đàm phán trong công việc', 55900.00, N'~/Content/images/nghe-thuat-tu-choi.png'),
(1, N'Kinh Doanh Thời Trang - 8 Bước Xây Dựng, Vận Hành Và Phát Triển Shop Quần Áo Của Bạn', N'Cẩm nang mở shop thời trang thực chiến', 142000.00, N'~/Content/images/kinh-doanh-thoi-trang.png'),
(1, N'LÀM GIÀU TỪ SIÊU CỔ PHIẾU – Câu Chuyện Của Gã Du Mục Phố Wall – JESSE C. STINE', N'Bí quyết đầu tư chứng khoán đỉnh cao', 399000.00, N'~/Content/images/sieu-co-phieu.png'),
(2, N'Hãy Chăm Sóc Mẹ (Tái Bản 2019)', N'Tiểu thuyết cảm động về tình mẫu tử của Shin Kyung-sook', 67000.00, N'~/Content/images/hay-cham-soc-me.png'),
(2, N'Cây Cam Ngọt Của Tôi', N'Tác phẩm kinh điển về tuổi thơ nghèo khó và giàu cảm xúc', 73800.00, N'~/Content/images/cay-cam-ngot.png'),
(3, N'Lập trình Web với ASP.NET MVC5', N'Giáo trình thực hành xây dựng ứng dụng web thương mại điện tử', 185000.00, N'~/Content/images/aspnet-mvc.png'),
(3, N'Entity Framework 6 Toàn Tập', N'Làm chủ ORM Entity Framework trong .NET', 150000.00, N'~/Content/images/ef6.png');
GO
