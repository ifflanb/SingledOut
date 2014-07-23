using System;
using System.Security.Cryptography;
using System.Text;
using SingledOut.Data;
using SingledOut.Model;
using SingledOut.Data.Entities;

namespace SingledOut.UnitTests.WebApi
{
    /// <summary>
    /// Unit test data helper.
    /// </summary>
    public static class UnitTestDataHelper
    {
        public static string CreateHash(string unHashed)
        {
            var x = new MD5CryptoServiceProvider();
            var data = Encoding.ASCII.GetBytes(unHashed);
            data = x.ComputeHash(data);
            return Encoding.ASCII.GetString(data);
        }

        /// <summary>
        /// Creates a new user in the database.
        /// </summary>
        public static UserModel CreateDefaultUser()
        {
            var user = new User
            {
                FirstName = "Jo",
                Surname = "Bloggs",
                Sex = "male",
                CreatedDate = DateTime.UtcNow,
                FacebookAccessToken = "1234567890",
                FacebookUserName = "111111111111",
                UpdateDate = DateTime.UtcNow,
                Email = "jo.bloggs@test.com",
                Password = CreateHash("testpassword1"),
                Age = 41,
                AuthToken = Guid.NewGuid(),
                UserLocation = new UserLocation
                {
                    Latitude = -43.548339,
                    Longitude = 172.567666,
                    CreatedDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                }
            };

            int? id = null;
            using (var ctx = new SingledOutContext())
            {
                ctx.Users.Add(user);
                id = ctx.SaveChanges();
            }

            UserModel userModel = null;
            
            if (id > 0)
            {
                user.ID = (int) id;
                userModel = MapUser(user);
            }

            return userModel;
        }


        /// <summary>
        /// Creates a new user in the database.
        /// </summary>
        public static void CreateUser(UserModel userModel)
        {
            var user = new User
            {
                FirstName = userModel.FirstName,
                Surname = userModel.Surname,
                Sex = userModel.Sex,
                CreatedDate = userModel.CreatedDate,
                FacebookAccessToken = userModel.FacebookAccessToken,
                FacebookUserName = userModel.FacebookUserName,
                UpdateDate = userModel.UpdateDate,
                Email = userModel.Email,
                Password = CreateHash(userModel.Password),
                Age = userModel.Age,
                AuthToken = userModel.AuthToken,
                UserLocation = userModel.UserLocation != null ? MapUserLocation(userModel.UserLocation) : null
            };

            using (var ctx = new SingledOutContext())
            {
                ctx.Users.Add(user);
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// Maps a user location model to a user entity.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserModel MapUser(User user)
        {
            var userModel = new UserModel
            {
                FirstName = user.FirstName,
                Surname = user.Surname,
                Email = user.Email,
                FacebookAccessToken = user.FacebookAccessToken,
                FacebookUserName = user.FacebookUserName,
                Age = user.Age,
                Sex = user.Surname,
                AuthToken = user.AuthToken,
                CreatedDate = user.CreatedDate,
                UpdateDate = user.UpdateDate,
                Password = user.Password,
                ID = user.ID,
                UserLocation = MapUserLocationModel(user.UserLocation)
            };
            return userModel;
        }

        /// <summary>
        /// Maps a user location entity to a user location model.
        /// </summary>
        /// <param name="userLocation"></param>
        /// <returns></returns>
        public static UserLocationModel MapUserLocationModel(UserLocation userLocation)
        {
            var userLocationModel = new UserLocationModel
            {
                Latitude = userLocation.Latitude,
                Longitude = userLocation.Longitude,
                UserID = userLocation.UserID,
                CreatedDate = userLocation.CreatedDate,
                UpdateDate = userLocation.UpdateDate
            };
            return userLocationModel;
        }

        /// <summary>
        /// Maps a user location model to a user entity.
        /// </summary>
        /// <param name="userLocationModel"></param>
        /// <returns></returns>
        public static UserLocation MapUserLocation(UserLocationModel userLocationModel)
        {
            var userLocation = new UserLocation()
            {
                Latitude = userLocationModel.Latitude,
                Longitude = userLocationModel.Longitude,
                UserID = userLocationModel.UserID,
                CreatedDate = userLocationModel.CreatedDate,
                UpdateDate = userLocationModel.UpdateDate
            };
            return userLocation;
        }

        /// <summary>
        /// Creates a user location in the database.
        /// </summary>
        /// <param name="userLocationModel"></param>
        public static void CreateUserLocation(UserLocationModel userLocationModel)
        {
            var userLocation = MapUserLocation(userLocationModel);

            using (var ctx = new SingledOutContext())
            {
                ctx.UserLocations.Add(userLocation);
                ctx.SaveChanges();
            }
        }
    }
}
