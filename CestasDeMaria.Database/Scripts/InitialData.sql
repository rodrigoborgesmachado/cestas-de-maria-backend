CREATE TABLE [dbo].[Logger](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[IsActive] [tinyint] NOT NULL,
	[IsDeleted] [tinyint] NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[AdminId] [bigint] NULL,	
	[ClassName] [nvarchar](255) NULL,
	[MethodName] [nvarchar](255) NULL,
	[MethodSignature] [nvarchar](255) NULL,
	[MethodParameters] [nvarchar](255) NULL,
	[StackTrace] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 

CREATE TABLE [dbo].[Admins]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[IsActive] [tinyint] NOT NULL,
	[IsDeleted] [tinyint] NOT NULL,
	[Username] VARCHAR(50) NOT NULL UNIQUE,
	[Name] VARCHAR(50) NOT NULL,
	[PasswordHash] VARCHAR(255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[MailMessage](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Updated] [datetime] NOT NULL,
	[IsActive] [tinyint] NOT NULL,
	[IsDeleted] [tinyint] NOT NULL,
	[AdminId] [bigint] NOT NULL,
	[Subject] [nvarchar](255) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[To] [nvarchar](255) NOT NULL,
	[CC] [nvarchar](255) NULL,
	[Retries] [int] NOT NULL,
	[MailMessageFamilyStatus] [nvarchar](255) NOT NULL,
	[Message] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
ALTER TABLE [dbo].[MailMessage]  WITH NOCHECK ADD FOREIGN KEY([AdminId])
REFERENCES [dbo].[Admins] ([Id])
GO
ALTER TABLE [dbo].[MailMessage] ADD  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[MailMessage] ADD  DEFAULT (getdate()) FOR [Updated]
GO
ALTER TABLE [dbo].[MailMessage] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[MailMessage] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[MailMessage] ADD  DEFAULT ((0)) FOR [Retries]
GO

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

CREATE TABLE [dbo].[BasketDeliveryStatus]
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

ALTER TABLE [dbo].[BasketDeliveryStatus]  WITH NOCHECK ADD FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Admins] ([Id])
GO

ALTER TABLE [dbo].[BasketDeliveryStatus]  WITH NOCHECK ADD FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[Admins] ([Id])
GO

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

INSERT INTO [Admins] (Created, Updated, IsActive, IsDeleted, Username, Name, PasswordHash)
values (getdate(), getdate(), 1, 0, 'admin@admin.com', 'Admin', '12345')
GO

INSERT INTO FamilyStatus (Created, Updated, IsActive, IsDeleted, CreatedBy, UpdatedBy, Description)
VALUES (getdate(), getdate(), 1, 0, 1, 1, 'Cortado')
GO

INSERT INTO FamilyStatus (Created, Updated, IsActive, IsDeleted, CreatedBy, UpdatedBy, Description)
VALUES (getdate(), getdate(), 1, 0, 1, 1, 'Em Espera')
GO

INSERT INTO FamilyStatus (Created, Updated, IsActive, IsDeleted, CreatedBy, UpdatedBy, Description)
VALUES (getdate(), getdate(), 1, 0, 1, 1, 'Em atendimento')
GO

INSERT INTO FamilyStatus (Created, Updated, IsActive, IsDeleted, CreatedBy, UpdatedBy, Description)
VALUES (getdate(), getdate(), 1, 0, 1, 1, 'Elegível')
GO

INSERT INTO BasketDeliveryStatus (Created, Updated, IsActive, IsDeleted, CreatedBy, UpdatedBy, Description)
VALUES (getdate(), getdate(), 1, 0, 1, 1, 'A solicitar')
GO

INSERT INTO BasketDeliveryStatus (Created, Updated, IsActive, IsDeleted, CreatedBy, UpdatedBy, Description)
VALUES (getdate(), getdate(), 1, 0, 1, 1, 'Contatado')
GO

INSERT INTO BasketDeliveryStatus (Created, Updated, IsActive, IsDeleted, CreatedBy, UpdatedBy, Description)
VALUES (getdate(), getdate(), 1, 0, 1, 1, 'Entregue')
GO

INSERT INTO BasketDeliveryStatus (Created, Updated, IsActive, IsDeleted, CreatedBy, UpdatedBy, Description)
VALUES (getdate(), getdate(), 1, 0, 1, 1, 'Faltou')
GO

