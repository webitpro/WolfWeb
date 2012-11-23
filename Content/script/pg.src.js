$(document).ready(function () {
    $('div.actionBox').slideDown('slow');
});

function reload() {
    $('div.actionBox').slideUp('slow', function () {
        location.reload();
        
    });
}

