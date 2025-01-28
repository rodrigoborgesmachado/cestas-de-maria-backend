CREATE TABLE [dbo].[Families]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[IsActive] [tinyint] NOT NULL,
	[IsDeleted] [tinyint] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[UpdatedBy] [bigint] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[BasketQuantity] [int] NOT NULL,
	[Phone] [varchar](255) NOT NULL,
	[Document] [varchar](255) NOT NULL,
	[Adults] [int] NOT NULL,
	[Children] [int] NOT NULL,
	[IsSocialProgramBeneficiary] [tinyint] NOT NULL,
	[IsFromLocal] [tinyint] NOT NULL,
	[HousingSituation] [varchar](255) NOT NULL,
	[HasSevereLimitation] [tinyint] NOT NULL,
	[Neighborhood] [varchar](255) NOT NULL,
	[Address] [varchar](255) NOT NULL,
	[FamilyStatusId] [bigint] NOT NULL,
	[DeliveryWeek] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Families]  WITH NOCHECK ADD FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Admins] ([Id])
GO

ALTER TABLE [dbo].[Families]  WITH NOCHECK ADD FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[Admins] ([Id])
GO

ALTER TABLE [dbo].[Families]  WITH NOCHECK ADD FOREIGN KEY([FamilyStatusId])
REFERENCES [dbo].[FamilyStatus] ([Id])
GO