﻿$("#postForm").click(function () {
    preparScopesForPost();
    $("#rsForm").submit();
});

$('#addScope').click(function () {
    addScopeTr();
});

function deleteScope(elem) {
    $(elem).parent().parent().parent().remove();
}

function addScopeTr() {
    var html = '<tr>';
    html += '<td>';
    html += '<input type="text" id="scopeWording" class="form-control" value="" />';
    html += '</td>';
    html += '<td>';
    html += '<input id="scopeReadWrite" type="checkbox" />';
    html += '</td>';
    html += '<td>';
    html += '<span><i onclick="deleteScope(this)" class="fas fa-2x fa-trash-alt"></i></span>';
    html += '</td>';
    html += '</tr>';

    $("#scopeTBody").append(html);
}

function preparScopesForPost() {
    $('#scopesToPost').html('');

    var count = 0;
    var existingWordings = []
    $('#scopeTBody > tr').each(function () {
        var wording = $(this).find('#scopeWording').val().trim();
        var isReadWrite = $(this).find('#scopeReadWrite').is(":checked");


        if (isEmptyOrSpaces(wording)) {
            $(this).remove();
        }
        else if ($.inArray(wording, existingWordings) !== -1) {
            $(this).remove();
        }
        else {

            existingWordings.push(wording);
            $('#scopesToPost').append('<input type="hidden" name="Scopes[' + count + '].Key" value="' + wording + '" />');
            $('#scopesToPost').append('<input type="hidden" name="Scopes[' + count + '].Value" value="' + isReadWrite + '" />');
            count++;
        }
    });
}