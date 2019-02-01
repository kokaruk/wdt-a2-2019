// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code

window.setTimeout(function () {
    $(".alert:not(.alert-info)").fadeTo(300, 0).slideUp(300, function () {
        $(this).remove();
    });
}, 2500);
