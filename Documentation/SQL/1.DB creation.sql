USE [RapidPay]
GO
ALTER TABLE [dbo].[CardPayment] DROP CONSTRAINT [DF_CardTransactions_IsCompleted]
GO
/****** Object:  Table [dbo].[UserToken]    Script Date: 2/17/2022 9:09:40 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserToken]') AND type in (N'U'))
DROP TABLE [dbo].[UserToken]
GO
/****** Object:  Table [dbo].[User]    Script Date: 2/17/2022 9:09:40 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type in (N'U'))
DROP TABLE [dbo].[User]
GO
/****** Object:  Table [dbo].[Fee]    Script Date: 2/17/2022 9:09:40 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Fee]') AND type in (N'U'))
DROP TABLE [dbo].[Fee]
GO
/****** Object:  Table [dbo].[CreditCard]    Script Date: 2/17/2022 9:09:40 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreditCard]') AND type in (N'U'))
DROP TABLE [dbo].[CreditCard]
GO
/****** Object:  Table [dbo].[CardPayment]    Script Date: 2/17/2022 9:09:40 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CardPayment]') AND type in (N'U'))
DROP TABLE [dbo].[CardPayment]
GO
/****** Object:  User [userRapidPay]    Script Date: 2/17/2022 9:09:40 PM ******/
DROP USER [userRapidPay]
GO
USE [master]
GO
/****** Object:  Login [userRapidPay]    Script Date: 2/17/2022 9:09:40 PM ******/
DROP LOGIN [userRapidPay]
GO
/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [userRapidPay]    Script Date: 2/17/2022 9:09:40 PM ******/
CREATE LOGIN [userRapidPay] WITH PASSWORD=N'hLa4kFbkzesXEzp/ks3+kAt3naDALrDZY+Q61zIGjlk=', DEFAULT_DATABASE=[RapidPay], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [userRapidPay] DISABLE
GO
USE [RapidPay]
GO
/****** Object:  User [userRapidPay]    Script Date: 2/17/2022 9:09:40 PM ******/
CREATE USER [userRapidPay] FOR LOGIN [userRapidPay] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [userRapidPay]
GO
/****** Object:  Table [dbo].[CardPayment]    Script Date: 2/17/2022 9:09:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CardPayment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CardId] [int] NOT NULL,
	[PaymentDate] [datetime] NOT NULL,
	[Amount] [float] NOT NULL,
	[IsCompleted] [bit] NOT NULL,
 CONSTRAINT [PK_CardTransactions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[CardPayment] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[CreditCard]    Script Date: 2/17/2022 9:09:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CreditCard](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CardNumber] [varchar](15) NOT NULL,
	[InitalBalance] [float] NOT NULL,
	[CurrentBalance] [float] NOT NULL,
 CONSTRAINT [PK_CreditCard] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[CreditCard] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[Fee]    Script Date: 2/17/2022 9:09:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Fee](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TimeFrom] [datetime] NOT NULL,
	[TimeTo] [datetime] NOT NULL,
	[CurrentFee] [float] NOT NULL,
 CONSTRAINT [PK_Fee] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[Fee] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[User]    Script Date: 2/17/2022 9:09:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NULL,
	[Username] [varchar](100) NOT NULL,
	[Password] [varchar](max) NOT NULL,
	[DocumentNumber] [varchar](20) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[User] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[UserToken]    Script Date: 2/17/2022 9:09:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserToken](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Token] [varchar](max) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ExpirationTime] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_UserToken] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER AUTHORIZATION ON [dbo].[UserToken] TO  SCHEMA OWNER 
GO
ALTER TABLE [dbo].[CardPayment] ADD  CONSTRAINT [DF_CardTransactions_IsCompleted]  DEFAULT ((0)) FOR [IsCompleted]
GO
