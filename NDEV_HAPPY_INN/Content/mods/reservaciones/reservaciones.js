// al cargarse el documento solo una vez (sin incluir los update panel)
$(document).ready(function () {

    // $('.selectpicker').selectpicker();
});

// al cargarse cada vez (incluyendo los update panel)
Sys.Application.add_load(function () {

    $('.selectpicker').selectpicker();
});