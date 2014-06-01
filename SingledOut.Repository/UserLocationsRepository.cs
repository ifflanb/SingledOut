﻿using System.Linq;
using SingledOut.Data;
using SingledOut.Data.Entities;

namespace SingledOut.Repository
{
    public class UserLocationsRepository : BaseRepository, IUserLocationsRepository
    {
        private readonly SingledOutContext _ctx;

        public UserLocationsRepository(SingledOutContext ctx)
            : base(ctx)
        {
            _ctx = ctx;
        }


        public IQueryable<UserLocation> GetAllUserLocations()
        {
            return _ctx.UserLocations.AsQueryable();
        }

        public UserLocation GetUserLocation(int userLocationID)
        {
            return _ctx.UserLocations.SingleOrDefault(o => o.ID == userLocationID);
        }

        public int Insert(UserLocation userLocation)
        {
            _ctx.UserLocations.Add(userLocation);
            return SaveAll();
        }

        public int Update(UserLocation originalUserLocation, UserLocation updatedUserLocation)
        {
            _ctx.UserLocations.Attach(updatedUserLocation);
            var entry = _ctx.Entry(updatedUserLocation);

            if (originalUserLocation.Latitude != updatedUserLocation.Latitude)
            {
                entry.Property(e => e.Latitude).IsModified = true;
            }

            if (originalUserLocation.Longitude != updatedUserLocation.Longitude)
            {
                entry.Property(e => e.Longitude).IsModified = true;
            }

            if (originalUserLocation.CreatedDate != updatedUserLocation.CreatedDate)
            {
                entry.Property(e => e.CreatedDate).IsModified = true;
            }

            if (originalUserLocation.UpdateDate != updatedUserLocation.UpdateDate)
            {
                entry.Property(e => e.UpdateDate).IsModified = true;
            }

            return SaveAll();
        }

        public int DeleteUserLocation(int id)
        {
            var userLocation = GetUserLocation(id);
            _ctx.UserLocations.Remove(userLocation);
            return SaveAll();
        }
    }
}
