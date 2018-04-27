USE [umajkla-evid]
GO
/****** Object:  Table [dbo].[APIkeys]    Script Date: 05.07.2017 4:28:09 odp. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[APIkeys](
	[keyId] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[userId] [nvarchar](128) NOT NULL,
	[keyPhrase] [nvarchar](128) NOT NULL,
	[label] [nvarchar](max) NULL,
 CONSTRAINT [PK_APIkeys] PRIMARY KEY CLUSTERED 
(
	[keyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 05.07.2017 4:28:10 odp. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[customers]    Script Date: 05.07.2017 4:28:10 odp. ******/
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
/****** Object:  Table [dbo].[eventPermissions]    Script Date: 05.07.2017 4:28:10 odp. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[eventPermissions](
	[permissionSetId] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[eventId] [uniqueidentifier] NOT NULL,
	[userId] [nvarchar](128) NOT NULL,
	[eventslevel] [int] NOT NULL,
	[customerslevel] [int] NOT NULL,
	[goodslevel] [int] NOT NULL,
	[paymentslevel] [int] NOT NULL,
	[supplieslevel] [int] NOT NULL,
	[transactionslevel] [int] NOT NULL,
 CONSTRAINT [PK_eventPermissions] PRIMARY KEY CLUSTERED 
(
	[permissionSetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[events]    Script Date: 05.07.2017 4:28:10 odp. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[events](
	[eventId] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	[dateFrom] [datetime] NOT NULL,
	[dateTo] [datetime] NOT NULL,
	[description] [nvarchar](max) NOT NULL,
	[locationId] [uniqueidentifier] NULL,
	[createdBy] [nvarchar](128) NULL,
	[created] [datetime] NOT NULL,
	[updated] [datetime] NOT NULL,
 CONSTRAINT [PK_events] PRIMARY KEY CLUSTERED 
(
	[eventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[items]    Script Date: 05.07.2017 4:28:10 odp. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[items](
	[itemId] [uniqueidentifier] NOT NULL,
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
	[itemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[locations]    Script Date: 05.07.2017 4:28:10 odp. ******/
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
	[countrycode] [nvarchar](2) NOT NULL,
	[createdBy] [nvarchar](128) NULL,
	[created] [datetime] NOT NULL,
	[updated] [datetime] NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	[latitude] [numeric](20, 15) NOT NULL,
	[longitude] [numeric](20, 15) NOT NULL,
 CONSTRAINT [PK_locations] PRIMARY KEY CLUSTERED 
