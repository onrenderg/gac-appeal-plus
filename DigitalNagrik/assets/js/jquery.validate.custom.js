//---------------------------------------------- File Validator----------------------------------------------//
var MaxSizeLimit = (8 * 1024);  //In MB

jQuery.validator.unobtrusive.adapters.add('filevalidation', ['filetypes', 'maxfilesize', 'urlforfilehex'], function (options) {
    options.rules['filevalidation'] = options.params;
    if (options.message) {
        options.messages['filevalidation'] = options.message;
    }
});

jQuery.validator.addMethod('filevalidation', function (value, element, params) {
    var FileTypeValid = false;
    var FileSizeValid = false;

    if (element.files.length < 1) { // No files selected
        FileSizeValid = true;
        FileTypeValid = true;
    }
    else if (!element.files || !element.files[0].size) { // This browser doesn't support the HTML5 API      
        FileSizeValid = true;
        FileTypeValid = true;
    }
    else {
        var _MaxFileSize = params.maxfilesize;
        if (_MaxFileSize === "any")
            FileSizeValid = element.files[0].size < MaxSizeLimit * 1024;
        else {
            FileSizeValid = element.files[0].size < parseInt(_MaxFileSize) * 1024;
        }
        if (FileSizeValid) {
            var _FileTypes = params.filetypes;
            var _Extensions = _FileTypes.split('|');
            const [fileNm, fileExt] = $(element).val().split(/\.(?=[^\.]+$)/);
            if ($.inArray(fileExt.toUpperCase(), _Extensions) !== -1) {
                FileTypeValid = true;
            }
        }
    }
    return FileTypeValid && FileSizeValid;
}, '');

//-----------------------------------------------------------------------------------------------------------//

