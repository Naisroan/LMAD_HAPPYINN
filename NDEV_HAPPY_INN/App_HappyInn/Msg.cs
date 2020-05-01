using System.Web.UI;

public static class Msg
{
    public const string CaptionError = "Ha ocurrido un error";
    public const string CaptionEspera = "¡Espere!";
    public const string CaptionFinalizado = "¡Exito!";
    public const string CaptionInfo = "Atención";
    public const string BotonOk = "Continuar";
    public const string BotonFinalizado = "Excelente";
    public const string BotonEspera = "Entendido";
    public const string BotonInfo = "Entendido";

    private const string NomFuncionJs = "MsjModal";
    public const string NomFuncionCRUDJs = "MsjModalCRUD";

    public enum Tipo
    {
        Error,
        Information,
        Success,
        Warning
    }

    /// <summary>
    /// Ejecuta una función JavaScript por medio de clase Page que abre un mensaje de forma modal.
    /// </summary>
    /// <param name="page">Page que ejecutará la función JavaScript</param>
    /// <param name="titulo">Titulo del mensaje modal</param>
    /// <param name="texto">Texto del mensaje</param>
    /// <param name="textoBoton">Texto del botón (por default es "Entendido")</param>
    /// <param name="tipoMensaje">Tipo de mensaje del modal (establece el icono y por default es el de información)</param>
    /// <param name="redirectUrl">Ingrese una url si desea redireccionar a otra página despues de dar click en el botón del modal</param>
    public static void Show
    (
        Page page,
        string titulo,
        string contenido,
        Tipo tipoMensaje = Tipo.Information,
        string boton = BotonInfo,
        string redireccionUrl = ""
    )
    {
        JavaScript
        (
            page,
            NomFuncionJs,
            titulo,
            contenido,
            boton,
            ValorTipoMensaje(tipoMensaje),
            redireccionUrl
        );
    }

    /// <summary>
    /// Ejecuta una función JavaScript por medio de un UpdatePanel que abre un mensaje de forma modal.
    /// </summary>
    /// <param name="page">UpdatePanel que ejecutará la función JavaScript</param>
    /// <param name="titulo">Titulo del mensaje modal</param>
    /// <param name="texto">Texto del mensaje</param>
    /// <param name="textoBoton">Texto del botón (por default es "Entendido")</param>
    /// <param name="tipoMensaje">Tipo de mensaje del modal (establece el icono y por default es el de información)</param>
    /// <param name="redirectUrl">Ingrese una url si desea redireccionar a otra página despues de dar click en el botón del modal</param>
    public static void Show
    (
        UpdatePanel updatePanel,
        string titulo,
        string contenido,
        Tipo tipoMensaje = Tipo.Information,
        string boton = BotonInfo,
        string redireccionUrl = ""
    )
    {
        JavaScript
        (
            updatePanel,
            NomFuncionJs,
            titulo,
            contenido,
            boton,
            ValorTipoMensaje(tipoMensaje),
            redireccionUrl
        );
    }

    public static string ValorTipoMensaje(Tipo tipoMensaje)
    {
        string icon = "";

        switch (tipoMensaje)
        {
            default:
            case Tipo.Information:
                {
                    icon = "info";
                    break;
                }

            case Tipo.Error:
                {
                    icon = "error";
                    break;
                }

            case Tipo.Success:
                {
                    icon = "success";
                    break;
                }

            case Tipo.Warning:
                {
                    icon = "warning";
                    break;
                }
        }

        return icon;
    }

    /// <summary>
    /// Método para invocar una función de javascript.
    /// </summary>
    /// <param name="updatePanel">UpdatePanel que ejecutará el script.</param>
    /// <param name="nomFuncion">Nombre de la función de javascript.</param>
    /// <param name="parametros">Parametros de la función de javascript.</param>
    public static void JavaScript(UpdatePanel updatePanel, string nomFuncion, params object[] parametros)
    {
        string funcionCompleta = ConstruirJs(nomFuncion, parametros);

        ScriptManager.RegisterStartupScript(
            updatePanel,
            updatePanel.GetType(),
            "scriptServerUpdatePanel",
            funcionCompleta,
            true
        );
    }

    /// <summary>
    /// Método para invocar una función de javascript.
    /// </summary>
    /// <param name="page">Page que ejecutará el script.</param>
    /// <param name="nomFuncion">Nombre de la función de javascript.</param>
    /// <param name="parametros">Parametros de la función de javascript.</param>
    /// <returns></returns>
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

    /// <summary>
    /// Método para la construcción de una función de javascript.
    /// </summary>
    /// <param name="nomFuncion">Nombre de la función de JavaScript</param>
    /// <param name="parametros">Parametros de la función</param>
    /// <returns></returns>
    private static string ConstruirJs(string nomFuncion, params object[] parametros)
    {
        string funcion = "";

        if (parametros.Length == 0)
        {
            funcion = $@"{nomFuncion}()";
        }
        else
        {
            funcion = $@"{nomFuncion}(";

            foreach (string param in parametros)
            {
                if (param.GetType().IsEquivalentTo(typeof(string)))
                {
                    funcion = $@"{funcion}'{param}',";
                }
                else
                {
                    funcion = $@"{funcion}{param},";
                }
            }

            funcion = $@"{funcion.TrimEnd(',')});";
        }

        return funcion;
    }
}