var rlsBase = (function () {
    function _generateUUID() {
        var d = new Date().getTime();
        var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (d + Math.random() * 16) % 16 | 0;
            d = Math.floor(d / 16);
            return (c == 'x' ? r : (r & 0x7 | 0x8)).toString(16);
        });
        return uuid;
    }
    function _are_cookies_enabled() {
        var cookieEnabled = (navigator.cookieEnabled) ? true : false;

        if (typeof navigator.cookieEnabled == "undefined" && !cookieEnabled) {
            document.cookie = "testcookie";
            cookieEnabled = (document.cookie.indexOf("testcookie") != -1) ? true : false;
        }
        return (cookieEnabled);
    }
    function _loadCss(url, appendTo) {
        var link = document.createElement('link');

        if (!appendTo) {
            appendTo = document.getElementsByTagName('head')[0];
        }

        link = document.createElement('link');
        link.type = 'text/css';
        link.rel = 'stylesheet';
        link.href = url;

        appendTo.appendChild(link);

        return link;
    }

    function _loadScript(src, callback, appendTo) {
        var script = document.createElement('script');

        if (!appendTo) {
            appendTo = document.getElementsByTagName('head')[0];
        }

        if (script.readyState && !script.onload) {
            // IE, Opera
            script.onreadystatechange = function () {
                if (script.readyState == "loaded" || script.readyState == "complete") {
                    script.onreadystatechange = null;
                    callback();
                }
            }
        }
        else {
            // Rest
            script.onload = callback;
        }

        script.src = src;
        appendTo.appendChild(script);
    }
    function _elSupportsAttr(el, attr) {
        return attr in document.createElement(el);
    }
    return {
        generateUUID: _generateUUID,
        are_cookies_enabled: _are_cookies_enabled,
        loadScript: _loadScript,
        elSupportsAttr: _elSupportsAttr,
        loadCss: _loadCss
    }
})();

var rlsXhr = (function () {
    // get запит, дані отримуються в json форматі
    // uri - повна адреса з параметрами, ok - функція, що виконується при при завершенні запиту без помилок; er - при помилці
    // setRequestHeader("X-Requested-With", "XMLHttpRequest") && setRequestHeader("Accept", "application/json")
    function _get(uri, ok, er) {
        var xhr = false;
        try {
            xhr = new XMLHttpRequest();
        }
        catch (e) {
            return true;
        }
        xhr.open("get", uri, true);
        xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
        xhr.setRequestHeader("Accept", "application/json");
        xhr.onreadystatechange = function (e) {
            e = e || window.event
            if (xhr.readyState == 4) {
                if (xhr.status == 200) {
                    ok(xhr);
                }
                else {
                    er(xhr);
                }
            }
        }

        //var params = 'name=' + encodeURIComponent(name) +'&surname=' + encodeURIComponent(surname);
        //uri += uri + '?'+params;
        xhr.send(null);
    }
    function _getxml(uri, ok, er) {
        var xhr = false;
        try {
            xhr = new XMLHttpRequest();
        }
        catch (e) {
            return true;
        }
        xhr.open("get", uri, true);
        xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
        xhr.setRequestHeader("Accept", "application/xml");
        xhr.onreadystatechange = function (e) {
            e = e || window.event
            if (xhr.readyState == 4) {
                if (xhr.status == 200) {
                    ok(xhr);
                }
                else {
                    er(xhr);
                }
            }
        }

        //var params = 'name=' + encodeURIComponent(name) +'&surname=' + encodeURIComponent(surname);
        //uri += uri + '?'+params;
        xhr.send(null);
    }

    // post запит, дані в json форматі
    // setRequestHeader('Content-Type', 'application/json; charset=utf-8')
    function _postjs(uri, data, ok, er) {
        var xhr = false;
        try {
            xhr = new XMLHttpRequest();
        }
        catch (e) {
            return true;
        }
        xhr.open("POST", uri, true);
        xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
        xhr.setRequestHeader("Accept", "application/json");

        xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');

        xhr.onreadystatechange = function (e) {
            e = e || window.event
            if (xhr.readyState == 4) {
                if (xhr.status == 200) {
                    ok(xhr);
                    //alert(xhr.responseText);
                }
                else {
                    er(xhr);
                }
            }
        }
        xhr.send(JSON.stringify(data));
    }

    return {
        postjs: _postjs,
        get: _get,
        getxml:_getxml
    }
})();

var rlsUiBase = (function () {
    function _getOffsetRect(elem) {
        // (1)
        var box = elem.getBoundingClientRect()

        // (2)
        var body = document.body
        var docElem = document.documentElement

        // (3)
        var scrollTop = window.pageYOffset || docElem.scrollTop || body.scrollTop
        var scrollLeft = window.pageXOffset || docElem.scrollLeft || body.scrollLeft

        // (4)
        var clientTop = docElem.clientTop || body.clientTop || 0
        var clientLeft = docElem.clientLeft || body.clientLeft || 0

        // (5)
        var top = box.top + scrollTop - clientTop
        var left = box.left + scrollLeft - clientLeft

        return { top: Math.round(top), left: Math.round(left) }
    }

    var scrollWidth = null;
    function _ScrollWidth() {
        if (scrollWidth) return scrollWidth;
        var div = document.createElement('div');
        div.style.overflowY = 'scroll';
        div.style.width = '50px';
        div.style.height = '50px';
        div.style.visibility = 'hidden';
        document.body.appendChild(div);
        scrollWidth = div.offsetWidth - div.clientWidth;
        document.body.removeChild(div);
        return scrollWidth;
    }

    return {
        getOffsetRect: _getOffsetRect,
        ScrollWidth: _ScrollWidth
    }
})();