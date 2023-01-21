using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Auth
{
    public class JwtModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
