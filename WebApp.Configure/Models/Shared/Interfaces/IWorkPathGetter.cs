using System;
using WebApp.Configure.Models.Behavior;

namespace WebApp.Configure.Models.Shared.Interfaces
{
    public interface IWorkPathGetter
    {
        WorkHandlePath GetHandlePath();
        void SetHandlePath(WorkHandlePath path);
    }
}
