using NDEV_HAPPY_INN;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

public static class Sys
{
    #region "CONSTANTES"

    private const string NOMBRE_FUNCION_JS_MOSTRAR_MODAL = "MostrarModal";

    #endregion

    public static void SignOut(HttpContext httpContext, bool clearCookies = false)
    {
        var cookieRecordarUsuario = httpContext.Request.Cookies.Get(login.NAME_COOKIE_RECORDAR_USUARIO);

        if (clearCookies && cookieRecordarUsuario != null)
        {
            cookieRecordarUsuario.Value = string.Empty;
            httpContext.Response.Cookies.Set(cookieRecordarUsuario);
        }

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

    public static int FillRepeater<T>(Repeater repeater, List<T> listSource)
    {
        try
        {
            repeater.DataSource = listSource;
            repeater.DataBind();

            return listSource != null ? repeater.Items.Count : 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static int FillRepeater<T, N>(Repeater repeater, IDictionary<T, N> listSource)
    {
        try
        {
            repeater.DataSource = listSource.Count > 0 ? listSource : null;
            repeater.DataBind();

            return repeater.Items.Count > 0 ? repeater.Items.Count : 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static int FillDropDown(DropDownList dropDownList, List<string> source, string textoSeleccion = "Seleccione...", bool textoSeleccionTop = true)
    {
        try
        {
            dropDownList.Items.Clear();
            dropDownList.DataValueField = string.Empty;
            dropDownList.DataTextField = string.Empty;
            dropDownList.DataSource = source != null && source.Count > 0 ? source : null;
            dropDownList.DataBind();
            dropDownList.Items.Insert(textoSeleccionTop ? 0 : dropDownList.Items.Count, new ListItem(textoSeleccion, "-1"));
            dropDownList.SelectedIndex = 0;

            return dropDownList.Items.Count;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static int FillDropDown<T>(DropDownList dropDownList, List<T> source, string textoSeleccion = "Seleccione...", bool textoSeleccionTop = true)
    {
        try
        {
            dropDownList.Items.Clear();
            dropDownList.DataValueField = string.Empty;
            dropDownList.DataTextField = string.Empty;
            dropDownList.DataSource = source != null && source.Count > 0 ? source : null;
            dropDownList.DataBind();
            dropDownList.Items.Insert(textoSeleccionTop ? 0 : dropDownList.Items.Count, new ListItem(textoSeleccion, "-1"));
            dropDownList.SelectedIndex = 0;

            return dropDownList.Items.Count;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static int FillDropDown<T>(DropDownList dropDownList, List<T> source, 
        string valueField, string textField, string textoSeleccion = "Seleccione...", bool textoSeleccionTop = true)
    {
        try
        {
            dropDownList.Items.Clear();
            dropDownList.DataValueField = valueField;
            dropDownList.DataTextField = textField;
            dropDownList.DataSource = source != null && source.Count > 0 ? source : null;
            dropDownList.DataBind();
            dropDownList.Items.Insert(textoSeleccionTop ? 0 : dropDownList.Items.Count, new ListItem(textoSeleccion, "-1"));
            dropDownList.SelectedIndex = 0;

            return dropDownList.Items.Count;
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

    public static void JavaScript(Page page, string nomFuncion, params object[] parametros)
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

    public static bool EsNumero(object valor, bool flotante = false)
    {
        if (valor == null)
            return false;

        if (string.IsNullOrEmpty(Convert.ToString(valor)))
            return false;

        if (!flotante)
        {
            int test = -1;
            return int.TryParse(Convert.ToString(valor), out test);
        }
        else
        {
            decimal test = 0;
            return decimal.TryParse(Convert.ToString(valor), out test);
        }
    }

    public static bool EsFecha(object valor)
    {
        if (valor == null)
            return false;

        if (string.IsNullOrEmpty(Convert.ToString(valor)))
            return false;

        DateTime test;
        return DateTime.TryParse(Convert.ToString(valor), out test);
    }
}