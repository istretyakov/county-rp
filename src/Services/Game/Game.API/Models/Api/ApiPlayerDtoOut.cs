﻿using System;

namespace CountyRP.Services.Game.API.Models.Api
{
    public class ApiPlayerDtoOut
    {
        public int Id { get; }

        public string Login { get; }

        public string Password { get; }

        public DateTimeOffset RegistrationDate { get; }

        public DateTimeOffset LastVisitDate { get; }

        public ApiPlayerDtoOut(
            int id,
            string login,
            string password,
            DateTimeOffset registrationDate,
            DateTimeOffset lastVisitDate
        )
        {
            Id = id;
            Login = login;
            Password = password;
            RegistrationDate = registrationDate;
            LastVisitDate = lastVisitDate;
        }
    }
}
