﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Result
{
    /// <summary>
    /// User-controlled result state.
    /// </summary>
    public enum ResultState
    {
        Found,
        Ignored,
        ToBeFixed,
        Fixed
    }
}
