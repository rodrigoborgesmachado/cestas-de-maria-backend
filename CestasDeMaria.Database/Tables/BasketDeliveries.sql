CREATE TABLE [dbo].[BasketDeliveries]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[IsActive] [tinyint] NOT NULL,
	[IsDeleted] [tinyint] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[UpdatedBy] [bigint] NOT NULL,
	[FamilyId] [bigint] NOT NULL,
	[DeliveryStatusId] [bigint] NOT NULL,
	[WeekOfMonth] [int] not null,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BasketDeliveries]  WITH NOCHECK ADD FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Admins] ([Id])
GO

ALTER TABLE [dbo].[BasketDeliveries]  WITH NOCHECK ADD FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[Admins] ([Id])
GO

ALTER TABLE [dbo].[BasketDeliveries]  WITH NOCHECK ADD FOREIGN KEY([FamilyId])
REFERENCES [dbo].[Families] ([Id])
GO

ALTER TABLE [dbo].[BasketDeliveries]  WITH NOCHECK ADD FOREIGN KEY([DeliveryStatusId])
REFERENCES [dbo].[BasketDeliveryStatus] ([Id])
GO
