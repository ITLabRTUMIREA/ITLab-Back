using System;
using WebApp.Configure.Models.Behavior;
using WebApp.Configure.Models.Shared.Interfaces;
namespace WebApp.Configure.Models.Shared
{
    public class WorkPathState : IWorkPathGetter
    {

        public WorkHandlePath WorkHandlePath { get; set; }

        public WorkHandlePath GetHandlePath()
            => WorkHandlePath;

        public void SetHandlePath(WorkHandlePath path)
            => WorkHandlePath = path;
    }
}
