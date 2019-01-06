function changeCulture(culture) {
    if (culture === 'en' || culture === 'fr') {
        var urlsParts = window.location.pathname.split('/');
       
        if (urlsParts.length > 1) {
            urlsParts[1] = culture;
        }
        var newUrl = '';
        $.each(urlsParts, function (key, value) {
            newUrl += value;
            newUrl += '/';
        });

        newUrl = newUrl.slice(0, -1);
        window.location.pathname = newUrl;
    }
}

function isEmptyOrSpaces(str) {
    return str === null || str.match(/^ *$/) !== null;
}