CREATE TABLE [dbo].[NZ_Location](
	[LocationId] [int] IDENTITY(10000,1) NOT NULL,
	[LocationName] [nvarchar](200) NOT NULL,
	[CountryCode] [varchar](3) NOT NULL,
	[CountryName] [nvarchar](50) NOT NULL,
	[CityName] [nvarchar](50) NULL,
	[CityID] [int] NULL,
	[Address] [nvarchar](500) NULL,
	[Latitude] [varchar](50) NULL,
	[Longitude] [varchar](50) NULL,
	[Airport] [bit] NULL,
	[AirportCode] [varchar](10) NULL,
	[RailwayStation] [bit] NULL,
	[StateProv] [nvarchar](50) NULL,
	[StateCode] [varchar](20) NULL,
	[PostalCode] [varchar](20) NULL,
	[Telephone] [varchar](50) NULL,
	[Email] [varchar](100) NULL,
	[Fax] [varchar](20) NULL,
	[OpenTime] [varchar](5) NULL,
	[CloseTime] [varchar](5) NULL,
	[OperationTime] [nvarchar](1000) NULL,
	[Supplier] [int] NOT NULL,
	[SuppLocId] [varchar](50) NOT NULL,
	[Vendor] [int] NULL,
	[VendorLocId] [varchar](50) NULL,
	[Status] [int] NOT NULL,
	[Remark] [nvarchar](100) NULL,
	[LocType] [varchar](15) NULL,
	[UpdateTime] [datetime] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[TimeOutFeeInfo] [nvarchar](300) NULL,
	[PickupDropoffInfo] [nvarchar](300) NULL,
	[Categorytypes] [text] NULL,
	[Driverages] [text] NULL,
 CONSTRAINT [PK_NZ_Location] PRIMARY KEY CLUSTERED 
(
	[LocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[NZ_RqLogInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[reqType] [nvarchar](200) NULL,
	[Date] [datetime] NOT NULL,
	[Level] [nvarchar](200) NOT NULL,
	[rqinfo] [nvarchar](max) NOT NULL,
	[rsinfo] [nvarchar](max) NULL,
	[Exception] [nvarchar](4000) NULL,
	[theadId] [nvarchar](200) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[NZ_CategoryTypes](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CategoryTypesId] [int] NULL,
	[VehicleCategoryType] [nvarchar](50) NULL,
	[DisplayOrder] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[NZ_DriverAge](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AgeId] [bigint] NULL,
	[DriverAge] [int] NULL,
	[IsDefault] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[NZ_Order](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Reservationref] [varchar](20) NULL,
	[Reservationno] [varchar](20) NULL,
	[Reservationdocumentno] [varchar](50) NULL,
	[Isonrequest] [bit] NULL,
	[Vendor] [varchar](25) NULL,
	[PickUpDateTime] [datetime] NULL,
	[ReturnDateTime] [datetime] NULL,
	[PickUpLocation] [varchar](10) NULL,
	[ReturnLocation] [varchar](10) NULL,
	[FirstName] [varchar](10) NULL,
	[LastName] [varchar](10) NULL,
	[Email] [varchar](50) NULL,
	[DriverAge] [int] NULL,
	[CountryCode] [varchar](2) NULL,
	[VehiclecategoryId] [int] NULL,
	[VehiclecategorytypeId] [int] NULL,
	[InsuranceId] [int] NULL,
	[ExtrakmsId] [int] NULL,
	[SuppOrderStatus] [varchar](20) NULL,
	[CreateTime] [datetime] NULL,
	[LastModifiy] [datetime] NULL,
 CONSTRAINT [PK_NZ_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


