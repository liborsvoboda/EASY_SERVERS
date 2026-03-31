/*Metro Storage Using HERE is Global Variable Definitions*/
//Metro.storage.setItem(key, value) - store any data to key
//Metro.storage.getItem(key, default ) - get stored values with key.


/*WebPages Language Variable*/
if (Metro.storage.getItem('WebPagesLanguage', null ) == null) {
    let WebPagesLanguage = (navigator.language || navigator.userLanguage).indexOf("cs") >= 0 ? "cz" : "en"; 
    Metro.storage.setItem('WebPagesLanguage', WebPagesLanguage);
}

/*WebPages Theme Scheme*/
if (Metro.storage.getItem('WebScheme', null) == null) {
    Metro.storage.setItem('WebScheme', "red-alert.css");
    ChangeSchemeTo(Metro.storage.getItem('WebScheme', null));
} else { ChangeSchemeTo(Metro.storage.getItem('WebScheme', null)); }

//Metro.storage.getItem('ApiToken', null)
//Metro.storage.setItem('ApiToken', data.Token)
//Metro.storage.setItem('UserData', data);







