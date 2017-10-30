var analyseObj = (function () {
    "use strict";
    let hubUrl = basePath+'/analyse';
    let httpConnection = new signalR.HttpConnection(hubUrl);
    let hubConnection = new signalR.HubConnection(httpConnection);
    function _ready() {
        hubConnection.on("Send", function (data) {
            var analyseResult = document.getElementById("analyseResult");
            analyseResult.value = data;
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
