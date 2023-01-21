using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Auth
{
    public class Settings
    {
        public class JWT
        {
            public string Secret { get; set; } = string.Empty;
        }
    }
}
