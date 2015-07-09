#region

using AutoMapper;
using Fesslersoft.SharedFolderSpy.Native;
using Fesslersoft.SharedFolderSpy.Native.Entities;

#endregion

namespace Fesslersoft.SharedFolderSpy.Shared
{
    public static class Maps
    {
        public static void Init()
        {
            #region NEtFileEnumResult <-> Win32.FileInfo3

            Mapper.CreateMap<Win32.FileInfo3, NEtFileEnumResult>()
                .ForMember(dest => dest.Id, map => map.MapFrom(src => src.SessionID))
                .ForMember(dest => dest.Permission, map => map.MapFrom(src => src.Permission))
                .ForMember(dest => dest.Pathname, map => map.MapFrom(src => src.PathName))
                .ForMember(dest => dest.NumberOfLocks, map => map.MapFrom(src => src.NumLocks))
                .ForMember(dest => dest.Username, map => map.MapFrom(src => src.UserName));
            Mapper.CreateMap<NEtFileEnumResult, Win32.FileInfo3>()
                .ForMember(dest => dest.SessionID, map => map.MapFrom(src => src.Id))
                .ForMember(dest => dest.Permission, map => map.MapFrom(src => src.Permission))
                .ForMember(dest => dest.PathName, map => map.MapFrom(src => src.Pathname))
                .ForMember(dest => dest.NumLocks, map => map.MapFrom(src => src.NumberOfLocks))
                .ForMember(dest => dest.UserName, map => map.MapFrom(src => src.Username));

            #endregion

            #region NetSessionEnumResult <-> Win32.SessionInfo502

            Mapper.CreateMap<Win32.SessionInfo502, NetSessionEnumResult>()
                .ForMember(dest => dest.ClientType, map => map.MapFrom(src => src.ClientType))
                .ForMember(dest => dest.ComputerName, map => map.MapFrom(src => src.ComputerName))
                .ForMember(dest => dest.NumOpens, map => map.MapFrom(src => src.NumOpens))
                .ForMember(dest => dest.SecondsActive, map => map.MapFrom(src => src.SecondsActive))
                .ForMember(dest => dest.SecondsIdle, map => map.MapFrom(src => src.SecondsIdle))
                .ForMember(dest => dest.Transport, map => map.MapFrom(src => src.Transport))
                .ForMember(dest => dest.UserFlags, map => map.MapFrom(src => src.UserFlags))
                .ForMember(dest => dest.UserName, map => map.MapFrom(src => src.UserName));
            Mapper.CreateMap<NetSessionEnumResult, Win32.SessionInfo502>()
                .ForMember(dest => dest.ClientType, map => map.MapFrom(src => src.ClientType))
                .ForMember(dest => dest.ComputerName, map => map.MapFrom(src => src.ComputerName))
                .ForMember(dest => dest.NumOpens, map => map.MapFrom(src => src.NumOpens))
                .ForMember(dest => dest.SecondsActive, map => map.MapFrom(src => src.SecondsActive))
                .ForMember(dest => dest.SecondsIdle, map => map.MapFrom(src => src.SecondsIdle))
                .ForMember(dest => dest.Transport, map => map.MapFrom(src => src.Transport))
                .ForMember(dest => dest.UserFlags, map => map.MapFrom(src => src.UserFlags))
                .ForMember(dest => dest.UserName, map => map.MapFrom(src => src.UserName));

            #endregion

            #region NetShareInfoResult <-> Win32.ShareInfo2

            Mapper.CreateMap<Win32.ShareInfo2, NetShareInfoResult>()
                .ForMember(dest => dest.CurrentUsers, map => map.MapFrom(src => src.CurrentUsers))
                .ForMember(dest => dest.MaxUsers, map => map.MapFrom(src => src.MaxUsers))
                .ForMember(dest => dest.NetName, map => map.MapFrom(src => src.NetName))
                .ForMember(dest => dest.Password, map => map.MapFrom(src => src.Password))
                .ForMember(dest => dest.Path, map => map.MapFrom(src => src.Path))
                .ForMember(dest => dest.Permissions, map => map.MapFrom(src => src.Permissions))
                .ForMember(dest => dest.Remark, map => map.MapFrom(src => src.Remark))
                .ForMember(dest => dest.ShareType, map => map.MapFrom(src => src.ShareType));
            Mapper.CreateMap<NetShareInfoResult, Win32.ShareInfo2>()
                .ForMember(dest => dest.CurrentUsers, map => map.MapFrom(src => src.CurrentUsers))
                .ForMember(dest => dest.MaxUsers, map => map.MapFrom(src => src.MaxUsers))
                .ForMember(dest => dest.NetName, map => map.MapFrom(src => src.NetName))
                .ForMember(dest => dest.Password, map => map.MapFrom(src => src.Password))
                .ForMember(dest => dest.Path, map => map.MapFrom(src => src.Path))
                .ForMember(dest => dest.Permissions, map => map.MapFrom(src => src.Permissions))
                .ForMember(dest => dest.Remark, map => map.MapFrom(src => src.Remark))
                .ForMember(dest => dest.ShareType, map => map.MapFrom(src => src.ShareType));

            #endregion
        }
    }
}