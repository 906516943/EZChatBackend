﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Models
{
    public record class AuthInfo(DateTime ExpireDate, Guid UserId);
}