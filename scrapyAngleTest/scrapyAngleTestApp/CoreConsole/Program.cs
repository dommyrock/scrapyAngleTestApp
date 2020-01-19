using CoreConsole.DataflowExamples;
using System;

namespace CoreConsole
{
    class Program
    {
        //not useed atm
        static async System.Threading.Tasks.Task Main(string[] args)
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

            #region TPL Dataflow Methods

            #region Block Types

            //BufferBlockSendAsync.Run();
            //BroadcastBlock.Run();
            //JoinBlock.Run();
            //JoinBlock.RunParallel();
            //BatchedJoinBlock.RunParallel();
            //WriteOnceBlock.Run();

            #endregion Block Types

            //With completion

            //await Completion.RunParallel();
            //await Completion.RunParallelExtension();
            //await LinkToOptionsAppend.Run();
            //await LinkToOptionsMaxMessages.Run();
            //await LinkToOptionsMessageFiltering.Run();
            //await MultipleProducersSingleConsumer.Run();
            //await MultipleProducersSingleConsumer.RunCustom();

            //Error handling
            //await ErrorHandling.Run();

            //Custom
            //await CustomBlockEncapsulate.Run();
            //await CustomGuaranteedDeliveryBroadcast.Run();

            #region Performance and monitoring

            //TPLDataflowPerformace.Run();
            //TPLDataflowPerformace.RunNonDataflowVersion();
            //await SingleProducerConstrained.Run();
            //await ConcurrentExclusiveScheduler.Run();
            //Decrease time between switches ---> ,new ExecutionDataflowBlockOptions(){MaxMessagesPerTask = 5} ....example
            //await MaxMessagesPerTask.Run();

            //External monitoring ---> Application insights  --> Hosted on Azure (SaaS) ,or Graphite

            #endregion Performance and monitoring

            #endregion TPL Dataflow Methods
        }
    }
}