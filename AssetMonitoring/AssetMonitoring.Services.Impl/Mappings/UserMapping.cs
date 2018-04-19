namespace AssetMonitoring.Services.Impl.Mappings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AssetMonitoring.Contracts;
    using AssetMonitoring.Contracts.Enums;

    public class UserMapping
    {

        public IQueryable<User> Map(IQueryable<Entities.User> source)
        {
            return from s in source
                   select new User
                   {
                       Id = s.Id,
                       Name = s.Name,
                       Email = s.Email,
                       B2cIdentifier = s.B2cIdentifier,
                       RoleId = s.RoleId.Value,
                       Role = (UserRole)Enum.Parse(typeof(UserRole), s.Role.Name)
                   };
        }

        public User Map(Entities.User source)
        {
            return source == null ? null : this.Map(new List<Entities.User> { source }.AsQueryable()).First();
        }
    }
}
