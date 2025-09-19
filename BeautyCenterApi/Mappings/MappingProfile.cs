using AutoMapper;
using BeautyCenterApi.Models;
using BeautyCenterApi.DTOs;

namespace BeautyCenterApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            // Customer mappings
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Customer, CustomerWithDetailsDto>()
                .ForMember(dest => dest.Appointments, opt => opt.MapFrom(src => src.Appointments))
                .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments))
                .ForMember(dest => dest.TotalPaid, opt => opt.Ignore())
                .ForMember(dest => dest.RemainingBalance, opt => opt.Ignore());

            // ServiceType mappings
            CreateMap<ServiceType, ServiceTypeDto>().ReverseMap();
            CreateMap<CreateServiceTypeDto, ServiceType>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateServiceTypeDto, ServiceType>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Appointment mappings
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => $"{src.Customer.FirstName} {src.Customer.LastName}"))
                .ForMember(dest => dest.ServiceTypeName, opt => opt.MapFrom(src => src.ServiceType.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.ServiceType.DurationMinutes))
                .ForMember(dest => dest.ServicePrice, opt => opt.MapFrom(src => src.ServiceType.Price));

            CreateMap<CreateAppointmentDto, Appointment>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Scheduled"))
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
                .ForMember(dest => dest.FinalPrice, opt => opt.Ignore())
                .ForMember(dest => dest.SessionsRemaining, opt => opt.MapFrom(src => src.SessionsTotal));

            CreateMap<UpdateAppointmentDto, Appointment>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.SessionsRemaining, opt => opt.Ignore());

            // Payment mappings
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => $"{src.Customer.FirstName} {src.Customer.LastName}"))
                .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Customer.Phone))
                .ForMember(dest => dest.ServiceTypeName, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.ServiceType.Name : null))
                .ForMember(dest => dest.ServicePrice, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.ServiceType.Price : (decimal?)null))
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.AppointmentDate : (DateTime?)null))
                .ForMember(dest => dest.Installments, opt => opt.MapFrom(src => src.Installments));

            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            
            // PaymentInstallment mappings
            CreateMap<PaymentInstallment, PaymentInstallmentDto>().ReverseMap();
            CreateMap<CreateInstallmentDto, PaymentInstallment>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}