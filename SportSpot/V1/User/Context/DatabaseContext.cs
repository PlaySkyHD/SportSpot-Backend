﻿using Microsoft.EntityFrameworkCore;

namespace SportSpot.V1.User
{
    public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
    {
    }
}
