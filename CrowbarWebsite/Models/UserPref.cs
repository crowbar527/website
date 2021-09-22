using CrowbarWebsite.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowbarWebsite.Models
{
    public class UserPref
    {
        public static Dictionary<UserPrefProps, string> SNAMES = new Dictionary<UserPrefProps, string>()
        {
            {UserPrefProps.SHOW_STATIC_CAMERAS, "xcfg__CFG_SSC" },
            {UserPrefProps.SHOW_RDLGHT_CAMERAS, "xcfg__CFG_SRC" },
            {UserPrefProps.SHOW_ANAAAL_AREA, "xcfg__CFG_AREA" },
            {UserPrefProps.SHOW_ANAAAL_CAS, "xcfg__CFG_CAS" },
            {UserPrefProps.PREF_AREAA_RADIUS, "xcfg__CFG_AREARADIUS" },
            {UserPrefProps.PREF_ADVFL_MAXCAS, "xcfg__CFG_MAXCAS" },
            {UserPrefProps.PREF_ADVFL_MINCAS, "xcfg__CFG_MINCAS" },
        };
        private bool __scams = true, __rcams = true, __aarea = true, __acas = true;
        private int __arad = 200, __cmax = 9999, __cmin = 0;
        public bool CFG__SHOW_SCAMS { get => __scams; set => __scams = value; }
        public bool CFG__SHOW_RCAMS { get => __rcams; set => __rcams = value; }
        public bool CFG__SHOW_AREA => __aarea;
        public bool CFG__SHOW_CAS => __acas;
        public int CFG__AREA_RADIUS => __arad;
        public int CFG__ADVF_MAXCAS => __cmax;
        public int CFG__ADVF_MINCAS => __cmin;

        [Obsolete]
        public static UserPref GenerateFromCOOKIES(IRequestCookieCollection cookies)
        {
            var up = new UserPref()
            {
                __rcams = bool.Parse(cookies["xcfg__CFG_SSC"] ?? "true"),
                __scams = bool.Parse(cookies["xcfg__CFG_SSC"] ?? "true"),
                __aarea = bool.Parse(cookies["xcfg__CFG_SSC"] ?? "true"),
                __acas = bool.Parse(cookies["xcfg__CFG_SSC"] ?? "true")
            };
            return up;
        }

        public static UserPref GenerateFromSESSION(ISession session)
        {
            var up = new UserPref()
            {
                __rcams = bool.Parse(session.GetString(SNAMES[UserPrefProps.SHOW_RDLGHT_CAMERAS]) ?? "true"),
                __scams = bool.Parse(session.GetString(SNAMES[UserPrefProps.SHOW_STATIC_CAMERAS]) ?? "true"),
                __aarea = bool.Parse(session.GetString(SNAMES[UserPrefProps.SHOW_ANAAAL_AREA]) ?? "true"),
                __acas = bool.Parse(session.GetString(SNAMES[UserPrefProps.SHOW_ANAAAL_CAS]) ?? "true"),
                __arad = int.Parse(session.GetString(SNAMES[UserPrefProps.PREF_AREAA_RADIUS]) ?? CacheService.CASRadiusInMeters.ToString()),
                __cmax = int.Parse(session.GetString(SNAMES[UserPrefProps.PREF_ADVFL_MAXCAS]) ?? "999"),
                __cmin = int.Parse(session.GetString(SNAMES[UserPrefProps.PREF_ADVFL_MINCAS]) ?? "0")
            };

            if (up.__cmax < up.__cmin)
            {
                var cmax = up.__cmax;
                up.__cmax = up.__cmin;
                up.__cmin = cmax;
            }
            up.__cmin = Math.Max(0, up.__cmin);
            up.__cmax = Math.Min(999, up.__cmax);

            return up;
        }
    }
    public enum UserPrefProps
    {
        SHOW_STATIC_CAMERAS,
        SHOW_RDLGHT_CAMERAS,
        SHOW_ANAAAL_AREA,
        SHOW_ANAAAL_CAS,
        PREF_AREAA_RADIUS,
        PREF_ADVFL_MAXCAS,
        PREF_ADVFL_MINCAS
    }
}
