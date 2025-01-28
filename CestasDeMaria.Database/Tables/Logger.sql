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