using BackEnd.Interfaces;
using BackEnd.Interfaces.IBusinessServices;
using BackEnd.Services.BusinessServices;

namespace BackEnd.Services
{
    public static class ServicesStartup
    {

      
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {

            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient<IStorageServices, StorageServices>();
            builder.Services.AddTransient<IMailService, MailService>();
            builder.Services.AddTransient<IGenericService, GenericService>();
            builder.Services.AddTransient<ICustomerServices, CustomerServices>();
            builder.Services.AddTransient<IRealEstatePropertyServices, RealEstatePropertyServices>();
            builder.Services.AddTransient<IRealEstatePropertyPhotoServices, RealEstatePropertyPhotoServices>();
            builder.Services.AddTransient<ICalendarServices, CalendarServices>();
        }
    }
}