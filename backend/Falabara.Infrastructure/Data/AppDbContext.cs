public DbSet<User> Users { get; set; }

dotnet ef migrations add CreateUsers
dotnet ef database update