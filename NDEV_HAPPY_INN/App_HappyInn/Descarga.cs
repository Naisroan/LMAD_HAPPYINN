using System;
using System.IO;
using System.Web;
using System.Web.UI;

public class Descarga : IDisposable
{
    #region "ATRIBUTOS"

    private string nombre;
    private string extension;
    private byte[] contenido;
    private string rutaArchivo;
    private bool borrarArchivo;

    #endregion

    public Descarga()
    {
        this.Nombre = "";
        this.Extension = "";
        this.RutaArchivo = "";
        this.BorrarArchivo = false;
    }

    public Descarga(string nombre, string extension, byte[] contenido)
    {
        this.Nombre = nombre;
        this.Extension = extension;
        this.Contenido = contenido;
        this.BorrarArchivo = false;
    }

    public Descarga(string nombre, string extension, string rutaArchivo, bool borrarDespues)
    {
        this.Nombre = nombre;
        this.Extension = extension;
        this.RutaArchivo = rutaArchivo;
        this.BorrarArchivo = borrarDespues;
    }

    #region "PROPIEDADES"

    public string Nombre
    {
        get { return this.nombre; }
        set { this.nombre = value; }
    }

    public string Extension
    {
        get { return this.extension; }
        set { this.extension = value; }
    }

    public byte[] Contenido
    {
        get { return this.contenido; }
        set { this.contenido = value; }
    }

    public string RutaArchivo
    {
        get { return this.rutaArchivo; }
        set { this.rutaArchivo = value; }
    }

    public bool BorrarArchivo
    {
        get { return this.borrarArchivo; }
        set { this.borrarArchivo = value; }
    }

    #endregion

    #region "MÉTODOS"

    public string tipoArchivo(string extension)
    {
        string tipo = "";
        string subTipo = "";

        if (extension == ".txt")
        {
            tipo = "text";
            subTipo = "plain";
        }
        else if (extension == ".csv")
        {
            tipo = "text";
            subTipo = "csv";
        }
        else if (extension == ".pdf")
        {
            tipo = "application";
            subTipo = "pdf";
        }
        else if (extension == ".xlsx")
        {
            tipo = "application";
            subTipo = "vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }
        else if (extension == ".xls")
        {
            tipo = "application";
            subTipo = "vnd.ms-excel";
        }
        else if (extension == ".docx")
        {
            tipo = "application";
            subTipo = "vnd.openxmlformats-officedocument.wordprocessingml.document";
        }
        else if (extension == ".doc" || extension == ".dot")
        {
            tipo = "application";
            subTipo = "msword";
        }
        else if (extension == ".ppsx")
        {
            tipo = "application";
            subTipo = "vnd.openxmlformats-officedocument.presentationml.presentation";
        }
        else if (extension == ".pps" || extension == ".ppsm")
        {
            tipo = "application";
            subTipo = "vnd.ms-powerpoint";
        }
        else if (extension == ".7z")
        {
            tipo = "application";
            subTipo = "x-7z-compressed";
        }
        else if (extension == ".zip")
        {
            tipo = "application";
            subTipo = "zip";
        }
        else if (extension == ".rar")
        {
            tipo = "application";
            subTipo = "x-rar-compressed";
        }
        else if (extension == ".bmp")
        {
            tipo = "image";
            subTipo = "bmp";
        }
        else if (extension == ".png")
        {
            tipo = "image";
            subTipo = "png";
        }
        else if (extension == ".gif")
        {
            tipo = "image";
            subTipo = "gif";
        }
        else if (extension == ".jpg" || extension == ".jpeg")
        {
            tipo = "image";
            subTipo = "jpeg";
        }
        else if (extension == ".mp4")
        {
            tipo = "video";
            subTipo = "mp4";
        }
        else if (extension == ".avi")
        {
            tipo = "video";
            subTipo = "x-msvideo";
        }
        else if (extension == ".mpeg")
        {
            tipo = "video";
            subTipo = "mpeg";
        }
        else if (extension == ".webm")
        {
            tipo = "video";
            subTipo = "webm";
        }
        else
        {
            tipo = "application";
            subTipo = "octet-stream";
        }

        return $"{tipo}/{subTipo}";
    }

    public void ObtenerArchivo(HttpContext httpContext)
    {
        try
        {
            if (!string.IsNullOrEmpty(RutaArchivo))
            {
                Contenido = File.ReadAllBytes(RutaArchivo);
            }

            httpContext.Response.Clear();
            httpContext.Response.Buffer = true;
            httpContext.Response.Charset = "";
            httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            httpContext.Response.ContentType = tipoArchivo(Extension);
            httpContext.Response.AppendHeader("Content-Disposition", $"attachment; filename={Nombre}{Extension}");
            httpContext.Response.BinaryWrite(Contenido);
            httpContext.Response.Flush();
            httpContext.Response.SuppressContent = true;
            httpContext.ApplicationInstance.CompleteRequest();

            if (!string.IsNullOrEmpty(RutaArchivo) && BorrarArchivo)
            {
                File.Delete(RutaArchivo);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void DescargarArchivo(Page page, HttpContext httpContext)
    {
        httpContext.Session["descarga"] = this;
        Sys.JavaScript(page, "openUrl", "/download.aspx");
    }

    public void Dispose()
    {
    }

    #endregion
}