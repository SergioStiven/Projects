// Created by @conmarap.

Array.prototype.search = function (elem) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == elem) return i;
    }
    return -1;
};

var Multiselect = function (selector) {
    if (!$(selector)) {
        console.error("ERROR: Element %s does not exist.", selector);
        return;
    }

    this.selector = selector;
    this.selections = [];

    (function (that) {
        that.events();
    })(this);
};

Multiselect.prototype = {
    open: function (that) {
        var target = $(that).parent().attr("data-target");

        // If we are not keeping track of this one's entries, then
        // start doing so.
        if (!this.selections) {
            this.selections = [];
        }

        $(this.selector + ".multiselect").toggleClass("active");
    },

    close: function () {
        $(this.selector + ".multiselect").removeClass("active");
    },

    events: function () {
        var that = this;

        $(document).on("click", that.selector + ".multiselect > .title", function (e) {
            if (e.target.className.indexOf("close-icon") < 0) {
                that.open();
            }
        });      

        $(document).on("click", that.selector + ".multiselect option", function (e) {
            // The original code set the value from option to title to show selected value make join with ',' 
            var selection = $(this).attr("value");

            // Actualiza array de cada lista para utilizarlo después con angularjs. Se debe especificar el id del control
            
            that.addItemsToArrayForAngularJS(selection, that.getVariableAngularFromSelector())
            
            // get text from selected option
            var selectionText = $(this).text();
            var target = $(this).parent().parent().attr("data-target");

            var io = that.selections.search(selectionText);

            if (io < 0) that.selections.push(selectionText);
            else that.selections.splice(io, 1);

            that.selectionStatus();
            that.setSelectionsString();
        });

        $(document).on("click", that.selector + ".multiselect > .title > .close-icon", function (e) {
            that.clearSelections();
        });

        $(document).on("blur", that.selector, function (e) {
            that.close();
        });

        $(document).click(function (e) {
            if (e.target.className.indexOf("title") < 0) {
                if (e.target.className.indexOf("text") < 0) {
                    if (e.target.className.indexOf("-icon") < 0) {
                        if (e.target.className.indexOf("selected") < 0 ||
                           e.target.localName != "option") {
                            //that.close(); // oculta la lista de opciones después de que deselecciona una opción
                        }
                    }
                }
            }
        });
    },

    selectionStatus: function () {
        var obj = $(this.selector + ".multiselect");

        if (this.selections.length) obj.addClass("selection");
        else obj.removeClass("selection");
    },

    clearSelections: function () {
        this.selections = [];
        this.selectionStatus();
        this.setSelectionsString();
        if (this.selector == '#categories') categories = []
        else if (this.selector == '#countries') countries = []
        else if (this.selector == '#plans') plans = []
        else if (this.selector == '#lenguages') lenguages = [];
    },

    getSelections: function () {
        return this.selections;
    },

    setSelectionsString: function () {
        var selects = this.getSelectionsString().split(", ");
        $(this.selector + ".multiselect > .title").attr("title", selects);
        
        var opts = $(this.selector + ".multiselect option");
        
        if (selects.length > 3) {
            var _selects = this.getSelectionsString().substr(0,24);
            //_selects = _selects.splice(0, 3);
            //var _selects = this.getSelectionsString().split(", ");
            //_selects = _selects.splice(0, 3);
            $(this.selector + ".multiselect > .title > .text")
                .text(_selects + " [...]"); // texto cuando es muy largo
        }
        else {
            $(this.selector + ".multiselect > .title > .text")
                .text(selects); // texto visible para el usuario
        }

        for (var i = 0; i < opts.length; i++) {
            $(opts[i]).removeClass("selected");
        }

        for (var j = 0; j < selects.length; j++) {
            var select = selects[j];

            for (var i = 0; i < opts.length; i++) {
                // Add class to current option selected
                if ($(opts[i]).text() == select) {
                    $(opts[i]).addClass("selected");
                    break;
                }
                // coments is because the original code was for id not text
                //if ($(opts[i]).attr("value") == select) {
                //    $(opts[i]).addClass("selected");
                //    break;
                //}
            }
        }
    },

    getSelectionsString: function () {
        if (this.selections.length > 0)
            return this.selections.join(", ");
        else return "Seleccione";
    },

    setSelections: function (arr) {
        if (!arr[0]) {
            console.error("ERROR: This does not look like an array.");
            return;
        }

        this.selections = arr;
        this.selectionStatus();
        this.setSelectionsString();
        
    },
    addItemsToArrayForAngularJS: function (newFilter, filters) {
        var found = $.inArray(newFilter, filters);
        if (found >= 0) {
            // Element was found, remove it.
            filters.splice(found, 1);
        } else {
            // Element was not found, add it.
            filters.push(newFilter);
        }
    },
    getVariableAngularFromSelector: function(selector) {
        if (this.selector == '#categories') return categories
        else if (this.selector == '#countries') return countries
        else if (this.selector == '#plans') return plans
        else if (this.selector == '#lenguages') return lenguages
        else return null;
    }
};

$(document).ready(function () {
    var multi = new Multiselect("#countries");
    var multi2 = new Multiselect("#categories");
    var multi3 = new Multiselect("#plans");
    var multi4 = new Multiselect("#lenguages");

});
