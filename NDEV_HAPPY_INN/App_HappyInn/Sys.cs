using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public static class Sys
{
    #region "CONSTANTES"

    private const string NOMBRE_FUNCION_JS_MOSTRAR_MODAL = "MostrarModal";

    #endregion

    public static void SignOut(HttpContext httpContext)
    {
        httpContext.Session.Abandon();
        httpContext.Session.Clear();
        FormsAuthentication.SignOut();
        FormsAuthentication.RedirectToLoginPage();
    }

    public static int FillGridView<T>(GridView gridView, List<T> source, bool addThead = false)
    {
        try
        {
            gridView.DataSource = source;
            gridView.DataBind();

            if (addThead && source != null && gridView.Rows.Count > 0)
            {
                gridView.HeaderRow.TableSection = TableRowSection.TableHeader;
            }

            return source != null && gridView.Rows.Count > 0 ? gridView.Rows.Count : 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static void ShowModal(Page page, UpdatePanel modal)
    {
        JavaScript(page, NOMBRE_FUNCION_JS_MOSTRAR_MODAL, modal.ClientID, true);
    }

    private static void JavaScript(Page page, string nomFuncion, params object[] parametros)
    {
        string funcionCompleta = ConstruirJs(nomFuncion, parametros);

        ScriptManager.RegisterStartupScript(
            page,
            page.GetType(),
            "scriptServerPage",
            funcionCompleta,
            true
        );
    }

    private static string ConstruirJs(string nomFuncion, params Object[] parametros)
    {
        string funcion = "";

        if (parametros.Length == 0)
        {
            funcion = $@"{nomFuncion}()";
        }
        else
        {
            funcion = $@"{nomFuncion}(";

            foreach (object param in parametros)
            {
                Type typeParam = param.GetType();

                if (typeParam.IsEquivalentTo(typeof(string)))
                    funcion = $@"{funcion}'{param}',";
                else if (typeParam.IsEquivalentTo(typeof(bool)))
                    funcion = $@"{funcion}{param.ToString().ToLower()},";
                else
                    funcion = $@"{funcion}{param},";
            }

            funcion = $@"{funcion.TrimEnd(',')});";
        }

        return funcion;
    }
}