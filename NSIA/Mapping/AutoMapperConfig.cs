using AutoMapper;
using NSIA.DTO;
using NSIA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSIA.Mapping
{
    //Don't forget to configure in Global.asax
    public class AutoMapperConfig: Profile
    {
        public static void Initialize()
        {
            Mapper.Initialize((config) =>
            {
                // Doamin to APIResources
                config.CreateMap<StatesDB, StateDBDTO>().ReverseMap();
                config.CreateMap<LGA, LgaDTO>().ReverseMap();
                config.CreateMap<Vehicle, CarMakeOutputDTO>().ReverseMap();
                config.CreateMap<Vehicle, CarModelDTO>().ReverseMap();
                config.CreateMap<UserInputDTO, AspNetUser>().ReverseMap();
                config.CreateMap<VehicledetailsInputDTO, VehicleDetail>().ReverseMap();
                config.CreateMap<VehicleInputDTO, Vehicle>().ReverseMap();
                config.CreateMap<DstvCustomerDetailsInputDTO, DstvSubscriber>().ReverseMap();
                config.CreateMap<DstvPolicy, DstvPoliciesDTO>().ReverseMap();
                config.CreateMap<DstvPremiumDetailsInput, DstvPremiumDetail>().ReverseMap();
                config.CreateMap<DstvHomeType, DstvHomeTypesDTO>().ReverseMap();
                config.CreateMap<DstvPersonalOption, DstvPersonalOptionsDTO>().ReverseMap();
                config.CreateMap<DstvTransactionDTO, DstvTransaction>().ReverseMap();
                config.CreateMap<DstvSubscriberTitle, DstvSubscriberTitleDTO>().ReverseMap();
                config.CreateMap<DstvSubscriberIdentificationType, DstvSubscriberIdMeansDTO>().ReverseMap();
            });
        }
    }
}