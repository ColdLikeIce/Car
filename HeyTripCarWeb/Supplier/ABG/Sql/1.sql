--  预支付表
CREATE TABLE [dbo].[ABG_CarProReservation](
	[OrderNo] [nvarchar](200) NOT NULL,
	[ReservationId] [nvarchar](200) NULL,
	[ReservationType] [nvarchar](255) NULL,
	[PickUpDateTime] [datetime] NULL,
	[ReturnDateTime] [datetime] NULL,
	[PickUpLocationCode] [nvarchar](255) NULL,
	[ReturnLocationCode] [nvarchar](255) NULL,
	[CodeContext] [nvarchar](255) NULL,
	[SIPP] [nvarchar](255) NULL,
	[TransmissionType] [nvarchar](255) NULL,
	[AirConditionInd] [nvarchar](255) NULL,
	[DriveType] [nvarchar](255) NULL,
	[PassengerQuantity] [nvarchar](255) NULL,
	[BaggageQuantity] [nvarchar](255) NULL,
	[FuelType] [nvarchar](255) NULL,
	[VehicleCategory] [nvarchar](255) NULL,
	[DoorCount] [nvarchar](255) NULL,
	[VehClass] [nvarchar](255) NULL,
	[CarName] [varchar](255) NULL,
	[PictureURL] [nvarchar](255) NULL,
	[RateDistanceInfo] [nvarchar](200) NULL,
	[RateTotalAmount] [decimal](18, 0) NULL,
	[EstimatedTotalAmount] [decimal](18, 0) NULL,
	[CurrencyCode] [nvarchar](255) NULL,
	[Location_Code] [nvarchar](255) NULL,
	[Location_OtherInfo] [nvarchar](max) NULL,
	[ReturnLocation_Code] [nvarchar](255) NULL,
	[ReturnLocation_OtherInfo] [nvarchar](max) NULL,
	[BirthDate] [nvarchar](200) NULL,
	[GivenName] [nvarchar](200) NULL,
	[MiddleName] [nvarchar](200) NULL,
	[Surname] [nvarchar](200) NULL,
	[Email] [nvarchar](255) NULL,
	[Telephone] [nvarchar](255) NULL,
	[AddressInfo] [nvarchar](max) NULL,
	[DocInfo] [nvarchar](255) NULL,
	[VendorName] [nvarchar](255) NULL,
	[VendorCode] [nvarchar](255) NULL,
	[CreateTime] [datetime] NOT NULL,
	[ConfirmTime] [datetime] NULL,
	[CancelTime] [datetime] NULL,
	[OrderStatus] [nvarchar](255) NULL,
	[Paytype] [nvarchar](200) NULL,
	[ratecode] [varchar](255) NULL,
 CONSTRAINT [PK_ABG_CarProReservation] PRIMARY KEY CLUSTERED 
(
	[OrderNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--  门店表
CREATE TABLE [dbo].[Abg_Location_New](
	[LocationCode] [varchar](50) NOT NULL,
	[LocationName] [varchar](300) NOT NULL,
	[RegionCode] [varchar](50) NOT NULL,
	[regionname] [varchar](255) NULL,
	[rentalType] [varchar](255) NULL,
	[Latitude] [varchar](255) NULL,
	[Longitude] [varchar](255) NULL,
	[PhoneNumber] [varchar](100) NULL,
	[AlternativePhoneNumber] [varchar](255) NULL,
	[Geoindicator] [varchar](255) NULL,
	[outsideReturn] [varchar](255) NULL,
	[HasSkiRack] [varchar](100) NULL,
	[HasSnowTyres] [varchar](100) NULL,
	[HasSnowChains] [varchar](100) NULL,
	[HasChildSeat] [varchar](100) NULL,
	[HasRoofLuggage] [varchar](100) NULL,
	[HasHandControl] [varchar](100) NULL,
	[IsGPS] [varchar](100) NULL,
	[AvisPreferred] [varchar](255) NULL,
	[ShuttleServiceAvailable] [varchar](255) NULL,
	[RoadServiceAvailable] [varchar](100) NULL,
	[Email] [varchar](100) NULL,
	[address] [varchar](100) NULL,
	[City] [varchar](100) NULL,
	[Postcode] [varchar](100) NULL,
	[APOCode] [varchar](100) NULL,
	[CollectionAvailable] [varchar](100) NULL,
	[VendorCode] [varchar](10) NULL,
	[VendorId] [varchar](50) NULL,
	[VendorName] [nvarchar](100) NULL,
	[CreateTime] [datetime] NULL,
	[UpdateTime] [datetime] NULL,
	[IsDeleted] [int] NOT NULL,
	[OperationTimes] [nvarchar](3000) NULL,
 CONSTRAINT [PK_[Abg_Location_New] PRIMARY KEY CLUSTERED 
(
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

-- 缓存表

CREATE TABLE [dbo].[ABG_RateCache](
	[SearchMD5] [varchar](16) NOT NULL,
	[SearchKey] [nvarchar](500) NULL,
	[SearchCount] [int] NOT NULL,
	[RateMD5] [varchar](16) NOT NULL,
	[RateCache] [nvarchar](max) NOT NULL,
	[CanSaleCount] [int] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[PreUpdateTime] [datetime] NOT NULL,
	[ExpireTime] [datetime] NOT NULL,
 CONSTRAINT [PK_ABG_RateCache] PRIMARY KEY CLUSTERED 
(
	[SearchMD5] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ABG_RateCache] ADD  CONSTRAINT [DF_ABG_RateCache_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
GO

ALTER TABLE [dbo].[ABG_RateCache] ADD  CONSTRAINT [DF_ABG_RateCache_PreUpdateTime]  DEFAULT (getdate()) FOR [PreUpdateTime]
GO

-- 供应商请求原始数据日志表

CREATE TABLE [dbo].[Abg_SupplierRqLogInfo](
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



