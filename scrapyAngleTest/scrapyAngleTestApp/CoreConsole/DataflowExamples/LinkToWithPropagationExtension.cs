using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace CoreConsole.DataflowExamples
{
    public static class LinkToWithPropagationExtension
    {
        //TPL DF Interfaces
        //ISouceBlock<> -implemented by every block that produces msgs
        //ITargetBlock<> -implemented by every block that accepts msgs
        //IPropagatorBlock<> -combined in both , that implements both above without adding new methods of its own
        //IDataflowBlock<> -contains methods for block completion
        public static IDisposable LinkToWithPropagation<T>(this ISourceBlock<T> source, ITargetBlock<T> target)
        {
            return source.LinkTo(target, new DataflowLinkOptions() { PropagateCompletion = true });
        }
    }
}