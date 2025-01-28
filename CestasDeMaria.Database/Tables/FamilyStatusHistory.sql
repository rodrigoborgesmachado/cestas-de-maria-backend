CREATE TABLE [dbo].[FamilyFamilyStatusHistory]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[IsActive] [tinyint] NOT NULL,
	[IsDeleted] [tinyint] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[UpdatedBy] [bigint] NOT NULL,
	[FamilyId] [bigint] NOT NULL,
	[OldFamilyStatusId] [bigint] NOT NULL,
	[NewFamilyStatusId] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[FamilyFamilyStatusHistory]  WITH NOCHECK ADD FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Admins] ([Id])
GO

ALTER TABLE [dbo].[FamilyFamilyStatusHistory]  WITH NOCHECK ADD FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[Admins] ([Id])
GO

ALTER TABLE [dbo].[FamilyFamilyStatusHistory]  WITH NOCHECK ADD FOREIGN KEY([FamilyId])
REFERENCES [dbo].[Families] ([Id])
GO

ALTER TABLE [dbo].[FamilyFamilyStatusHistory]  WITH NOCHECK ADD FOREIGN KEY([OldFamilyStatusId])
REFERENCES [dbo].[FamilyStatus] ([Id])
GO

ALTER TABLE [dbo].[FamilyFamilyStatusHistory]  WITH NOCHECK ADD FOREIGN KEY([NewFamilyStatusId])
REFERENCES [dbo].[FamilyStatus] ([Id])
GO