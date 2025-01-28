CREATE TABLE [dbo].[FamilyStatus]
(
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[IsActive] [tinyint] NOT NULL,
	[IsDeleted] [tinyint] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[UpdatedBy] [bigint] NOT NULL,
    [Description] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
    [Id] ASC
) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[FamilyStatus]  WITH NOCHECK ADD FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Admins] ([Id])
GO

ALTER TABLE [dbo].[FamilyStatus]  WITH NOCHECK ADD FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[Admins] ([Id])
GO