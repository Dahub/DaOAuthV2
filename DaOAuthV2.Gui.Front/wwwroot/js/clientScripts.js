function changeScopeSelected(source, id)
{   
    var isChecked = $(source).prop("checked");
    if (isChecked)
        $('#' + id).val('true');
    else
        $('#' + id).val('false');
}