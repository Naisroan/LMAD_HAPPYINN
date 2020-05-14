Sys.Application.add_load(function () {

    // esto para tener checkbox personalizados, elimina la etiqueta span que se crea por default en un CheckBox de ASP.NET
    $("div.custom-checkbox > span.custom-control-input").each(function (o, c) {

        var t = $(c),
            e = t.find("input[type=checkbox]").first(),
            p = t.parent("div.custom-checkbox").first(),
            l = p.find("label.custom-control-label").first();

        IsNill(e) || (
            e.addClass("custom-control-input"),
            e.prependTo(p),
            l.attr('for', e.attr('id')),
            t.remove()
        );
    });

});

/**
 * Función para verificar si una variable es nula o indefinida
 * @param {any} l
 */
const IsNill = l => (null === l || undefined === l);

/**
 * Función para mostrar un mensaje modal, usa SweetAlert versión 1.0, y la clase Msg.cs es quien utiliza esta función
 * @param {any} l
 * @param {any} n
 * @param {any} o
 * @param {any} t
 * @param {any} i
 */
const MsjModal = (l, n, o, t, i) => { if (swal({ title: l, text: n, icon: t, button: o, closeOnClickOutside: !1 }), null != i && null != i && "" != i) { var c = $(".swal-button--confirm"); null != c && null != c && $(".swal-button--confirm").on("click", () => { window.location = i; }) } };

/**
 * Función para mostrar un mensaje modal en una ventana modal, usa SweetAlert versión 1.0, y la clase Msg.cs es quien utiliza esta función
 * @param {any} id
 * @param {any} l
 * @param {any} n
 * @param {any} o
 * @param {any} t
 */
const MsjModalCRUD = (id, l = '', n = '', o = '', t = '') => { var mdl = $('#' + id); if (!IsNill(mdl)) mdl.modal('hide'); if (!IsNill(l)) { if (swal({ title: l, text: n, icon: t, button: o, closeOnClickOutside: !1 })) { var c = $(".swal-button--confirm"); null != c && null != c && $(".swal-button--confirm").on("click", () => { window.location = window.location; }) } } }

/**
 * Función para deshabilitar un botón al dar click, esto para evitar que de click muchas veces
 * @param {any} n
 * @param {any} s
 */
const onButtonLoading = (n, s = "") => { var r = $(n); !r.hasClass('aspNetDisabled') && (r.addClass("disabled"), r.text(s), r.append('<span class="spinner-border spinner-border-sm ml-1" role="status" aria-hidden="true"></span>')) };

/**
 * Función para mostrar una ventana modal de bootstrap
 * @param {any} id
 * @param {any} show
 */
const MostrarModal = (id, show) => { if (show) { $('#' + id).modal({ show: true, keyboard: false, backdrop: false }); } else { $('#' + id).modal('hide'); } }