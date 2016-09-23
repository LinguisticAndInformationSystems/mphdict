var pclsMenuObj = (function () {
    "use strict";
    var pcls_search;

    function _ready() {
        pcls_search = document.getElementById('search-pcls');
        if (pcls_search != null) {
            pcls_search.addEventListener("input", search_cls, false);
        }
    }

    function search_cls() {
        var val = pcls_search.value;
        var element = document.getElementById('ptype-' + val);
        if (element != null) {
            element.scrollIntoView(true);
        }
    }

    return {
        ready: _ready

    }
})();

(function () {
    "use strict";
    document.addEventListener("DOMContentLoaded", pclsMenuObj.ready, false);
})();
