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
            {UserPrefProps.SHOW_RDLGHT_CAMERAS, "xcfg__CFG_SRC" }
        };
        private bool __scams = true, __rcams = true;
        public bool CFG__SHOW_SCAMS { get => __scams; set => __scams = value; }
        public bool CFG__SHOW_RCAMS { get => __rcams; set => __rcams = value; }


        public static UserPref GenerateFromCOOKIES(IRequestCookieCollection cookies)
        {
            var up = new UserPref()
            {
                __rcams = bool.Parse(cookies["__CFG_SRC"] ?? "true"),
                __scams = bool.Parse(cookies["__CFG_SSC"] ?? "true")
            };
            return up;
        }

        public static UserPref GenerateFromSESSION(ISession session)
        {
            var up = new UserPref()
            {
                __rcams = bool.Parse(session.GetString(SNAMES[UserPrefProps.SHOW_RDLGHT_CAMERAS]) ?? "true"),
                __scams = bool.Parse(session.GetString(SNAMES[UserPrefProps.SHOW_STATIC_CAMERAS]) ?? "true")
            };
            return up;
        }
    }
    public enum UserPrefProps
    {
        SHOW_STATIC_CAMERAS,
        SHOW_RDLGHT_CAMERAS
    }
}
