using CoreConsole.DataflowExamples;
using System;

namespace CoreConsole
{
    class Program
    {
        //not useed atm
        static void Main(string[] args)
        {
            //DI CONTAINER -> service registration

            //// add the framework services
            //var services = new ServiceCollection();
            ////.AddLogging();

            //// add StructureMap
            //var container = new Container();
            //container.Configure(config =>
            //{
            //    // Register stuff in container, using the StructureMap APIs...
            //    config.Scan(_ =>
            //    {
            //        _.AssemblyContainingType(typeof(Program));
            //        _.WithDefaultConventions();
            //    });
            //    // Populate the container using the service collection
            //    config.Populate(services);
            //});

            //var serviceProvider = container.GetInstance<IServiceProvider>();

            //// rest of method as before
            ///
            //--------------------------------------------------------------------------------
            //Call TPL Dataflow Methods:

            #region TPL Block Types

            //BufferBlockSendAsync.Run();
            //BroadcastBlock.Run();
            //JoinBlock.Run();
            //JoinBlock.RunParallel();
            //BatchedJoinBlock.RunParallel();
            //WriteOnceBlock.Run();

            #endregion TPL Block Types
        }
    }
}