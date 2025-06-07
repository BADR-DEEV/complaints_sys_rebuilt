using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using complaints_back.Dtos;
using complaints_back.DTOs;
using complaints_back.models.Users;

namespace complaints_back
{
    public class MappingDtos : Profile
    {

        public MappingDtos()
        {
            // CreateMap<Complain, AddComplainDto>();
            // CreateMap<Complain, UpdateComplainDto>();
            // CreateMap<UpdateComplainDto, Complain>();
            // CreateMap<AddComplainDto, Complain>();
            CreateMap<UserRegisterDto, User>();
            CreateMap<User, UserRegisterDto>();
            CreateMap<UserLoginDto, User>();
            CreateMap<User, UserLoginDto>();
            CreateMap<User, UserResponseDto>();
            CreateMap<UserResponseDto, User>();
            // CreateMap<User, PersonUserDto>();
            // CreateMap<PersonUserDto, User>();


            // CreateMap<UpdateCharacterDto, Character>();
        }

    }
}