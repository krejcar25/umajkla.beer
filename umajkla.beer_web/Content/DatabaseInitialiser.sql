USE [umajkla-evid]
GO
/****** Object:  Table [dbo].[AspNetUserDetails]    Script Date: 12.03.2017 8:38:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserDetails](
	[userId] [nvarchar](128) NOT NULL,
	[firstName] [nvarchar](255) NOT NULL,
	[middleNames] [nvarchar](255) NULL,
	[lastName] [nvarchar](255) NOT NULL,
	[phone] [nvarchar](255) NULL,
	[landline] [nvarchar](255) NULL,
	[address1] [nvarchar](255) NOT NULL,
	[address2] [nvarchar](255) NULL,
	[city] [nvarchar](255) NOT NULL,
	[postcode] [nvarchar](255) NOT NULL,
	[country] [nvarchar](255) NOT NULL,
	[nickname] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_AspNetUserDetails] PRIMARY KEY CLUSTERED 
(
	[userId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[customers]    Script Date: 12.03.2017 8:38:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[customers](
	[customerId] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[address] [nvarchar](max) NOT NULL,
	[phone] [nvarchar](255) NOT NULL,
	[email] [nvarchar](255) NOT NULL,
	[created] [datetime] NOT NULL,
	[updated] [datetime] NOT NULL,
	[notes] [nvarchar](max) NULL,
	[eventId] [uniqueidentifier] NULL,
	[createdBy] [nvarchar](128) NULL,
 CONSTRAINT [PK_customers] PRIMARY KEY CLUSTERED 
(
	[customerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[events]    Script Date: 12.03.2017 8:38:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[events](
	[eventId] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	[datefrom] [datetime] NOT NULL,
	[dateto] [datetime] NOT NULL,
	[description] [nvarchar](max) NOT NULL,
	[locationId] [uniqueidentifier] NULL,
	[createdBy] [nvarchar](128) NULL,
 CONSTRAINT [PK_events] PRIMARY KEY CLUSTERED 
(
	[eventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[goods]    Script Date: 12.03.2017 8:38:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[goods](
	[goodsId] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[price] [int] NOT NULL,
	[unit] [nvarchar](255) NOT NULL,
	[created] [datetime] NOT NULL,
	[updated] [datetime] NOT NULL,
	[notes] [nvarchar](max) NULL,
	[eventId] [uniqueidentifier] NULL,
	[createdBy] [nvarchar](128) NULL,
 CONSTRAINT [PK_goods] PRIMARY KEY CLUSTERED 
(
	[goodsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[locations]    Script Date: 12.03.2017 8:38:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[locations](
	[locationId] [uniqueidentifier] NOT NULL,
	[street1] [nvarchar](max) NOT NULL,
	[street2] [nvarchar](max) NULL,
	[city] [nvarchar](max) NOT NULL,
	[postcode] [nvarchar](255) NOT NULL,
	[country] [nvarchar](max) NOT NULL,
	[createdBy] [nvarchar](128) NULL,
 CONSTRAINT [PK_locations] PRIMARY KEY CLUSTERED 
(
	[locationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[payments]    Script Date: 12.03.2017 8:38:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[payments](
	[paymentId] [uniqueidentifier] NOT NULL,
	[amount] [int] NOT NULL,
	[customerId] [uniqueidentifier] NOT NULL,
	[datetime] [datetime] NULL,
	[notes] [nvarchar](max) NULL,
	[processedBy] [nvarchar](128) NULL,
 CONSTRAINT [PK_payments] PRIMARY KEY CLUSTERED 
(
	[paymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[supplies]    Script Date: 12.03.2017 8:38:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[supplies](
	[supplyId] [uniqueidentifier] NOT NULL,
	[itemId] [uniqueidentifier] NULL,
	[amount] [int] NOT NULL,
	[created] [datetime] NOT NULL,
	[modified] [datetime] NOT NULL,
	[notes] [nvarchar](max) NULL,
	[processedBy] [nvarchar](128) NULL,
 CONSTRAINT [PK_supplies] PRIMARY KEY CLUSTERED 
(
	[supplyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[transactions]    Script Date: 12.03.2017 8:38:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[transactions](
	[transactionId] [uniqueidentifier] NOT NULL,
	[customerId] [uniqueidentifier] NULL,
	[itemId] [uniqueidentifier] NULL,
	[amount] [int] NOT NULL,
	[multiplier] [int] NOT NULL,
	[created] [datetime] NOT NULL,
	[updated] [datetime] NOT NULL,
	[notes] [nvarchar](max) NULL,
	[processedBy] [nvarchar](128) NULL,
 CONSTRAINT [PK_transactions] PRIMARY KEY CLUSTERED 
(
	[transactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF_customers_customerId]  DEFAULT (newid()) FOR [customerId]
GO
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF_customers_created]  DEFAULT (getdate()) FOR [created]
GO
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF_customers_updated]  DEFAULT (getdate()) FOR [updated]
GO
ALTER TABLE [dbo].[events] ADD  CONSTRAINT [DF_events_eventId]  DEFAULT (newid()) FOR [eventId]
GO
ALTER TABLE [dbo].[goods] ADD  CONSTRAINT [DF_goods_goodsId]  DEFAULT (newid()) FOR [goodsId]
GO
ALTER TABLE [dbo].[goods] ADD  CONSTRAINT [DF_goods_created]  DEFAULT (getdate()) FOR [created]
GO
ALTER TABLE [dbo].[goods] ADD  CONSTRAINT [DF_goods_updated]  DEFAULT (getdate()) FOR [updated]
GO
ALTER TABLE [dbo].[locations] ADD  CONSTRAINT [DF_locations_locationId]  DEFAULT (newid()) FOR [locationId]
GO
ALTER TABLE [dbo].[payments] ADD  CONSTRAINT [DF_payments_paymentId]  DEFAULT (newid()) FOR [paymentId]
GO
ALTER TABLE [dbo].[payments] ADD  CONSTRAINT [DF_payments_datetime]  DEFAULT (getdate()) FOR [datetime]
GO
ALTER TABLE [dbo].[supplies] ADD  CONSTRAINT [DF_supplies_supplyId]  DEFAULT (newid()) FOR [supplyId]
GO
ALTER TABLE [dbo].[transactions] ADD  CONSTRAINT [DF_transactions_transactionId]  DEFAULT (newid()) FOR [transactionId]
GO
ALTER TABLE [dbo].[transactions] ADD  CONSTRAINT [DF_transactions_datetime]  DEFAULT (getdate()) FOR [created]
GO
ALTER TABLE [dbo].[transactions] ADD  CONSTRAINT [DF__tmp_ms_xx__updat__49C3F6B7]  DEFAULT (getdate()) FOR [updated]
GO
ALTER TABLE [dbo].[AspNetUserDetails]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserDetails_AspNetUsers] FOREIGN KEY([userId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[AspNetUserDetails] CHECK CONSTRAINT [FK_AspNetUserDetails_AspNetUsers]
GO
ALTER TABLE [dbo].[customers]  WITH CHECK ADD  CONSTRAINT [FK_customers_AspNetUsers] FOREIGN KEY([createdBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[customers] CHECK CONSTRAINT [FK_customers_AspNetUsers]
GO
ALTER TABLE [dbo].[customers]  WITH CHECK ADD  CONSTRAINT [FK_customers_events] FOREIGN KEY([eventId])
REFERENCES [dbo].[events] ([eventId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[customers] CHECK CONSTRAINT [FK_customers_events]
GO
ALTER TABLE [dbo].[events]  WITH CHECK ADD  CONSTRAINT [FK_events_AspNetUsers] FOREIGN KEY([createdBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[events] CHECK CONSTRAINT [FK_events_AspNetUsers]
GO
ALTER TABLE [dbo].[events]  WITH CHECK ADD  CONSTRAINT [FK_events_locations] FOREIGN KEY([locationId])
REFERENCES [dbo].[locations] ([locationId])
GO
ALTER TABLE [dbo].[events] CHECK CONSTRAINT [FK_events_locations]
GO
ALTER TABLE [dbo].[goods]  WITH CHECK ADD  CONSTRAINT [FK_goods_AspNetUsers] FOREIGN KEY([createdBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[goods] CHECK CONSTRAINT [FK_goods_AspNetUsers]
GO
ALTER TABLE [dbo].[goods]  WITH CHECK ADD  CONSTRAINT [FK_goods_events] FOREIGN KEY([eventId])
REFERENCES [dbo].[events] ([eventId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[goods] CHECK CONSTRAINT [FK_goods_events]
GO
ALTER TABLE [dbo].[locations]  WITH CHECK ADD  CONSTRAINT [FK_locations_AspNetUsers] FOREIGN KEY([createdBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[locations] CHECK CONSTRAINT [FK_locations_AspNetUsers]
GO
ALTER TABLE [dbo].[payments]  WITH CHECK ADD  CONSTRAINT [FK_payments_AspNetUsers] FOREIGN KEY([processedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[payments] CHECK CONSTRAINT [FK_payments_AspNetUsers]
GO
ALTER TABLE [dbo].[payments]  WITH CHECK ADD  CONSTRAINT [FK_payments_customers] FOREIGN KEY([customerId])
REFERENCES [dbo].[customers] ([customerId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[payments] CHECK CONSTRAINT [FK_payments_customers]
GO
ALTER TABLE [dbo].[supplies]  WITH CHECK ADD  CONSTRAINT [FK_supplies_AspNetUsers] FOREIGN KEY([processedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[supplies] CHECK CONSTRAINT [FK_supplies_AspNetUsers]
GO
ALTER TABLE [dbo].[supplies]  WITH CHECK ADD  CONSTRAINT [FK_supplies_goods] FOREIGN KEY([itemId])
REFERENCES [dbo].[goods] ([goodsId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[supplies] CHECK CONSTRAINT [FK_supplies_goods]
GO
ALTER TABLE [dbo].[transactions]  WITH CHECK ADD  CONSTRAINT [FK_transactions_AspNetUsers] FOREIGN KEY([processedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[transactions] CHECK CONSTRAINT [FK_transactions_AspNetUsers]
GO
ALTER TABLE [dbo].[transactions]  WITH CHECK ADD  CONSTRAINT [FK_transactions_customers] FOREIGN KEY([customerId])
REFERENCES [dbo].[customers] ([customerId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[transactions] CHECK CONSTRAINT [FK_transactions_customers]
GO
ALTER TABLE [dbo].[transactions]  WITH CHECK ADD  CONSTRAINT [FK_transactions_goods] FOREIGN KEY([itemId])
REFERENCES [dbo].[goods] ([goodsId])
GO
ALTER TABLE [dbo].[transactions] CHECK CONSTRAINT [FK_transactions_goods]
GO
