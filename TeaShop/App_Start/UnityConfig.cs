using TeaShop.Respository;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using TeaShop.Controllers;

namespace TeaShop
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container.RegisterType<DataContext>();
            //container.RegisterType<HomeController>();
            //container.RegisterType<CategoryController>();
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}