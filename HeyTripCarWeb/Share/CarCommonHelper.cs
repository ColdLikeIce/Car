using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using XiWan.Car.BusinessShared.Enums;
using XiWan.Car.BusinessShared.Public;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Share
{
    public static class CarCommonHelper
    {
        public static bool HasItems<T>(this List<T> data)
        {
            if (data != null && data.Count > 0)
                return true;
            return false;
        }

        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

        /// <summary>
        /// 租车官网域名
        /// </summary>
        public static readonly string domainUrl = "https://heytripgocar.com";

        /// <summary>
        /// 租车官网h5域名
        /// </summary>
        public static readonly string h5SiteUrl = "https://m.heytripgocar.com";

        /// <summary>
        /// 域名名称
        /// </summary>
        public static readonly string domainName = "heytripgocar.com";

        #region 车型组

        private static Dictionary<EnumCarSize, (int, EnumCarVehicleGroup)> VehicleGroupFirst;
        private static Dictionary<EnumCarDoors, (int, EnumCarVehicleGroup)> VehicleGroupSecond;

        public static Dictionary<EnumCarVehicleGroup, List<EnumCarVehicleGroup>> VehicleFilterGroup;

        static CarCommonHelper()
        {
            VehicleGroupFirst = new Dictionary<EnumCarSize, (int, EnumCarVehicleGroup)>()
            {
                { EnumCarSize.M,(1,EnumCarVehicleGroup.Mini) },
                { EnumCarSize.N,(1,EnumCarVehicleGroup.Mini) },
                { EnumCarSize.E,(1,EnumCarVehicleGroup.Economy) },
                { EnumCarSize.H,(1,EnumCarVehicleGroup.Economy) },
                { EnumCarSize.C,(1,EnumCarVehicleGroup.Compact) },
                { EnumCarSize.D,(1,EnumCarVehicleGroup.Compact) },
                { EnumCarSize.I,(1,EnumCarVehicleGroup.Medium) },
                { EnumCarSize.J,(1,EnumCarVehicleGroup.Medium) },
                { EnumCarSize.S,(1,EnumCarVehicleGroup.Large) },
                { EnumCarSize.R,(1,EnumCarVehicleGroup.Large) },
                { EnumCarSize.F,(1,EnumCarVehicleGroup.FullSize) },
                { EnumCarSize.G,(1,EnumCarVehicleGroup.FullSize) },
                { EnumCarSize.P,(1,EnumCarVehicleGroup.Premium) },
               // { EnumCarSize.U,(1,EnumCarVehicleGroup.Premium) },
                { EnumCarSize.L,(1,EnumCarVehicleGroup.Premium) },
                { EnumCarSize.W,(1,EnumCarVehicleGroup.Premium) },
                { EnumCarSize.O,(1,EnumCarVehicleGroup.Special) },
                { EnumCarSize.X,(1,EnumCarVehicleGroup.Special) },
            };
            VehicleGroupSecond = new Dictionary<EnumCarDoors, (int, EnumCarVehicleGroup)>()
            {
                { EnumCarDoors.B,(0,EnumCarVehicleGroup.Mini)},
                { EnumCarDoors.C,(0,EnumCarVehicleGroup.Mini)},
                { EnumCarDoors.D,(0,EnumCarVehicleGroup.Economy)},
                { EnumCarDoors.W,(2,EnumCarVehicleGroup.Estate)},
                { EnumCarDoors.V,(2,EnumCarVehicleGroup.Minivans)},
                { EnumCarDoors.L,(2,EnumCarVehicleGroup.Limo)},
                { EnumCarDoors.S,(2,EnumCarVehicleGroup.SportsCar)},
                 { EnumCarDoors.T,(2,EnumCarVehicleGroup.Convertible)},
                { EnumCarDoors.F,(2,EnumCarVehicleGroup.SUV)},
                { EnumCarDoors.J,(2,EnumCarVehicleGroup.Special)},
                { EnumCarDoors.X,(0,EnumCarVehicleGroup.Special)},
                { EnumCarDoors.P,(0,EnumCarVehicleGroup.Truck)},
                { EnumCarDoors.Q,(0,EnumCarVehicleGroup.Truck)},
                 { EnumCarDoors.Z,(0,EnumCarVehicleGroup.Special)},
                { EnumCarDoors.E,(2,EnumCarVehicleGroup.Convertible)},
                { EnumCarDoors.M,(2,EnumCarVehicleGroup.SUV)},
                 { EnumCarDoors.R,(0,EnumCarVehicleGroup.Motorhome)},
                  { EnumCarDoors.H,(2,EnumCarVehicleGroup.Motorhome)},
                  { EnumCarDoors.Y,(2,EnumCarVehicleGroup.Special)},
                { EnumCarDoors.N,(2,EnumCarVehicleGroup.Convertible)},
                { EnumCarDoors.G,(2,EnumCarVehicleGroup.SUV)},
                { EnumCarDoors.K,(2,EnumCarVehicleGroup.Truck)}
            };
            VehicleFilterGroup = new Dictionary<EnumCarVehicleGroup, List<EnumCarVehicleGroup>>()
            {
                {EnumCarVehicleGroup.Small,new List<EnumCarVehicleGroup>(){ EnumCarVehicleGroup.Mini, EnumCarVehicleGroup.Economy, EnumCarVehicleGroup.Compact } },
                {EnumCarVehicleGroup.Medium,new List<EnumCarVehicleGroup>(){ EnumCarVehicleGroup.Medium } },
                {EnumCarVehicleGroup.Minivans,new List<EnumCarVehicleGroup>(){ EnumCarVehicleGroup.Minivans } },
                {EnumCarVehicleGroup.Large,new List<EnumCarVehicleGroup>(){ EnumCarVehicleGroup.Large, EnumCarVehicleGroup.FullSize } },
                {EnumCarVehicleGroup.Estate,new List<EnumCarVehicleGroup>(){ EnumCarVehicleGroup.Estate } },
                {EnumCarVehicleGroup.SUV,new List<EnumCarVehicleGroup>(){ EnumCarVehicleGroup.SUV } },
                {EnumCarVehicleGroup.Special,new List<EnumCarVehicleGroup>(){ EnumCarVehicleGroup.SportsCar, EnumCarVehicleGroup.Convertible, EnumCarVehicleGroup.Special } },
                {EnumCarVehicleGroup.Truck,new List<EnumCarVehicleGroup>(){ EnumCarVehicleGroup.Truck} },
                {EnumCarVehicleGroup.Motorhome,new List<EnumCarVehicleGroup>(){ EnumCarVehicleGroup.Motorhome } },
                {EnumCarVehicleGroup.Premium,new List<EnumCarVehicleGroup>(){ EnumCarVehicleGroup.Premium } },
                 //{EnumCarVehicleGroup.Motorhome,new List<EnumCarVehicleGroup>(){ EnumCarVehicleGroup.Motorhome } },
                 //{EnumCarVehicleGroup.Limo,new List<EnumCarVehicleGroup>(){ EnumCarVehicleGroup.Limo } },
                 //{EnumCarVehicleGroup.Luxury,new List<EnumCarVehicleGroup>(){ EnumCarVehicleGroup.Luxury } },
                 //{EnumCarVehicleGroup.Oversize,new List<EnumCarVehicleGroup>(){ EnumCarVehicleGroup.Oversize } },
            };
        }

        /// <summary>
        /// 车型组。统一分组
        /// </summary>
        /// <returns></returns>
        public static EnumCarVehicleGroup GetVehicleGroup(string sipp)
        {
            if (string.IsNullOrWhiteSpace(sipp) || sipp.Length != 4)
            {
                return EnumCarVehicleGroup.None;
            }

            sipp = sipp.ToUpper().Trim();

            var first = (EnumCarSize)Enum.Parse(typeof(EnumCarSize), sipp.Substring(0, 1));
            var second = (EnumCarDoors)Enum.Parse(typeof(EnumCarDoors), sipp.Substring(1, 1));

            (int, EnumCarVehicleGroup) result = (0, EnumCarVehicleGroup.None);

            if (VehicleGroupFirst.TryGetValue(first, out var firstValue))
            {
                if (firstValue.Item1 > result.Item1)
                {
                    result = firstValue;
                }
            }

            if (VehicleGroupSecond.TryGetValue(second, out var secondValue))
            {
                if (secondValue.Item1 > result.Item1)
                {
                    result = secondValue;
                }
            }

            //return result.Item2;

            var filterGroup = VehicleFilterGroup.Where(w => w.Value.Any(a => a == result.Item2)).Select(s => s.Key)?.FirstOrDefault();

            return filterGroup ?? EnumCarVehicleGroup.None;
        }

        #endregion 车型组

        #region 统一政策条款

        public static List<StdTermAndCondition> GetTermsAndConditions(FullRentalPolicy full_rental_policy)
        {
            if (null == full_rental_policy.Insurance)
                throw new Exception("Insurance 为空");

            if (full_rental_policy.PaymentAndDeposit == null)
            {
                full_rental_policy.PaymentAndDeposit = new PaymentAndDepositPolicy();
            }

            const string change_line = "<br />";
            string insurance_str = string.Empty;
            foreach (var insurance in full_rental_policy.Insurance)
            {
                insurance_str += @$"{insurance.Text}{change_line}";
            }

            // Debug.Assert((full_rental_policy.PriceInfomation.RateDistance.Length > 0), "full_rental_policy.PriceInfomation.RateDistance is empty");
            StdSection insurance_section = new StdSection()
            {
                Title = "Included in Price",
                Text = @$"Airport/city/other Surcharge{change_line}
Vehicle Registration Fee{change_line}
Vehicle Rental{change_line}
Rate Distance {full_rental_policy.PriceInfomation.RateDistance}{change_line}
{insurance_str}
Taxes And Surcharges",
            };

            var priceInformation = new StdTermAndCondition()
            {
                Titel = "Price Information",
                Sections = new List<StdSection>()
                {
                    new StdSection()
                    {
                        Title = "Rental Period Calculation",
                        Text = "The minimum charge is one day (24 hours) , unless \"Calendar day\" is indicated on the Rental Agreement. The daily charge applies to consecutive 24-hour periods starting at the hour and minute the rental begins or, if a calendar day is specified on the rental document, each consecutive calendar day or any part of a calendar day starting on the calendar day on which the rental commences."
                    },
                    insurance_section,
                    new StdSection()
                    {
                        Text = @$"Charges for vehicle pickup/return out of office hours, such as administration fees and garage parking fees{change_line}
Charges for additionals/extras, such as GPS Navigation, Child seats, Infant seats, etc{change_line}
Delivery/Collection service offered by car rental agent{change_line}
Charges&fines including tolls, cross-border fees, congestion charges, parking fees, speeding tickets or any other traffic fines{change_line}
Additional drivers (unless '1 Additional Driver(s) Included ' is stipulated on the rental, addition of drivers will incur charge per day.){change_line}
Others not specified in car rental price inclusion",
                        Title = "Applicable Payable Fees&Charges"
                    },
                    new StdSection()
                    {
                        Title = "Note:",
                        Text = "The charges above are not included in the rental price unless stipulated otherwise. If applicable, the charges incurred are payable in local currency at the rental counter. Please pay attention to the terms of Rental Agreement you will sign at the time of pickup and review the final invoice for such charges once received."
                    },
                }
            };

            if (full_rental_policy.PriceInfomation.AdditionalServices.HasItems())
            {
                StdSection additional_section = new StdSection()
                {
                    Title = "Additional Service",
                    Text = string.Join(change_line, full_rental_policy.PriceInfomation.AdditionalServices),
                };

                priceInformation.Sections.Add(additional_section);
            }

            priceInformation.Sections.Add(new StdSection()
            {
                Title = "",
                Text = $"The prices given are only in estimated amount(tax included/excluded in the rental price - varying from car rental agents). The charges incurred are payable in local currency at the rental counter.{change_line}If the prices stand to be confirmed, our customer representative will email you the details after you make a request in your booking on {domainName}.{change_line}Requests for additional service cannot be guaranteed as availability is subject to change.",
            });

            var licenceSectionsSB = new StringBuilder();
            licenceSectionsSB.AppendLine(change_line + "The main driver and any additional drivers will need to provide a full driving licence in their name.");
            licenceSectionsSB.AppendLine(change_line + "It is each driver’s responsibility to find out what paperwork they need before driving in another country. For example, you may need a visa and/or International Driving Permit as well as your driving licence.");
            licenceSectionsSB.AppendLine(change_line + "Each driver will need to provide a valid driving licence. If it is written in non-Latin characters, they'll also need to provide a valid International Driving Permit or a certified translation. Any driver with a driving licence from outside Europe is advised to have an International Driving Permit as well.");

            var identificationSectionsSB = new StringBuilder("At the counter, you'll need to provide:");
            licenceSectionsSB.AppendLine(change_line + "Each driver's full, valid driving licence");
            licenceSectionsSB.AppendLine(change_line + "Your rental voucher");
            licenceSectionsSB.AppendLine(change_line + "The main driver must present this Rental Voucher when picking the car up.");

            if (!full_rental_policy.Licence.IsNullOrEmpty())
            {
                licenceSectionsSB.AppendLine(change_line);
                licenceSectionsSB.AppendLine(change_line + full_rental_policy.Licence);
            }

            var CompulsoryDocumentsWhenPickUp = new StdTermAndCondition()
            {
                Titel = "Compulsory documents  when pick-up",
                Sections = new List<StdSection>()
                {
                    new StdSection()
                    {
                        Title = "Licence",
                        Text = licenceSectionsSB.ToString()
                    },
                     new StdSection()
                    {
                        Title = "Identification",
                        Text = licenceSectionsSB.ToString()
                    }
                }
            };

            var patmentSectionsSB = new StringBuilder("For online payment, Heytripgo accepts ONLY: Credit/Debit Cards in forms of VISA, MasterCard, JCB, American Express, Discover. The credit card must be embossed and a PIN number may be required.");
            patmentSectionsSB.AppendLine(change_line + "Accepted payment method(s) by Budget for payment due at pick-up:");
            patmentSectionsSB.AppendLine(change_line + "Physical credit card(s) in the full name of the main driver(Virtual cards, electronic cards and prepaid cards are not accepted.)");
            patmentSectionsSB.AppendLine(change_line + "Accepted Credit Cards:");

            if (full_rental_policy.PaymentAndDeposit.Cards.HasItems())
            {
                patmentSectionsSB.AppendLine(string.Join(change_line, full_rental_policy.PaymentAndDeposit.Cards));
            }
            else
            {
                patmentSectionsSB.AppendLine("Visa; Master");
            }

            if (!full_rental_policy.PaymentAndDeposit.CardRemark.IsNullOrEmpty())
            {
                patmentSectionsSB.AppendLine(change_line + full_rental_policy.PaymentAndDeposit.CardRemark);
            }

            patmentSectionsSB.AppendLine(change_line + "*Accepted cards must be with magnetic stripes or chips.");
            patmentSectionsSB.AppendLine(change_line + "In the case of payment made by credit cards not in the name of main driver at pick-up, the driver may need to supplement evidentiary materials for scrutiny of payments or car rental agent may refuse to release the vehicle. No funds paid will be reimbursed.");

            var depositSectionsSB = new StringBuilder("You may pay for your deposit by the following ways");
            if (full_rental_policy.PaymentAndDeposit.Deposits.HasItems())
            {
                if (full_rental_policy.PaymentAndDeposit.Deposits.Count == 2)
                {
                    depositSectionsSB.AppendLine(change_line + $"{full_rental_policy.PaymentAndDeposit.Deposits[0].Currency} {full_rental_policy.PaymentAndDeposit.Deposits[0].Amount}" +
                        $"-{full_rental_policy.PaymentAndDeposit.Deposits[1].Currency} {full_rental_policy.PaymentAndDeposit.Deposits[1].Amount}");
                }
                else
                {
                    depositSectionsSB.AppendLine(change_line + $"{full_rental_policy.PaymentAndDeposit.Deposits[0].Currency} {full_rental_policy.PaymentAndDeposit.Deposits[0].Amount}");
                }
                depositSectionsSB.AppendLine(full_rental_policy.PaymentAndDeposit.DepositType == 1 ? "from main driver's debit card(s) at pick up." : "for pre - authorization on main driver's credit card(s) at pick up.");
            }
            depositSectionsSB.AppendLine(change_line + "Usually a deposit will be held as a guarantee by car rental companies at pick-up. This will be fully released or refunded if there is no damage/theft of the vehicle or any due payment after car return. Please note that this does not represent your total excess liability which can be found in the Insurance Coverage/ Excess liability section.");
            depositSectionsSB.AppendLine(change_line + "In the event that you fail to present a valid payment method with enough funds for deposit,the car rental agent may refuse to release the vehicle. In these instances, no funds paid will be reimbursed.");

            var paymentMethodAndDeposit = new StdTermAndCondition()
            {
                Titel = "Payment Method and Deposit",
                Sections = new List<StdSection>()
                {
                    new StdSection()
                    {
                        Title = "Payment Method",
                        Text = patmentSectionsSB.ToString()
                    },
                     new StdSection()
                    {
                        Title = "Deposit",
                        Text = depositSectionsSB.ToString()
                    }
                }
            };

            var driverAgeSectionsSB = new StringBuilder("");
            if (full_rental_policy.DriverAge.MinDrivingAge > 0)
            {
                driverAgeSectionsSB.AppendLine(change_line + $"Minimum age limit for this car: {full_rental_policy.DriverAge.MinDrivingAge} years old");
            }
            if (full_rental_policy.DriverAge.MaxDrivingAge > 0)
            {
                driverAgeSectionsSB.AppendLine(change_line + $"Maximum age limit for this car: {full_rental_policy.DriverAge.MaxDrivingAge} years old");
            }

            if (full_rental_policy.DriverAge.MinYoungAge > 0 && full_rental_policy.DriverAge.MaxYoungAge > 0)
            {
                driverAgeSectionsSB.AppendLine(change_line + $"A young driver fee will apply for drivers at: {full_rental_policy.DriverAge.MinYoungAge} - {full_rental_policy.DriverAge.MaxYoungAge} years old");
            }

            driverAgeSectionsSB.AppendLine(change_line + $"Charge (only for reference):{full_rental_policy.DriverAge.Currency} {full_rental_policy.DriverAge.YoungFee} per day");
            if (full_rental_policy.DriverAge.MaxYoungFee > 0)
            {
                driverAgeSectionsSB.AppendLine(change_line + $"{full_rental_policy.DriverAge.Currency} {full_rental_policy.DriverAge.MaxYoungFee} capped for your rental");
            }
            driverAgeSectionsSB.AppendLine(change_line + "If you fall out of the age limits,");
            driverAgeSectionsSB.AppendLine(change_line + "① You might not be able to hire a car.");
            driverAgeSectionsSB.AppendLine(change_line + "② You might be subject to charge of a young driver fee if you are under the minimum age limit) or");
            driverAgeSectionsSB.AppendLine(change_line + "③ You might be subject to to charge of a senior driver fee or purchase of extra insurance if you are over the maximum age limit.");
            driverAgeSectionsSB.AppendLine(change_line + "Please note that if applicable, such surcharge will be payable in local currency at the rental counter, tax excluded.");
            driverAgeSectionsSB.AppendLine(change_line + "If the age limit stands to be confirmed, a general rule applies (stands to be corrected) :");
            driverAgeSectionsSB.AppendLine(change_line + "① If you are under 25 years old, you may be charged of a young driver fee;");
            driverAgeSectionsSB.AppendLine(change_line + "② If you are over 65 years old, you may be subject to charge of a senior driver fee or purchase of extra insurance.");
            driverAgeSectionsSB.AppendLine(change_line + "In some car rental companies, additional age requirement may apply to certain car types.");
            var driverAgeRequirement = new StdTermAndCondition()
            {
                Titel = "Driver’s Age Requirement",
                Sections = new List<StdSection>()
                {
                    new StdSection()
                    {
                      //  Title = "Payment Method",
                        Text = driverAgeSectionsSB.ToString()
                    }
                }
            };

            var additionalDriversSectionsSB = new StringBuilder("Driver's age and driving licence requirements apply to additional drivers.");
            additionalDriversSectionsSB.AppendLine(change_line + "The number of additional drivers is limited to the number of passenger seats in the vehicle rented. Request to add drivers is made upon pickup at the rental counter.");
            additionalDriversSectionsSB.AppendLine(change_line + "Additional drivers should present the required license documents along with the main driver's for authorization at the time of pickup.");
            additionalDriversSectionsSB.AppendLine(change_line + "Additional charge may apply for adding addtional drivers and is paid in local currency at rental desk.");
            var additional_drivers = new StdTermAndCondition()
            {
                Titel = "Additional Drivers",
                Sections = new List<StdSection>()
                {
                    new StdSection()
                    {
                        Text =additionalDriversSectionsSB.ToString()
                    }
                }
            };

            var carTypeDescriptionSectionsSB = new StringBuilder("Driver's age and driving licence requirements apply to additional drivers.");
            carTypeDescriptionSectionsSB.AppendLine(change_line + "The vehicles tagged “or similar” are displayed with images of similar cars in the same category. Vehicle make, model and color are not guaranteed at pick-up. However, you are entitled to a vehicle with the same passenger and luggage capacity in the same category.");
            carTypeDescriptionSectionsSB.AppendLine(change_line + "If the car rental agent fails to provide you with a car of the same category, you are entitled to a refund or free upgrade.");
            carTypeDescriptionSectionsSB.AppendLine(change_line + "If it is stipulated as \"model guaranteed\", the make and model of the vehicles are guaranteed at pick-up.");
            var car_type_description = new StdTermAndCondition()
            {
                Titel = "Car Type Description",
                Sections = new List<StdSection>()
                {
                    new StdSection()
                    {
                        Text =carTypeDescriptionSectionsSB.ToString()
                    }
                }
            };

            var InsuranceSectionsSB = new StringBuilder();
            if (full_rental_policy.Insurance.HasItems())
            {
                InsuranceSectionsSB.Append("Insurance Package Inclusion");
                foreach (var insurance in full_rental_policy.Insurance)
                {
                    InsuranceSectionsSB.AppendLine(change_line + insurance.Text);
                    switch (insurance.Type)
                    {
                        case InsuranceType.CDW:
                            InsuranceSectionsSB.AppendLine(change_line + "In the case of damage to the bodywork of the rental car, it limits your financial loss to an excess, the maximum amount that you will have to pay. It usually does not cover the engine, windscreens, tyres, undercarriage, wheels, indicators, interior and towing charges, etc.\r\n");
                            break;

                        case InsuranceType.TP:
                            InsuranceSectionsSB.AppendLine(change_line + "It covers part of the cost in the event of a theft or robbery incident involving the rental car. It does not cover anything inside the car, such as luggage or GPS. It only limits your financial loss to an excess, sometimes known as the deductible - the maximum amount that you have to pay in the case of theft or attempted theft.");

                            break;
                    }
                }
            }
            InsuranceSectionsSB.AppendLine(change_line + "Note:");
            InsuranceSectionsSB.AppendLine(change_line + "●The excess(if applicable) is the maximum amount you will be held liable for in the case of damage or theft of the vehicle.");
            InsuranceSectionsSB.AppendLine(change_line + "●The insurance package above does not cover windscreens, tyres, undercarriage, replacement locks, replacement keys and towing charges.");
            InsuranceSectionsSB.AppendLine(change_line + "●Extended protection products/excess waiver products are available for purchase at the time of pickup. Renters may purchase them in their interests. Heytripgo will not endorse any of these products.");
            InsuranceSectionsSB.AppendLine(change_line + "●The insurance package will be nullified under any of the following circumstances:");
            InsuranceSectionsSB.AppendLine(change_line + "●Damage caused by improper use of the vehicle, such as scratching the headstock when standing on the roof to take photos and ironing of the cigarette butts");
            InsuranceSectionsSB.AppendLine(change_line + "●Vehicle theft due to loss or damage of the vehicle's key, or to park the vehicle for a long time in a sparsely populated area");
            InsuranceSectionsSB.AppendLine(change_line + "●Damage by an unregistered driver driving the vehicle");
            InsuranceSectionsSB.AppendLine(change_line + "●Damage due to speeding, drunk driving or violation of laws and regulations of the country of travel");
            InsuranceSectionsSB.AppendLine(change_line + "●Damage caused by vehicle wading");
            InsuranceSectionsSB.AppendLine(change_line + "●Vehicle repair costs not recognized by car rental agent");
            InsuranceSectionsSB.AppendLine(change_line + "●Failure to call the Police (for official police report) and car rental agent in the case of accident or theft");
            InsuranceSectionsSB.AppendLine(change_line + "●Damage and loss caused when driving across island or across country without consent and authorization of car rental agent");
            InsuranceSectionsSB.AppendLine(change_line + "●For damage made by third party, car rental agent will deduct the cost from the deposit temporarily and will release it after compensation is made by third party.");
            var insurance_info = new StdTermAndCondition()
            {
                Titel = "Insurance Information",
                Sections = new List<StdSection>()
                {
                    new StdSection()
                    {
                        Text = InsuranceSectionsSB.ToString()
                    }
                }
            };

            var FuelPolicySectionsSB = new StringBuilder("Fuel Policy");
            FuelPolicySectionsSB.AppendLine(change_line + full_rental_policy.Fuel.Type.GetDescription());
            FuelPolicySectionsSB.AppendLine(change_line + "Note:");
            FuelPolicySectionsSB.AppendLine(change_line + "① Confirm with the rental counter about applicable fuel type of the vehicle and make sure you choose the right type when you refuel it.");
            FuelPolicySectionsSB.AppendLine(change_line + "② Take photos of the total mileage and fuel gauge at the time of pick-up and return and keep the last refueling documents. These materials would be helpful in the case of dispute over fuel, especially under full-to-full fuel policy.");
            var fuel_section = new StdTermAndCondition()
            {
                Titel = "Fuel Policy",
                Sections = new List<StdSection>()
                {
                    new StdSection()
                    {
                         Text =FuelPolicySectionsSB.ToString()
                    },
                }
            };

            var CancelPolicySectionsSB = new StringBuilder();
            switch (full_rental_policy.Cancellation.Type)
            {
                case EnumCarCancelType.FreeCancel:
                    CancelPolicySectionsSB.AppendLine(change_line + "Free Cancellation");
                    CancelPolicySectionsSB.AppendLine(change_line + @$"Free cancellation before reserved pick-up time (Local time:{full_rental_policy.Cancellation.LocationTime}); Refunds will be made to the original credit card used for booking.");
                    break;

                case EnumCarCancelType.FeeCancel:
                    CancelPolicySectionsSB.AppendLine(change_line + "Fee Cancellation");
                    CancelPolicySectionsSB.AppendLine(change_line + @$"If you cancel within {full_rental_policy.Cancellation.CancelHours} hours before pickup(Local time:{full_rental_policy.Cancellation.LocationTime}), you’ll be charged a cancellation fee {full_rental_policy.Cancellation.FeeText}, or if your online payment is less than the fee, no refund will be made for your booking.");
                    break;

                case EnumCarCancelType.NonCancel:
                    CancelPolicySectionsSB.AppendLine(change_line + "No Cancellation");
                    break;
            }
            var NoShowSectionsSB = new StringBuilder();
            NoShowSectionsSB.AppendLine(change_line + $@"In the event of a no-show or failure to pick up the vehicle, a fee equal to");
            if (full_rental_policy.Cancellation.Type == EnumCarCancelType.NonCancel)
            {
                NoShowSectionsSB.AppendLine(change_line + "the total prepaid amount");
            }
            else
            {
                NoShowSectionsSB.AppendLine(change_line + $"of {full_rental_policy.Cancellation.NoShow}% of total rental ");
            }

            var cancellation_section = new StdTermAndCondition()
            {
                Titel = "Cancellation and No Show Policy",
                Sections = new List<StdSection>()
                {
                   new StdSection()
                   {
                       Title ="Cancellation",
                       Text = CancelPolicySectionsSB.ToString(),
                   },
                   new StdSection()
                   {
                       Title ="NO SHOW",
                       Text = NoShowSectionsSB.ToString(),
                   }
                }
            };

            var StdTermAndConditions = new List<StdTermAndCondition>()
            {
                priceInformation,
                CompulsoryDocumentsWhenPickUp,
                paymentMethodAndDeposit,
                driverAgeRequirement,
                additional_drivers,
                car_type_description,
                insurance_info,
                fuel_section,
                cancellation_section,
            };

            if (full_rental_policy.OtherInformation.HasItems())
            {
                var other_section = new StdTermAndCondition()
                {
                    Titel = "Other Information",
                    Sections = full_rental_policy.OtherInformation.Select(s => new StdSection()
                    {
                        Text = s.Item2,
                        Title = s.Item1,
                    }).ToList()
                };

                StdTermAndConditions.Add(other_section);
            }

            return StdTermAndConditions;
        }

        #endregion 统一政策条款
    }

    public class Encryptor
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("1234567812345678"); // 16字节密钥
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567812345678");  // 16字节初始化向量

        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                byte[] bytes = Convert.FromBase64String(cipherText);

                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}