using CestasDeMaria.Application.Interfaces;
using CestasDeMaria.Application.Services;
using CestasDeMaria.Domain.Interfaces.Repository;
using CestasDeMaria.Domain.Interfaces.Services;
using CestasDeMaria.Domain.Services;
using CestasDeMaria.Infrastructure.CrossCutting.Adapter;
using CestasDeMaria.Infrastructure.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;

namespace CestasDeMaria.Infrastructure.CrossCutting.IoC
{
    public class DependencyResolverContainer : Container
    {
        private static DependencyResolverContainer _instance;

        public static DependencyResolverContainer Instance => _instance ?? (_instance = new DependencyResolverContainer());

        private DependencyResolverContainer()
        {
            //RegisterServices();
        }

        public DependencyResolverContainer(IServiceCollection services)
        {
            RegisterServices(services);
        }

        public void RegisterService<TService, TImplementation>(IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddScoped<TService, TImplementation>();
        }

        public void RegisterServices(IServiceCollection services)
        {
            #region Sevice

            //RegisterService<IViaCepService, ViaCepService>(services);
            RegisterService<IBlobStorageService, BlobAzureStorageService>(services);
            RegisterService<ISendGridService, SendGridService>(services);

            #endregion

            #region Application

            RegisterService<ILoggerAppService, LoggerAppService>(services);
            RegisterService<IMailMessageAppService, MailMessageAppService>(services);
            RegisterService<IAdminsAppService, AdminsAppService>(services);
            RegisterService<IBasketdeliveriesAppService, BasketdeliveriesAppService>(services);
            RegisterService<IBasketdeliverystatusAppService, BasketdeliverystatusAppService>(services);
            RegisterService<IFamiliesAppService, FamiliesAppService>(services);
            RegisterService<IFamilyfamilystatushistoryAppService, FamilyfamilystatushistoryAppService>(services);
            RegisterService<IFamilystatusAppService, FamilystatusAppService>(services);

            #endregion

            #region Repository

            RegisterService<IAdminsRepository, AdminsRepository>(services);
            RegisterService<IBasketdeliveriesRepository, BasketdeliveriesRepository>(services);
            RegisterService<IBasketdeliverystatusRepository, BasketdeliverystatusRepository>(services);
            RegisterService<IFamiliesRepository, FamiliesRepository>(services);
            RegisterService<IFamilyfamilystatushistoryRepository, FamilyfamilystatushistoryRepository>(services);
            RegisterService<IFamilystatusRepository, FamilystatusRepository>(services);
            RegisterService<ILoggerRepository, LoggerRepository>(services);
            RegisterService<IMailmessageRepository, MailMessageRepository>(services);

            #endregion

            TypeAdapterFactory.SetCurrent(new AutomapperTypeAdapterFactory());
        }
    }
}
