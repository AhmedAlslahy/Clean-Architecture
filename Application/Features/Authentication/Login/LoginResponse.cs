using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Authentication.Login;

public class LoginResponse
{
    public required string Token { get; set; }
    public DateTime ExpireIn { get; set; }
}