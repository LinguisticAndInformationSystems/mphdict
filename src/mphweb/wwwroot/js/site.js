var pclsMenuObj = (function () {
    "use strict";
    var pcls_search;
    var pclass_list;
    var entry;
    function _ready() {
        entry = document.querySelector('#wrapper > main > article > section');
        pcls_search = document.getElementById('search-pcls');
        if (pcls_search != null) {
            pcls_search.addEventListener("input", search_cls, false);
        }
        pclass_list = document.getElementById('pclass-list');
        if (pclass_list != null) {
            pclass_list.addEventListener("click", function (event) {
                var aTag = event.target;
                for(;;){
                    if (aTag.tagName === 'A') {
                        break;
                    }
                    aTag = aTag.parentNode;
                }
                var cls = aTag.id.substring(6);
                var uri = pclsBasePath + (pclsBasePath[pclsBasePath.length - 1] === '/' ? 'api/pcls/GetView/' : '/api/pcls/GetView/') + cls;
                rlsXhr.get(uri, function (req) {
                    var content = JSON.parse(req.responseText);
                    entry.innerHTML = content.html;
                    var selected_class = pclass_list.querySelector('.selected-word');
                    if (selected_class != null) selected_class.classList.remove('selected-word');
                    selected_class = document.getElementById('ptype-' + cls);
                    selected_class.classList.add('selected-word');
                }, rlsXhr.err);
                return return_false(event);
            }, false);
        }
    }

    function search_cls() {
        var val = pcls_search.value;
        var element = document.getElementById('ptype-' + val);
        if (element != null) {
            element.scrollIntoView(true);
        }
    }
    function _get_pcls() {

    }
    function return_false(event) {
        event.stopPropagation();
        event.preventDefault();
        return false;
    }

    return {
        ready: _ready,
        get_pcls: _get_pcls
    }
})();

(function () {
    "use strict";
    document.addEventListener("DOMContentLoaded", pclsMenuObj.ready, false);
})();
