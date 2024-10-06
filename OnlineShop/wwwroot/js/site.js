// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/** JavaScript的Font Loading API */
document.fonts.ready.then(function () {
    document.body.classList.add('fonts-loaded');
});