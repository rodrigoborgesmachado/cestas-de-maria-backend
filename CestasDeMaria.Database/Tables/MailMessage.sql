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