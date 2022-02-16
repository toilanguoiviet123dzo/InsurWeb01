"use strict";

/*for focus items*/
function focusEditor(className) {
    document.getElementsByClassName(className)[0].querySelector("input").focus();
}
/* Warning F5 */
$(window).bind('beforeunload', function (e) {
    return "Load lại trang có thể bị mất dữ liệu. Bạn có muốn tiếp tục..."
    e.preventDefault();
});
/* getDimensions*/
window.getDimensions = function () {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
};


function clickElement(element) {
    element.click();
}

function downloadFile(mimeType, base64String, fileName) {

    var fileDataUrl = "data:" + mimeType + ";base64," + base64String;
    fetch(fileDataUrl)
        .then(response => response.blob())
        .then(blob => {

            //create a link
            var link = window.document.createElement("a");
            link.href = window.URL.createObjectURL(blob, { type: mimeType });
            link.download = fileName;

            //add, click and remove
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        });
}

function uploadFile_FileSelected() {
    var count = document.getElementById('fileToUpload').files.length;
    document.getElementById('details').innerHTML = "";
    for (var index = 0; index < count; index++) {
        var file = document.getElementById('fileToUpload').files\[index\];
        var fileSize = 0;
        if (file.size > 1024 * 1024)
            fileSize = (Math.round(file.size * 100 / (1024 * 1024)) / 100).toString() + 'MB';
        else
            fileSize = (Math.round(file.size * 100 / 1024) / 100).toString() + 'KB';
        document.getElementById('details').innerHTML += 'Name: ' + file.name + '<br>Size: ' + fileSize + '<br>Type: ' + file.type;
        document.getElementById('details').innerHTML += '<p>';
    }
    document.getElementById("ImagePreView").src = "data:image/png;base64," + file;
}