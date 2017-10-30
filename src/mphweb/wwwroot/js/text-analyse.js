let hubUrl = 'http://localhost:59737/analyse';
let httpConnection = new signalR.HttpConnection(hubUrl);
let hubConnection = new signalR.HubConnection(httpConnection);
var analyseObj = (function () {
    "use strict";
    var pcls_search;
    var pclass_list;
    var entry;
    var lang_chb;
    var is_head_chb;
    function _ready() {
        hubConnection.on("Send", function (data) {
            document.getElementById("analyseResult").innerText = data;
        });
        document.getElementById("performBtn").addEventListener("click", function (e) {
            let message = document.getElementById("initialText").value;
            hubConnection.invoke('Send', message);
        });
        hubConnection.start();
    }

    function return_false(event) {
        event.stopPropagation();
        event.preventDefault();
        return false;
    }

    return {
        ready: _ready
    }
})();

(function () {
    "use strict";
    document.addEventListener("DOMContentLoaded", analyseObj.ready, false);
})();
