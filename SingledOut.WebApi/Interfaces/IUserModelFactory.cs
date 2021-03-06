﻿using System.Collections.Generic;
using SingledOut.Data;
using SingledOut.Model;
using SingledOut.SearchParameters;

namespace SingledOut.WebApi.Interfaces
{
    public interface IUserModelFactory
    {
        UserModel Create(User user);

        IEnumerable<UserModel> Create(IEnumerable<User> users, UsersSearchParameters sp);

        User Parse(UserModel model);

        User ParseUpdate(User user, UserModel userModel);
    }
}
