// STARTUP Temp Variables Definitions
let pageLoader;


/*Definitions  of Global System Behaviors */
function ChangeSchemeTo(n) {
    $("#color-scheme").attr("href", "../metro/css/schemes/" + n);
    $("#scheme-name").html(n);
    Metro.storage.setItem('WebScheme', n);
}


/*Start of Global Loading Indicator for All Pages*/
function showPageLoading() {
    if (pageLoader != undefined) { Metro.activity.close(pageLoader); }
    pageLoader = Metro.activity.open({
        type: 'atom',
        style: 'dark',
        /*overlayColor: '#fff',*/
        overlayClickClose: true,
        /*overlayAlpha: 1*/
    });
    window.onload = function () {
        Metro.activity.close(pageLoader);
    };
}
function hidePageLoading() {
    Metro.activity.close(pageLoader);
}

function googleTranslateElementInit() {
    window.onload = function () {
        new google.translate.TranslateElement({
            pageLanguage: 'en',
            //includedLanguages: 'en,cs',
            layout: google.translate.TranslateElement.InlineLayout.HORIZONTAL
        }, 'google_translate_element');
    };
   
}
