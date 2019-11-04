using Autofac;
using SiteSpecificScrapers;
using System.Linq;
using System.Reflection;

namespace scrapyAngleTestApp
{
    /// <summary>
    /// (D.Injection system makes dependency inversion principle easier)
    /// </summary>
    public static class ContainerConfig
    {
        public static IContainer Configure()//test this one
        {
            //like key value pair list of all our Child classes we want ti instantiate
            var builder = new ContainerBuilder();

            //1 at the time
            //We depend on interfaces not on implementations (...so i can just switch type<Application> for any other that implements IApplication)
            builder.RegisterType<Application>().As<IApplication>();//same as calling .AsSelf() --to inject its own concrete instance

            //By default, all concrete classes in the assembly will be registered.
            //This includes internal and nested private classes.
            /// <see cref="=https://autofac.readthedocs.io/en/latest/register/scanning.html"/>

            //Multiple
            builder.RegisterAssemblyTypes(Assembly.Load(nameof(SiteSpecificScrapers)))//nameof makes it strongly typed, so better than string :)
                    .As<ISiteSpecific>(); //Expose types via its Service
            //.Where(t => t.Namespace.Contains("Services")) can filter it with lambda
            //.AsImplementedInterfaces();//Register the type as providing all of its public interfaces as services

            return builder.Build();
        }
    }
}

//Dependency inversion --->[TOP ->DOWN] instead of Bottop UP
//Top level controlls/registers all of our dependencies , (in this case Program.cs -consoleApp)