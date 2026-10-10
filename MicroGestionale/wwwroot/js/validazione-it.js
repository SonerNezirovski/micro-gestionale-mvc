// Validazione dei numeri in formato italiano (es. 12,50 oppure 1.234,50).
// jQuery Validate di default accetta solo il punto come separatore decimale.
(function ($) {
    if (!$ || !$.validator) {
        return;
    }

    var formatoItaliano = /^-?(\d{1,3}(\.\d{3})+|\d+)(,\d+)?$/;

    function leggiNumero(valore) {
        return parseFloat(valore.replace(/\./g, "").replace(",", "."));
    }

    $.validator.methods.number = function (valore, elemento) {
        return this.optional(elemento) || formatoItaliano.test(valore);
    };

    $.validator.methods.range = function (valore, elemento, limiti) {
        if (this.optional(elemento)) {
            return true;
        }
        var numero = leggiNumero(valore);
        return !isNaN(numero) && numero >= limiti[0] && numero <= limiti[1];
    };
})(window.jQuery);