(
	[locationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[payments]    Script Date: 05.07.2017 4:28:10 odp. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[payments](
	[paymentId] [uniqueidentifier] NOT NULL,
	[amount] [int] NOT NULL,
	[customerId] [uniqueidentifier] NOT NULL,
	[notes] [nvarchar](max) NULL,
	[processedBy] [nvarchar](128) NULL,
	[updated] [datetime] NOT NULL,
	[created] [datetime] NOT NULL,
 CONSTRAINT [PK_payments] PRIMARY KEY CLUSTERED 
(
	[paymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[supplies]    Script Date: 05.07.2017 4:28:10 odp. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[supplies](
	[supplyId] [uniqueidentifier] NOT NULL,
	[itemId] [uniqueidentifier] NOT NULL,
	[amount] [int] NOT NULL,
	[price] [int] NOT NULL,
	[created] [datetime] NOT NULL,
	[updated] [datetime] NOT NULL,
	[notes] [nvarchar](max) NULL,
	[processedBy] [nvarchar](128) NULL,
 CONSTRAINT [PK_supplies] PRIMARY KEY CLUSTERED 
(
	[supplyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[transactions]    Script Date: 05.07.2017 4:28:10 odp. ******/
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
ALTER TABLE [dbo].[APIkeys] ADD  CONSTRAINT [DF_APIkeys_keyId]  DEFAULT (newid()) FOR [keyId]
GO
ALTER TABLE [dbo].[APIkeys] ADD  CONSTRAINT [DF_APIkeys_label]  DEFAULT ('Default') FOR [label]
GO
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF_customers_customerId]  DEFAULT (newid()) FOR [customerId]
GO
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF_customers_created]  DEFAULT (getdate()) FOR [created]
GO
ALTER TABLE [dbo].[customers] ADD  CONSTRAINT [DF_customers_updated]  DEFAULT (getdate()) FOR [updated]
GO
ALTER TABLE [dbo].[eventPermissions] ADD  CONSTRAINT [DF_eventPermissions_permissionSetId]  DEFAULT (newid()) FOR [permissionSetId]
GO
ALTER TABLE [dbo].[eventPermissions] ADD  CONSTRAINT [DF_eventPermissions_eventslevel]  DEFAULT ((0)) FOR [eventslevel]
GO
ALTER TABLE [dbo].[eventPermissions] ADD  CONSTRAINT [DF_eventPermissions_customerslevel]  DEFAULT ((0)) FOR [customerslevel]
GO
ALTER TABLE [dbo].[eventPermissions] ADD  CONSTRAINT [DF_eventPermissions_goodslevel]  DEFAULT ((0)) FOR [goodslevel]
GO
ALTER TABLE [dbo].[eventPermissions] ADD  CONSTRAINT [DF_eventPermissions_paymentslevel]  DEFAULT ((0)) FOR [paymentslevel]
GO
ALTER TABLE [dbo].[eventPermissions] ADD  CONSTRAINT [DF_eventPermissions_supplieslevel]  DEFAULT ((0)) FOR [supplieslevel]
GO
ALTER TABLE [dbo].[eventPermissions] ADD  CONSTRAINT [DF_eventPermissions_transactionslevel]  DEFAULT ((0)) FOR [transactionslevel]
GO
ALTER TABLE [dbo].[events] ADD  CONSTRAINT [DF_events_eventId]  DEFAULT (newid()) FOR [eventId]
GO
ALTER TABLE [dbo].[events] ADD  CONSTRAINT [DF_events_created]  DEFAULT (getdate()) FOR [created]
GO
ALTER TABLE [dbo].[events] ADD  CONSTRAINT [DF_events_updated]  DEFAULT (getdate()) FOR [updated]
GO
ALTER TABLE [dbo].[items] ADD  CONSTRAINT [DF_goods_goodsId]  DEFAULT (newid()) FOR [itemId]
GO
ALTER TABLE [dbo].[items] ADD  CONSTRAINT [DF_goods_created]  DEFAULT (getdate()) FOR [created]
GO
ALTER TABLE [dbo].[items] ADD  CONSTRAINT [DF_goods_updated]  DEFAULT (getdate()) FOR [updated]
GO
ALTER TABLE [dbo].[locations] ADD  CONSTRAINT [DF_locations_locationId]  DEFAULT (newid()) FOR [locationId]
GO
ALTER TABLE [dbo].[locations] ADD  CONSTRAINT [DF_locations_created]  DEFAULT (getdate()) FOR [created]
GO
ALTER TABLE [dbo].[locations] ADD  CONSTRAINT [DF_locations_updated]  DEFAULT (getdate()) FOR [updated]
GO
ALTER TABLE [dbo].[locations] ADD  CONSTRAINT [DF_locations_name]  DEFAULT ('placeholdername') FOR [name]
GO
ALTER TABLE [dbo].[locations] ADD  CONSTRAINT [DF_locations_latitude]  DEFAULT ((0)) FOR [latitude]
GO
ALTER TABLE [dbo].[locations] ADD  CONSTRAINT [DF_locations_longitude]  DEFAULT ((0)) FOR [longitude]
GO
ALTER TABLE [dbo].[payments] ADD  CONSTRAINT [DF_payments_paymentId]  DEFAULT (newid()) FOR [paymentId]
GO
ALTER TABLE [dbo].[payments] ADD  CONSTRAINT [DF_payments_updated]  DEFAULT (getdate()) FOR [updated]
GO
ALTER TABLE [dbo].[payments] ADD  CONSTRAINT [DF_payments_created]  DEFAULT (getdate()) FOR [created]
GO
ALTER TABLE [dbo].[supplies] ADD  CONSTRAINT [DF_supplies_supplyId]  DEFAULT (newid()) FOR [supplyId]
GO
ALTER TABLE [dbo].[supplies] ADD  CONSTRAINT [DF_supplies_created]  DEFAULT (getdate()) FOR [created]
GO
ALTER TABLE [dbo].[supplies] ADD  CONSTRAINT [DF_supplies_updated]  DEFAULT (getdate()) FOR [updated]
GO
ALTER TABLE [dbo].[transactions] ADD  CONSTRAINT [DF_transactions_transactionId]  DEFAULT (newid()) FOR [transactionId]
GO
ALTER TABLE [dbo].[transactions] ADD  CONSTRAINT [DF_transactions_datetime]  DEFAULT (getdate()) FOR [created]
GO
ALTER TABLE [dbo].[transactions] ADD  CONSTRAINT [DF__tmp_ms_xx__updat__49C3F6B7]  DEFAULT (getdate()) FOR [updated]
GO
ALTER TABLE [dbo].[APIkeys]  WITH CHECK ADD  CONSTRAINT [FK_APIkeys_AspNetUsers] FOREIGN KEY([userId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[APIkeys] CHECK CONSTRAINT [FK_APIkeys_AspNetUsers]
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
ALTER TABLE [dbo].[eventPermissions]  WITH CHECK ADD  CONSTRAINT [FK_eventPermissions_AspNetUsers] FOREIGN KEY([userId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[eventPermissions] CHECK CONSTRAINT [FK_eventPermissions_AspNetUsers]
GO
ALTER TABLE [dbo].[eventPermissions]  WITH CHECK ADD  CONSTRAINT [FK_eventPermissions_events] FOREIGN KEY([eventId])
REFERENCES [dbo].[events] ([eventId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[eventPermissions] CHECK CONSTRAINT [FK_eventPermissions_events]
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
ALTER TABLE [dbo].[items]  WITH CHECK ADD  CONSTRAINT [FK_goods_AspNetUsers] FOREIGN KEY([createdBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[items] CHECK CONSTRAINT [FK_goods_AspNetUsers]
GO
ALTER TABLE [dbo].[items]  WITH CHECK ADD  CONSTRAINT [FK_goods_events] FOREIGN KEY([eventId])
REFERENCES [dbo].[events] ([eventId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[items] CHECK CONSTRAINT [FK_goods_events]
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
REFERENCES [dbo].[items] ([itemId])
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
REFERENCES [dbo].[items] ([itemId])
GO
ALTER TABLE [dbo].[transactions] CHECK CONSTRAINT [FK_transactions_goods]
GO
