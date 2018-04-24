using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    public enum Criterion
    {
        Less = -2,
        LessOrEqual = -1,
        Equal = 0,
        MoreOrEqual = 1,
        More = 2
    }
}
