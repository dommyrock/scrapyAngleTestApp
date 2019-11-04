using Autofac;
using SiteSpecificScrapers;
using System.Linq;
using System.Reflection;

namespace scrapyAngleTestApp
{
    /// <summary>
    /// Startup.cs in .Net CORE->(D.Injection system makes dependency inversion principle easier)
    /// </summary>
    public static class ContainerConfig
    {
        public static IContainer Configure()//test this one
        {
            //like key value pair list of all our Child classes we want ti instantiate
            var builder = new ContainerBuilder();

            //builder.RegisterType<NabavaNetSitemap>().As<INabavaNetSitemap> 1 by 1 method (non efficient for bigger projects... )better use default .Core DI

            //By default, all concrete classes in the assembly will be registered.
            //This includes internal and nested private classes.
            /// <see cref="=https://autofac.readthedocs.io/en/latest/register/scanning.html"/>
            //In simple terms, think about a .NET type that implements an interface!!

            builder.RegisterAssemblyTypes(Assembly.Load(nameof(SiteSpecificScrapers)))//nameof makes it strongly typed, so better than string :)
                    .As<ISiteSpecific>(); //Expose types via its Service
            //.Where(t => t.Namespace.Contains("Services")) can filter it with lambda
            //.AsImplementedInterfaces();//Register the type as providing all of its public interfaces as services

            return builder.Build();
        }
    }
}