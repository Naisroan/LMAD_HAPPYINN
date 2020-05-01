using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

public static class Sys
{
    public static void SignOut(HttpContext httpContext)
    {
        httpContext.Session.Abandon();
        httpContext.Session.Clear();
        FormsAuthentication.SignOut();
        FormsAuthentication.RedirectToLoginPage();
    }
}