function initeditor() {

    CKEDITOR.replace('content', {
        language: 'vi',
        filebrowserImageUploadUrl: '/editor/UploadImage'
    });

}

function showProgressDialog() {
    $('#loading-indicator').show();
}

function hideProgressDialog() {
    $('#loading-indicator').hide();
}

/**show modal**/

function showmodal(id) {
    $('#' + id).modal('show');
}
