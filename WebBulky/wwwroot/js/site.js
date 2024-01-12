// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $('#Input_Role').on('change', function () {
        var selection = $('#Input_Role option:selected').text();

        if (selection === 'Company') {
            $('#Input_CompanyId').show();
        } else {
            $('#Input_CompanyId').hide();
        }
    });
});