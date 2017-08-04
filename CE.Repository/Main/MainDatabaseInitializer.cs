using CE.Entity.Main;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BGP.Utils.Common;
using System.IO;
using System.Reflection;

namespace CE.Repository.Main
{
    public class MainDatabaseInitializer : DropCreateDatabaseIfModelChanges<MainContext>
    {
        protected override void Seed(MainContext context)
        {
            try
            {
                //var mongoClient = Mongo.MongoDatabaseFactory.Create();

                // role
                var superAdminRole = new Role() { Name = CE.Enum.UserRole.SuperAdmin.GetDescription() };
                var clientRole = new Role() { Name = CE.Enum.UserRole.Client.GetDescription() };
                var vendorRole = new Role() { Name = CE.Enum.UserRole.Vendor.GetDescription() };

                // The order is very important
                context.Roles.Add(superAdminRole);
                context.Roles.Add(vendorRole);
                context.Roles.Add(clientRole);

                context.SaveChanges();

                // user
                var passwordHasher = new PasswordHasher();
                var superAdminUser = new User()
                {
                    UserName = "admin@ce.com",
                    Email = "admin@ce.com",
                    FirstName = "admin",
                    LastName = "ce",
                    PasswordHash = passwordHasher.HashPassword("Qwerty123$%"),
                    DateOfBirth = DateTime.Today,
                    Gender = "m",
                    PostalCode = "1000",
                    City = "Bruxelles",
                    PhoneNumber = "06-12345678",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = true,
                    CreatedDate = DateTime.Now,
                    UserStatus = Enum.UserStatus.Active
                };

                var vendor = new User()
                {
                    UserName = "vendor@CE.com",
                    Email = "vendor@CE.com",
                    FirstName = "vendor",
                    LastName = "ce",
                    DateOfBirth = new DateTime(1988, 8, 19),
                    Gender = "m",
                    PostalCode = "1000",
                    City = "Bruxelles",
                    PhoneNumber = "06-13345678",
                    PasswordHash = passwordHasher.HashPassword("Qwerty123$"),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = true,
                    CreatedDate = DateTime.Now,
                    UserStatus = Enum.UserStatus.Active
                };


                var client = new User()
                {
                    UserName = "client@CE.com",
                    Email = "client@CE.com",
                    FirstName = "client",
                    LastName = "ce",
                    DateOfBirth = new DateTime(1988, 8, 19),
                    Gender = "m",
                    PostalCode = "1000",
                    City = "Bruxelles",
                    PhoneNumber = "06-22345678",
                    PasswordHash = passwordHasher.HashPassword("Qwerty123$"),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = true,
                    CreatedDate = DateTime.Now,
                    UserStatus = Enum.UserStatus.Active
                };

                

                context.Users.Add(superAdminUser);
                context.Users.Add(client);
                context.Users.Add(vendor);

                context.SaveChanges();

                superAdminUser.Roles.Add(new UserRole() { UserId = superAdminUser.Id, RoleId = superAdminRole.Id });
                vendor.Roles.Add(new UserRole() { UserId = vendor.Id, RoleId = vendorRole.Id });
                client.Roles.Add(new UserRole() { UserId = client.Id, RoleId = clientRole.Id });

                context.SaveChanges();

                //ExecuteSqlFile(context, "EnumResource.sql");

                RemoveAllForeignKey(context);
            }
            catch (Exception ex)
            {
                // delete database if initialization failed
                context.Database.Delete();
                throw ex;
            }
        }

        private void RemoveAllForeignKey(MainContext context)
        {
            var sql = @"DECLARE @ConstrainName VARCHAR(200), @Schema VARCHAR(200), @TableName VARCHAR(200)
DECLARE FK_CURSOR CURSOR FOR
SELECT
U.CONSTRAINT_NAME,
U.TABLE_SCHEMA,
U.TABLE_NAME
FROM
INFORMATION_SCHEMA.KEY_COLUMN_USAGE U
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS C
ON U.CONSTRAINT_NAME = C.CONSTRAINT_NAME
WHERE
C.CONSTRAINT_TYPE = 'FOREIGN KEY'

OPEN FK_CURSOR
FETCH NEXT FROM FK_CURSOR INTO @ConstrainName, @Schema, @TableName

WHILE @@FETCH_STATUS = 0   
BEGIN   
    EXEC ('ALTER TABLE [' + @Schema + '].[' + @TableName + '] DROP CONSTRAINT [' + @ConstrainName + ']')   

    FETCH NEXT FROM FK_CURSOR INTO @ConstrainName, @Schema, @TableName   
END   

CLOSE FK_CURSOR   
DEALLOCATE FK_CURSOR
";

            context.Database.ExecuteSqlCommand(sql);
        }

        private void ExecuteSqlFile(MainContext context, string embededFileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = typeof(MainDatabaseInitializer).Assembly.GetName().Name + "." + embededFileName;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string sql = reader.ReadToEnd();

                    context.Database.ExecuteSqlCommand(sql);
                }
            }
        }
    }
}
