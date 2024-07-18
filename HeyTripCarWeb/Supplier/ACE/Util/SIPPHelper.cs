using XiWan.Car.BusinessShared.Enums;

namespace HeyTripCarWeb.Supplier.ACE.Util
{
    public class SIPPHelper
    {
        /// <summary>
        /// 根据SIPP码返回燃料,驱动，门信息
        /// type：0(燃料) 1(驱动) 2 (门)
        /// </summary>
        /// <param name="sippCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int SIPPCodeAnalysis(string sippCode, int type)
        {
            var returnRes = 0;
            switch (type)
            {
                case 0: //燃料信息
                    switch (sippCode[3])
                    {
                        case 'N':
                        case 'R':
                            returnRes = (int)EnumCarFuelType.Unspecified;
                            break;

                        case 'V':
                        case 'Z':
                            returnRes = (int)EnumCarFuelType.Petrol;
                            break;

                        case 'D':
                        case 'Q':
                            returnRes = (int)EnumCarFuelType.Diesel;
                            break;

                        case 'E':
                        case 'C':
                            returnRes = (int)EnumCarFuelType.Electric;
                            break;

                        case 'H':
                        case 'I':
                            returnRes = (int)EnumCarFuelType.Hybrid;
                            break;

                        case 'L':
                        case 'S':
                            returnRes = (int)EnumCarFuelType.CompressedGas;
                            break;

                        case 'M':
                        case 'F':
                            returnRes = (int)EnumCarFuelType.MultiFuel;
                            break;

                        case 'A':
                        case 'B':
                            returnRes = (int)EnumCarFuelType.Hydrogen;
                            break;

                        case 'U':
                        case 'X':
                            returnRes = (int)EnumCarFuelType.Ethanol;
                            break;
                    }
                    break;

                case 1: //驱动信息
                    switch (sippCode[2])
                    {
                        case 'A':
                        case 'M':
                            returnRes = (int)EnumCarDriveType.Unspecified;
                            break;

                        case 'B':
                        case 'N':
                            returnRes = (int)EnumCarDriveType.WD4;
                            break;

                        case 'D':
                        case 'C':
                            returnRes = (int)EnumCarDriveType.AWD;
                            break;
                    }
                    break;

                case 2:
                    switch (sippCode[1])
                    {
                        case 'B':
                            returnRes = (int)EnumCarDoorCount.Door2;
                            break;

                        case 'D':
                            returnRes = (int)EnumCarDoorCount.Door4;
                            break;

                        case 'C':
                            returnRes = (int)EnumCarDoorCount.Door2_4;
                            break;
                    }
                    break;

                case 3:
                    switch (sippCode[0])
                    {
                        case 'M':
                        case 'E':
                        case 'N':
                        case 'H':
                            returnRes = (int)EnumCarVehicleGroup.Small;
                            break;

                        case 'C':
                        case 'D':
                            returnRes = (int)EnumCarVehicleGroup.Medium;
                            break;

                        case 'I':
                        case 'S':
                        case 'F':
                        case 'J':
                        case 'R':
                        case 'G':
                            returnRes = (int)EnumCarVehicleGroup.Large;
                            break;

                        case 'L':
                        case 'P':
                        case 'X':
                        case 'U':
                        case 'W':
                            returnRes = (int)EnumCarVehicleGroup.Premium;
                            break;

                        case 'O':
                            returnRes = (int)EnumCarVehicleGroup.Oversize;
                            break;
                    }
                    break;

                case 4:
                    switch (sippCode[0])
                    {
                        case 'C':
                            returnRes = (int)EnumCarVehicleClass.Compact;
                            break;

                        case 'D':
                            returnRes = (int)EnumCarVehicleClass.CompactElite;
                            break;

                        case 'E':
                            returnRes = (int)EnumCarVehicleClass.Economy;
                            break;

                        case 'F':
                            returnRes = (int)EnumCarVehicleClass.Fullsize;
                            break;

                        case 'G':
                            returnRes = (int)EnumCarVehicleClass.FullsizeElite;
                            break;

                        case 'H':
                            returnRes = (int)EnumCarVehicleClass.EconomyElite;
                            break;

                        case 'I':
                            returnRes = (int)EnumCarVehicleClass.Intermediate;
                            break;

                        case 'J':
                            returnRes = (int)EnumCarVehicleClass.IntermediateElite;
                            break;

                        case 'L':
                            returnRes = (int)EnumCarVehicleClass.Luxury;
                            break;

                        case 'M':
                            returnRes = (int)EnumCarVehicleClass.Mini;
                            break;

                        case 'N':
                            returnRes = (int)EnumCarVehicleClass.MiniElite;
                            break;

                        case 'O':
                            returnRes = (int)EnumCarVehicleClass.Oversize;
                            break;

                        case 'P':
                            returnRes = (int)EnumCarVehicleClass.Premium;
                            break;

                        case 'R':
                            returnRes = (int)EnumCarVehicleClass.StandardElite;
                            break;

                        case 'S':
                            returnRes = (int)EnumCarVehicleClass.Standard;
                            break;

                        case 'U':
                            returnRes = (int)EnumCarVehicleClass.PremiumElite;
                            break;

                        case 'W':
                            returnRes = (int)EnumCarVehicleClass.LuxuryElite;
                            break;

                        case 'X':
                            returnRes = (int)EnumCarVehicleClass.Special;
                            break;
                    }
                    break;

                case 5:
                    switch (sippCode[2])
                    {
                        case 'A':
                        case 'B':
                        case 'D':
                            returnRes = (int)EnumCarTransmissionType.Automatic;
                            break;

                        case 'C':
                        case 'M':
                        case 'N':
                            returnRes = (int)EnumCarTransmissionType.Manual;
                            break;
                    }
                    break;

                case 6:
                    switch (sippCode[1])
                    {
                        case 'B':
                            returnRes = (int)EnumCarVehicleCategory.DoorCar2_3;
                            break;

                        case 'C':
                            returnRes = (int)EnumCarVehicleCategory.DoorCar2Or4;
                            break;

                        case 'D':
                            returnRes = (int)EnumCarVehicleCategory.DoorCar4_5;
                            break;

                        case 'E':
                            returnRes = (int)EnumCarVehicleCategory.Coupe;
                            break;

                        case 'F':
                            returnRes = (int)EnumCarVehicleCategory.SUV;
                            break;

                        case 'G':
                            returnRes = (int)EnumCarVehicleCategory.Crossover;
                            break;

                        case 'H':
                            returnRes = (int)EnumCarVehicleCategory.Motorhome;
                            break;

                        case 'J':
                            returnRes = (int)EnumCarVehicleCategory.AllTerrain;
                            break;

                        case 'K':
                            returnRes = (int)EnumCarVehicleCategory.CommercialVanOrTruck;
                            break;

                        case 'L':
                            returnRes = (int)EnumCarVehicleCategory.Limo;
                            break;

                        case 'M':
                            returnRes = (int)EnumCarVehicleCategory.Monospace;
                            break;

                        case 'N':
                            returnRes = (int)EnumCarVehicleCategory.Roadster;
                            break;

                        case 'P':
                            returnRes = (int)EnumCarVehicleCategory.Pickup;
                            break;

                        case 'Q':
                            returnRes = (int)EnumCarVehicleCategory.Pickup;
                            break;

                        case 'R':
                            returnRes = (int)EnumCarVehicleCategory.Recreational;
                            break;

                        case 'S':
                            returnRes = (int)EnumCarVehicleCategory.Sport;
                            break;

                        case 'T':
                            returnRes = (int)EnumCarVehicleCategory.Convertible;
                            break;

                        case 'V':
                            returnRes = (int)EnumCarVehicleCategory.Van;
                            break;

                        case 'W':

                            break;

                        case 'X':
                            returnRes = (int)EnumCarVehicleCategory.Special;
                            break;

                        case 'Y':
                            returnRes = (int)EnumCarVehicleCategory.ToWheelVehicle;
                            break;

                        case 'Z':
                            returnRes = (int)EnumCarVehicleCategory.SpecialOffer;
                            break;
                    }
                    break;
            }
            return returnRes;
        }
    }
}