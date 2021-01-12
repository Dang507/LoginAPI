using AutoMapper;
using JWTDemo.Models;
using System;




namespace JWTDemo.Services
{
    public class AutoMapper: Profile
    {
         public AutoMapper()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisModel, User>();
            CreateMap<UpdateModel, User>();
        }
           

    }
}
