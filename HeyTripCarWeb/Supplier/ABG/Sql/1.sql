CREATE TABLE [dbo].[Abg_CarProReservation](
	[OrderNo] [nvarchar](200) NOT NULL,
	[ReservationId] [nvarchar](200) NULL,
	[ReservationType] [nvarchar](50) NULL,
	[PickUpDateTime] [datetime] NULL,
	[ReturnDateTime] [datetime] NULL,
	[PickUpLocationCode] [nvarchar](50) NULL,
	[ReturnLocationCode] [nvarchar](50) NULL,
	[CodeContext] [nvarchar](50) NULL,
	[SIPP] [nvarchar](50) NULL,
	[TransmissionType] [nvarchar](50) NULL,
	[AirConditionInd] [nvarchar](50) NULL,
	[DriveType] [nvarchar](50) NULL,
	[PassengerQuantity] [nvarchar](50) NULL,
	[BaggageQuantity] [nvarchar](50) NULL,
	[FuelType] [nvarchar](50) NULL,
	[VehicleCategory] [nvarchar](50) NULL,
	[DoorCount] [nvarchar](50) NULL,
	[VehClass] [nvarchar](50) NULL,
	[CarName] [varchar](255) NULL,
	[PictureURL] [nvarchar](50) NULL,
	[RateDistanceInfo] [nvarchar](200) NULL,
	[RateTotalAmount] [decimal](18, 0) NULL,
	[EstimatedTotalAmount] [decimal](18, 0) NULL,
	[CurrencyCode] [nvarchar](50) NULL,
	[Location_Code] [nvarchar](50) NULL,
	[Location_OtherInfo] [nvarchar](2000) NULL,
	[ReturnLocation_Code] [nvarchar](50) NULL,
	[ReturnLocation_OtherInfo] [nvarchar](2000) NULL,
	[BirthDate] [nvarchar](200) NULL,
	[GivenName] [nvarchar](200) NULL,
	[MiddleName] [nvarchar](200) NULL,
	[Surname] [nvarchar](200) NULL,
	[Email] [nvarchar](50) NULL,
	[Telephone] [nvarchar](50) NULL,
	[AddressInfo] [nvarchar](2000) NULL,
	[DocInfo] [nvarchar](255) NULL,
	[VendorName] [nvarchar](255) NULL,
	[VendorCode] [nvarchar](50) NULL,
	[CreateTime] [datetime] NOT NULL,
	[ConfirmTime] [datetime] NULL,
	[CancelTime] [datetime] NULL,
	[OrderStatus] [nvarchar](50) NULL,
	[Paytype] [nvarchar](200) NULL,
	[ratecode] [varchar](255) NULL,
 CONSTRAINT [PK_ABG_CarProReservation] PRIMARY KEY CLUSTERED 
(
	[OrderNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]



CREATE TABLE [dbo].[Abg_CreditCardPolicy](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AvisLocationCode] [nvarchar](50) NULL,
	[CountryName] [nvarchar](100) NULL,
	[RegionName] [nvarchar](255) NULL,
	[StationNumber] [nvarchar](255) NULL,
	[AvisLocationName] [nvarchar](255) NULL,
	[VehicleModelName] [nvarchar](255) NULL,
	[AvisCarGroup] [nvarchar](255) NULL,
	[VehicleSIPPCode] [nvarchar](255) NULL,
	[NumCreditCardsRequired] [int] NOT NULL,
	[DetailRecordIndicator] [nvarchar](255) NOT NULL,
	[CreatTime] [datetime] NULL,
	[Updatetime] [datetime] NULL,
	[hashkey] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Abg_CreditCardPolicy] ADD  DEFAULT ('1') FOR [DetailRecordIndicator]
GO


GO

CREATE TABLE [dbo].[Abg_Location_New](
	[LocationCode] [varchar](50) NOT NULL,
	[LocationName] [varchar](300) NOT NULL,
	[RegionCode] [varchar](50) NULL,
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
	[hashKey] [nvarchar](255) NULL,
	[operationtimehashkey] [nvarchar](255) NULL,
 CONSTRAINT [PK_[Abg_Location_New] PRIMARY KEY CLUSTERED 
(
	[LocationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Abg_RateCache](
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

ALTER TABLE [dbo].[Abg_RateCache] ADD  CONSTRAINT [DF_ABG_RateCache_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
GO

ALTER TABLE [dbo].[Abg_RateCache] ADD  CONSTRAINT [DF_ABG_RateCache_PreUpdateTime]  DEFAULT (getdate()) FOR [PreUpdateTime]
GO

CREATE TABLE [dbo].[Abg_RqLogInfo](
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



CREATE TABLE [dbo].[Abg_YoungDriver](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NULL,
	[Country] [varchar](50) NULL,
	[Region] [varchar](255) NULL,
	[Station] [varchar](50) NULL,
	[LocationName] [varchar](250) NULL,
	[CarGroup] [nchar](10) NULL,
	[MinimumAge] [int] NULL,
	[MaximumAge] [int] NULL,
	[YoungAge] [int] NULL,
	[EnglandYoungDriverSurcharge] [varchar](255) NULL,
	[SpainYoungDriverSurcharge] [varchar](255) NULL,
	[CzechYoungDriverSurcharge] [varchar](255) NULL,
	[DenmarkYoungDriverSurcharge] [varchar](255) NULL,
	[GermanyYoungDriverSurcharge] [varchar](255) NULL,
	[FranceYoungDriverSurcharge] [varchar](255) NULL,
	[ItalyYoungDriverSurcharge] [varchar](255) NULL,
	[NorwayYoungDriverSurcharge] [varchar](255) NULL,
	[NethlandsYoungDriverSurcharge] [varchar](255) NULL,
	[PortugalYoungDriverSurcharge] [varchar](255) NULL,
	[SwedenYoungDriverSurcharge] [varchar](255) NULL,
	[SlovakYoungDriverSurcharge] [varchar](255) NULL,
	[EnglandMaximumAgeSurcharge] [varchar](255) NULL,
	[SpainMaximumAgeSurcharge] [varchar](255) NULL,
	[CzechMaximumAgeSurcharge] [varchar](255) NULL,
	[DenmarkMaximumAgeSurcharge] [varchar](255) NULL,
	[GermanyMaximumAgeSurcharge] [varchar](255) NULL,
	[FranceMaximumAgeSurcharge] [varchar](255) NULL,
	[ItalyMaximumAgeSurcharge] [varchar](255) NULL,
	[NorwayMaximumAgeSurcharge] [varchar](255) NULL,
	[NetherlandsMaximumAgeSurcharge] [varchar](255) NULL,
	[PortugalMaximumAgeSurcharge] [varchar](255) NULL,
	[SwedenMaximumAgeSurcharge] [varchar](255) NULL,
	[SlovakMaximumAgeSurcharge] [varchar](255) NULL,
	[CreateTime] [datetime] NULL,
	[UpdateTime] [datetime] NULL,
	[hashKey] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
