using System;
using System.Collections.Generic;

namespace Example2.Database.Models;

public partial class Account
{
    public string AccountNo { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;
}
